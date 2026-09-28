interface IImageCache : IDisposable
{
    Rectangle ViewportCanvasCoordinates { get; }
    Rectangle ViewportControlCoordinates { get; }
    IReadOnlyList<ImageEntry> VisibleSet { get; }

    event EventHandler? VisibleSetChanged;

    void ViewportChanged(Rectangle newViewportCanvasCoordinates);
    Rectangle ControlRectangleForCanvasRectangle(Rectangle canvasRect);

    void SetSoftMemoryLimit(long bytes);
    void SetHardMemoryLimit(long bytes);
}

class FactoryOptions
{
    CancellationToken CancellationToken { get; }
    LoadedImageKind Kind { get; }

    // Used only if striped, otherwise null
    int? StripeIndex { get; }

    // Used only if zoomable canvas; otherwise null
    int? ZoomLevel { get; }
    int? TileX { get; }
    int? TileY { get; }
    int? TileEdgeLength { get; }
    SKPoint? TopLeftCanvasPoint { get; } // computed property
}

class CanvasImage
{
    Func<FactoryOptions, Task<SKImage>> _imageFactory;
    int CanvasX { get; }
    int CanvasY { get; }
    int Width { get; }
    int Height { get; }
    int? OnlyAtZoomLevel { get; } // if null, show at all zoom levels
}

class ImageEntryKey : IEquatable<ImageEntryKey>
{
    LoadedImageKind Kind { get; }
    int? ZoomLevel { get; } // if null, applies to all zoom levels
    int CanvasX { get; }
    int CanvasY { get; }

    override bool Equals(object? obj);
    bool Equals(ImageEntryKey? other);
    override int GetHashCode();
}

class ImageEntry : IDisposable
{
    ImageEntryState State { get; }
    ImageEntryKey EntryKey { get; }

    // Parent is only used by zoomable canvas mode
    ZoomableCanvasWorkingSet? Parent { get; }

    CanvasImage CanvasImage { get; }
    SKImage? Image { get; }
    CancellationTokenSource? LoadCancellationTokenSource { get; }
    long? ByteSize { get; }
    long? LastUsedTick { get; }
    int? LoadGeneration { get; }

    event EventHandler? ImageLoadedOrUnloaded;

    void BeginLoad();
    void CancelLoad();
    void OnLoadCompleted(SKImage image, int loadGeneration, int? parentEpoch, long byteSize);
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

class SingleImageCache : IImageCache
{
    Rectangle ViewportCanvasCoordinates { get; }
    Rectangle ViewportControlCoordinates { get; }
    IReadOnlyList<ImageEntry> VisibleSet { get; } // always the one entry

    ImageEntry Entry { get; }

    event EventHandler? VisibleSetChanged;

    SingleImageCache(...);

    void ViewportChanged(Rectangle newViewportCanvasCoordinates);
    Rectangle ControlRectangleForCanvasRectangle(Rectangle canvasRect);
    void SetSoftMemoryLimit(long bytes);
    void SetHardMemoryLimit(long bytes);
    void Dispose();
}

class StripedImageCache : IImageCache
{
    Rectangle ViewportCanvasCoordinates { get; }
    Rectangle ViewportControlCoordinates { get; }
    IReadOnlyList<ImageEntry> VisibleSet { get; } // loaded entries currently intersecting viewport

    IReadOnlyList<Rectangle> StripeRects { get; } // full-width, non-overlapping vertical stripes in stable index order
    IReadOnlyList<ImageEntry> Entries { get; } // one entry per stripe
    long SoftMemoryLimitBytes { get; }
    long HardMemoryLimitBytes { get; }

    event EventHandler? VisibleSetChanged;

    StripedImageCache(...); // takes StripeRects plus a stripe-index-based factory/provider

    void ViewportChanged(Rectangle newViewportCanvasCoordinates);
    Rectangle ControlRectangleForCanvasRectangle(Rectangle canvasRect);
    void RecomputeVisibleSet();
    void CleanupForSoftMemoryLimit();
    void SetSoftMemoryLimit(long bytes);
    void SetHardMemoryLimit(long bytes);
    void Dispose();
}

class ZoomableCanvasWorkingSet
{
    WorkingSetState State { get; }
    int Epoch { get; }
    int ZoomLevel { get; }
    bool IsFallback { get; } // irreversible: once fallback, this set is a dead set walking
    IReadOnlyList<ImageEntry> ImageEntries { get; } // all entries for this zoom level
    IReadOnlyList<ImageEntry> VisibleSet { get; }   // visible loaded subset

    long TotalLoadedBytes { get; }
    int ActiveLoadCount { get; }

    event EventHandler? VisibleSetChanged;

    ZoomableCanvasWorkingSet(...);

    void SetFallback(Rectangle viewport); // cancels active loads and unloads everything outside viewport immediately
    void ImageLoadedOrUnloaded();
    void ViewportChanged(Rectangle viewport);

    bool ContainsEntry(ImageEntry imageEntry);
    bool HasVisibleCoverage();
    bool FullyCoversViewport(Rectangle viewport); // true when this working set alone covers the visible viewport
    void OnVisibleSetChanged();
}

enum WorkingSetState
{
    Building,
    Loading,
    Active,
    Superseded,
    Canceled
}

class ZoomableCanvasImageCache : IImageCache
{
    Rectangle ViewportCanvasCoordinates { get; }
    Rectangle ViewportControlCoordinates { get; }
    IReadOnlyList<ImageEntry> VisibleSet { get; } // concat/layering of working set visible sets

    IReadOnlyList<ZoomableCanvasWorkingSet> WorkingSets { get; }
    int? CurrentZoomLevel { get; }
    Size ZoomableCanvasTileSize { get; }
    Size? TotalZoomableCanvasPixelSize { get; }
    int Epoch { get; }
    long SoftMemoryLimitBytes { get; }
    long HardMemoryLimitBytes { get; }

    event EventHandler? VisibleSetChanged;

    ZoomableCanvasImageCache(...);

    void ViewportChanged(Rectangle newViewportCanvasCoordinates);
    Rectangle ControlRectangleForCanvasRectangle(Rectangle canvasRect);
    void SetSoftMemoryLimit(long bytes);
    void SetHardMemoryLimit(long bytes);

    int GetZoomLevelForViewport(Rectangle viewport);
    void HandleZoomLevelChange(int newZoomLevel);
    bool CurrentWorkingSetFullyCoversViewport();
    void CleanupInactiveFallbackWorkingSets();
    void OnVisibleSetChanged();
    void Dispose();
}

enum LoadedImageKind
{
    SingleImage,
    Striped,
    ZoomableCanvas
}

class InfiniteCanvasControl : UserControl, IDisposable
{
    const float MinZoomScale = 1f / 64f;
    const float MaxZoomVelocity = 1f;
    const float ZoomDampening = 0.9f;

    IImageCache _imageCache;
    List<ImageEntry> _visibleImageEntries;
    SKPoint canvasOffset;
    float canvasZoomScale;

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
    void LoadSingleImage(string filePath);
    void LoadSingleInMemoryImage(Image<Rgba32> image);
    void LoadStripedImages(...); // striped image provider or stripe rects + stripe factory
    void LoadZoomableCanvas(string tilesFolderPath, Size tileSize);
    void LoadFactoryImage(Func<FactoryOptions, Task<SKImage>> imageFactory);

    override void OnPaintSurface(SKPaintGLSurfaceEventArgs e);
    override void OnPaint(PaintEventArgs e);
    void OnAnimationFrame(object?, EventArgs);
    void ImageCache_VisibleSetChanged(object? sender, EventArgs e);
    void InfiniteCanvasControl_MouseMove(object sender, MouseEventArgs e);
    void InfiniteCanvasControl_MouseUp(object sender, MouseEventArgs e);
    void InfiniteCanvasControl_MouseDown(object sender, MouseEventArgs e);
    void InfiniteCanvasControl_MouseWheel(object sender, MouseEventArgs e);

    void SetSoftMemoryLimit(long bytes);
    void SetHardMemoryLimit(long bytes);

    static SKPoint Multiply(SKPoint point, float scalar);
    static SKPoint Divide(SKPoint point, float divisor);

    void Dispose();
}
