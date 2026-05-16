class FactoryOptions
{
    CancellationToken CancellationToken { get; }
    LoadedImageKind Kind { get; }

    // Used only if zoomable canvas; otherwise null
    int? ZoomLevel { get; }
    int? TileX { get; }
    int? TileY { get; }
}

class CanvasImage
{
    Func<FactoryOptions, Task<SKImage>> _imageFactory;
    public int CanvasX { get; } // pixel coordinates; when loading a zoomable canvas, we can convert tile coordinates easily to pixel coordinates by multiplying by the tile size (e.g. 256)
    public int CanvasY { get; }
    public int? Width { get; }
    public int? Height { get; }
    public int? OnlyAtZoomLevel { get; } // if null, show at all zoom levels

    public override int GetHashCode()
    {
        return HashCode.Combine(CanvasX, CanvasY, OnlyAtZoomLevel);
    }
}

class ImageEntryKey : IEquatable<ImageEntryKey>
{
    LoadedImageKind Kind { get; }
    int? ZoomLevel { get; } // if null, applies to all zoom levels
    int CanvasX { get; }
    int CanvasY { get; }

    override bool Equals(object? obj);
    bool Equals(ImageEntryKey? other);

    public override int GetHashCode()
    {
        return HashCode.Combine(Kind, ZoomLevel, CanvasX, CanvasY);
    }
}

class ImageEntry : IDisposable
{
    ImageEntryState State { get; }
    ImageEntryKey EntryKey { get; }
    WorkingSet Parent { get; }

    CanvasImage CanvasImage { get; }
    SKImage? Image { get; }
    CancellationTokenSource? LoadCancellationTokenSource { get; }
    long? ByteSize { get; }
    long? LastUsedTick { get; }
    int? LoadGeneration { get; }
    
    void BeginLoad();   // can be called from State = Unloaded or Faulted both
    // If image is loaded, this is a no-op and throws no error
    // Lets callers always ask to load and not have to care if it is loaded
    void CancelLoad();  // I think this is alright? maybe don't need ANOTHER CTS here?
    void OnLoadCompleted(SKImage image, int loadGeneration, long byteSize); // called by the task we spawn in BeginLoad - task calls the factory, then calls this; this just sets properties
    void Unload();
    void Dispose();
}

enum ImageEntryState
{
    Unloaded,
    Loading,
    Loaded,
    Faulted,
    Disposed
}

class WorkingSet
{
    WorkingSetState State { get; }     // yes these all only list get
    int Epoch { get; }                 // not because I think these should all be readonly
    int? ZoomLevel { get; }  // for SingleImage/Striped, this is null, meaning it applies to all zoom levels
    bool IsFallback { get; } // false means "current zoom level", true means "show until enough of new working set is loaded such that all images in this working set are covered over"
    long TotalLoadedBytes { get; }
    int ActiveLoadCount { get; } // computed property, sum of ImageEntries where State == Loading
    IReadOnlyList<ImageEntry> ImageEntries { get; } // all the ImageEntries that belong to this zoom level

    IReadOnlyList<ImageEntry> VisibleSet { get; private set; } // recomputed on BOTH viewport change and image load/unload, only in-viewport images
    event EventHandler? VisibleSetChanged; // raised on load/unload

    WorkingSet(...);

    void SetFallback(); // sets IsFallback == true, cancels all active loads
    // no logic for Disposed/Superseded. why?
    // if IsFallback
    //    && ActiveLoadCount == 0
    //    && (TotalLoadedBytes == 0
    //       || none of the loaded tiles are in the viewport)
    // then ImageCache can just remove this instance from the list
    // of working sets

    void ImageLoadedOrUnloaded();

    void ViewportChanged(Rectangle viewport); // computs visible ImageEntries, starts loads on all
    // viewport can zoom a little before loading another zoom level, so this handles both panning and zooming
    void OnVisibleSetChanged();
}

class ImageCache
{
    // maybe these rects and sizes should be Skia types
    Rectangle ViewportCanvasCoordinates { get; }
    Rectangle ViewportControlCoordinates { get; } // always (0, 0, control width, control height)
    List<WorkingSet> WorkingSets { get; }   // or a private readonly field or whatever
    IReadOnlyList<ImageEntry> VisibleSet { get; } // computed property based on the VisibleSet of each WorkingSet; recomputed inside WorkingSets on any VisibleSetChanged event from any WorkingSet, or when the ViewportCanvasCoordinates changes
    // just a big concat, really
    LoadedImageKind Kind { get; }
    int? CurrentZoomLevel { get; } // computed property based on ViewportCanvasCoordinates and tile size
    Size? ZoomableCanvasTileSize { get; }
    Size? TotalZoomableCanvasPixelSize { get; }
    // it's worth noting more about zoomable canvases
    // it's like we have one really big image we chopped up into square tiles
    // say we have a 104856x1048576 image and we chop it up into 1024x1024 tiles
    // so it's still 1 mebipixel across, but it's also 1024 tiles across at zoom level 0
    // zoom level 1 tiles are still 1024x1024 PNGs on disk
    // but they represent 4 zoom level 0 tiles shrunk down to 25% each
    // so a 2048x2048 pixel block, and there are 512 such tiles across now
    // at zoom level 2, each til is a 4096x4096 block and there's 256 tiles across
    // the highest zoom level is when the whole canvas fits in one tile
    // log_2(bigImageMaxDimension) - 10
    int Epoch { get; }
    long SoftMemoryLimitBytes { get; } // above this, invisible loaded ImageEntries are unloaded
    long HardMemoryLimitBytes { get; } // above this, even visible ImageEntries are unloaded until we drop back below this yes it will make visible stuff vanish no I don't care
    // but we should favor completely unloading IsFallback == true WorkingSets even if they're still visible because we want to clear them anyway at some point

    event EventHandler? VisibleSetChanged; // i luv 2 bubble up events 4 multiple layers

    // it feels more pure to let the ViewportCanvasCoordinates have a setter
    // but man that would be such a big setter given that it would update all
    // the working sets which would then cause image loads, feels not very
    // setter-like
    void ViewportChanged(Rectangle newViewportCanvasCoordinates);
    Rectangle ControlRectangleForCanvasRectangle(Rectangle canvasRect);
    void OnVisibleSetChanged();
}

enum LoadedImageKind
{
    SingleImage,   // one big image for the entire canvas; no zoom levels; not tiled; just one big image that we pan around on
    Striped,       // vertical stack of images; no zoom levels
    ZoomableCanvas // grids of tiles at multiple zoom levels; only mode that uses multiple WorkingSets
}

class InfiniteCanvasControl : UserControl, IDisposable
{
    const float MinZoomScale = 1f / 64f; // don't zoom in more than 64x64 control pixels = 1 canvas pixel
    const float MaxZoomVelocity = 1f;
    const float ZoomDampening = 0.9f;

    ImageCache _imageCache;
    List<ImageEntry> _visibleImageEntries;
    SKPoint canvasOffset; // (0, 0) means the top-left of the canvas is aligned with the top-left of the control
    // i.e. (100, 0) means the canvas's origin is now at control coordinates (-100, 0), off the left edge
    // +X means "panned right", -X means "panned left", +Y means "panned down", -Y means "panned up"
    float canvasZoomScale; // 1.0 means 1 control pixel == 1 canvas pixel
    // 2.0 means 1 control pixel == 4 canvas pixels (zoomed out)
    // 0.5 means 4 control pixels == 1 canvas pixel (zoomed in)
    // up means zoom out, down means zoom in

    // Existing stuff from the control
    SKPoint _mouseHoverControlPosition;
    System.Windows.Forms.Timer _hoverTimer;
    SKPoint? _dragStartControlPoint;
    SKPoint? _dragEndControlPoint;
    SKPoint? _dragVelocity;
    SKPoint? _dragInertia;
    bool _isDragging;
    float _zoomVelocity;
    ContextMenuStrip _contextMenu;

    InfiniteCanvasControl();
    void LoadSingleImage(string);
    void LoadSingleInMemoryImage(Image<Rgba32>);
    void LoadStripedImages(string folderPath);
    void LoadZoomableCanvas(string tilesFolderPath, Size tileSize);
    void LoadFactoryImage(Func<FactoryOptions, Task<SKImage>>); // supports all three kinds

    override void OnPaintSurface(SKPaintGLSurfaceEventArgs e);
    override void OnPaint(PaintEventArgs e);
    void OnAnimationFrame(object?, EventArgs);
    void ImageCache_VisibleSetChanged(object? sender, EventArgs e);
    void InfiniteCanvasControl_MouseMove(object sender, MouseEventArgs e);
    void InfiniteCanvasControl_MouseUp(object sender, MouseEventArgs e);
    void InfiniteCanvasControl_MouseDown(object sender, MouseEventArgs e);
    void InfiniteCanvasControl_MouseWheel(object sender, MouseEventArgs e);

    void SetSoftMemoryLimit(long bytes);
    void SetHardMemoryLimit(long bytes);    // prevents hard limit < soft limit

    static SKPoint Multiply(SKPoint point, float scalar);
    static SKPoint Divide(SKPoint point, float divisor);

    void Dispose();
}