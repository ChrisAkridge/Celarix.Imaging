using System.Drawing;
using System.Linq;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    internal sealed class ZoomableCanvasImageCache : IImageCache
    {
        private readonly Control uiControl;
        private readonly IZoomableCanvasSource source;
        private readonly List<ZoomableCanvasWorkingSet> workingSets = new();
        private readonly List<ImageEntry> mutableVisibleSet = new();
        private bool hasCurrentZoomLevel;
        private bool cleaningMemory;
        private bool disposed;

        public Rectangle ViewportCanvasCoordinates { get; private set; }
        public Rectangle ViewportControlCoordinates { get; private set; }
        public IReadOnlyList<ZoomableCanvasWorkingSet> WorkingSets => workingSets;
        public IReadOnlyList<ImageEntry> VisibleSet => mutableVisibleSet;
        public LoadedImageKind Kind { get; } = LoadedImageKind.ZoomableCanvas;
        public int CurrentZoomLevel { get; private set; }
        public Size ZoomableCanvasTileSize { get; }
        public Size TotalZoomableCanvasPixelSize { get; }
        public int Epoch { get; private set; }
        public long SoftMemoryLimitBytes { get; private set; }
        public long HardMemoryLimitBytes { get; private set; }

        public event EventHandler? VisibleSetChanged;

        public ZoomableCanvasImageCache(Control uiControl,
            Rectangle viewportControlCoordinates,
            IZoomableCanvasSource source,
            long softMemoryLimitBytes = 1L * 1024L * 1024L * 1024L,
            long hardMemoryLimitBytes = 2L * 1024L * 1024L * 1024L)
        {
            ArgumentNullException.ThrowIfNull(uiControl);
            ArgumentNullException.ThrowIfNull(source);

            this.uiControl = uiControl;
            this.source = source;
            ViewportControlCoordinates = viewportControlCoordinates;
            ZoomableCanvasTileSize = source.TilePixelSize;
            TotalZoomableCanvasPixelSize = source.GetTotalLevel0PixelSize();
            SoftMemoryLimitBytes = softMemoryLimitBytes;
            HardMemoryLimitBytes = Math.Max(hardMemoryLimitBytes, softMemoryLimitBytes);
        }

        public void AddWorkingSet(ZoomableCanvasWorkingSet workingSet)
        {
            ArgumentNullException.ThrowIfNull(workingSet);

            workingSets.Add(workingSet);
            workingSet.VisibleSetChanged += WorkingSet_VisibleSetChanged;
            CleanupForMemoryLimits();
            RecomputeVisibleSet();
        }

        public bool RemoveWorkingSet(ZoomableCanvasWorkingSet workingSet)
        {
            ArgumentNullException.ThrowIfNull(workingSet);

            if (!workingSets.Remove(workingSet))
            {
                return false;
            }

            workingSet.VisibleSetChanged -= WorkingSet_VisibleSetChanged;
            workingSet.Dispose();
            RecomputeVisibleSet();
            return true;
        }

        public void ClearWorkingSets()
        {
            foreach (var workingSet in workingSets)
            {
                workingSet.VisibleSetChanged -= WorkingSet_VisibleSetChanged;
                workingSet.Dispose();
            }

            workingSets.Clear();
            RecomputeVisibleSet();
        }

        public void ViewportChanged(Rectangle newViewportCanvasCoordinates)
        {
            ViewportCanvasCoordinates = newViewportCanvasCoordinates;

            var newZoomLevel = GetZoomLevelForViewport(newViewportCanvasCoordinates);
            if (!hasCurrentZoomLevel || CurrentZoomLevel != newZoomLevel)
            {
                HandleZoomLevelChange(newZoomLevel);
            }

            foreach (var workingSet in GetRelevantWorkingSets().ToArray())
            {
                workingSet.ViewportChanged(newViewportCanvasCoordinates);
            }

            CleanupForMemoryLimits();
            CleanupInactiveFallbackWorkingSets();
            RecomputeVisibleSet();
        }

        public void SetViewportControlCoordinates(Rectangle newViewportControlCoordinates)
        {
            ViewportControlCoordinates = newViewportControlCoordinates;
            RecomputeVisibleSet();
        }

        public Rectangle ControlRectangleForCanvasRectangle(Rectangle canvasRect)
        {
            if (ViewportCanvasCoordinates.Width <= 0 || ViewportCanvasCoordinates.Height <= 0)
            {
                return Rectangle.Empty;
            }

            var scaleX = ViewportControlCoordinates.Width / (float)ViewportCanvasCoordinates.Width;
            var scaleY = ViewportControlCoordinates.Height / (float)ViewportCanvasCoordinates.Height;

            var left = (int)MathF.Round((canvasRect.Left - ViewportCanvasCoordinates.Left) * scaleX);
            var top = (int)MathF.Round((canvasRect.Top - ViewportCanvasCoordinates.Top) * scaleY);
            var width = Math.Max(1, (int)MathF.Round(canvasRect.Width * scaleX));
            var height = Math.Max(1, (int)MathF.Round(canvasRect.Height * scaleY));

            return new Rectangle(left, top, width, height);
        }

        public void SetSoftMemoryLimit(long bytes)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(bytes);

            SoftMemoryLimitBytes = bytes;
            if (HardMemoryLimitBytes < SoftMemoryLimitBytes)
            {
                HardMemoryLimitBytes = SoftMemoryLimitBytes;
            }

            CleanupForMemoryLimits();
            RecomputeVisibleSet();
        }

        public void SetHardMemoryLimit(long bytes)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(bytes);

            HardMemoryLimitBytes = Math.Max(bytes, SoftMemoryLimitBytes);
            CleanupForMemoryLimits();
            RecomputeVisibleSet();
        }

        public bool CurrentWorkingSetFullyCoversViewport()
            => GetCurrentWorkingSet()?.FullyCoversViewport(ViewportCanvasCoordinates) == true;

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            ClearWorkingSets();
        }

        private void WorkingSet_VisibleSetChanged(object? sender, EventArgs e)
        {
            CleanupForMemoryLimits();
            CleanupInactiveFallbackWorkingSets();
            RecomputeVisibleSet();
        }

        private IEnumerable<ZoomableCanvasWorkingSet> GetRelevantWorkingSets()
            => workingSets.Where(ws =>
                (ws.ZoomLevel == CurrentZoomLevel && !ws.IsFallback)
                || ws.IsFallback);

        private void HandleZoomLevelChange(int newZoomLevel)
        {
            if (hasCurrentZoomLevel && CurrentZoomLevel == newZoomLevel)
            {
                return;
            }

            hasCurrentZoomLevel = true;
            Epoch += 1;
            CurrentZoomLevel = newZoomLevel;

            if (GetCurrentWorkingSet() == null)
            {
                AddWorkingSet(BuildWorkingSet(CurrentZoomLevel));
            }

            foreach (var workingSet in workingSets)
            {
                if (workingSet.ZoomLevel != CurrentZoomLevel)
                {
                    workingSet.SetFallback(ViewportCanvasCoordinates);
                }
            }
        }

        private int GetZoomLevelForViewport(Rectangle viewportCanvasCoordinates)
        {
            if (ZoomableCanvasTileSize.Width <= 0 || ZoomableCanvasTileSize.Height <= 0 || viewportCanvasCoordinates.Width <= 0 || viewportCanvasCoordinates.Height <= 0)
            {
                throw new InvalidOperationException("Tile size and viewport size must be positive to compute zoom level.");
            }

            var tileSize = ZoomableCanvasTileSize;
            var longAxis = Math.Max(viewportCanvasCoordinates.Width, viewportCanvasCoordinates.Height);
            var longAxisInTiles = longAxis / (float)Math.Max(tileSize.Width, tileSize.Height);
            if (longAxisInTiles <= 1f)
            {
                return 0;
            }

            return (int)Math.Ceiling(Math.Log2(longAxisInTiles));
        }

        private void CleanupInactiveFallbackWorkingSets()
        {
            if (CurrentWorkingSetFullyCoversViewport())
            {
                for (var i = workingSets.Count - 1; i >= 0; i--)
                {
                    if (workingSets[i].IsFallback)
                    {
                        RemoveWorkingSet(workingSets[i]);
                    }
                }

                return;
            }

            for (var i = workingSets.Count - 1; i >= 0; i--)
            {
                var workingSet = workingSets[i];
                if (!workingSet.IsFallback)
                {
                    continue;
                }

                if (workingSet.ActiveLoadCount == 0
                    && workingSet.TotalLoadedBytes == 0
                    && !workingSet.HasVisibleCoverage())
                {
                    RemoveWorkingSet(workingSet);
                }
            }
        }

        private void CleanupForMemoryLimits()
        {
            if (cleaningMemory)
            {
                return;
            }

            cleaningMemory = true;
            try
            {
                CleanupForSoftMemoryLimit();
                CleanupForHardMemoryLimit();
            }
            finally
            {
                cleaningMemory = false;
            }
        }

        private void CleanupForSoftMemoryLimit()
        {
            var totalLoadedBytes = GetTotalLoadedBytes();
            if (totalLoadedBytes <= SoftMemoryLimitBytes)
            {
                return;
            }

            var candidates = workingSets
                .Where(ws => !ws.IsFallback)
                .SelectMany(ws => ws.ImageEntries)
                .Where(entry => entry.State == ImageEntryState.Loaded && !IntersectsViewport(entry))
                .OrderBy(entry => entry.LastUsedTick ?? long.MinValue)
                .ThenByDescending(DistanceFromViewportCenterSquared)
                .ToArray();

            foreach (var candidate in candidates)
            {
                if (totalLoadedBytes <= SoftMemoryLimitBytes)
                {
                    break;
                }

                totalLoadedBytes -= candidate.ByteSize ?? 0;
                candidate.Unload();
            }
        }

        private void CleanupForHardMemoryLimit()
        {
            var totalLoadedBytes = GetTotalLoadedBytes();
            if (totalLoadedBytes <= HardMemoryLimitBytes)
            {
                return;
            }

            var currentWorkingSet = GetCurrentWorkingSet();
            var candidates = workingSets
                .SelectMany(ws => ws.ImageEntries.Select(entry => new
                {
                    WorkingSet = ws,
                    Entry = entry,
                    Visible = IntersectsViewport(entry),
                    IsCurrent = ws == currentWorkingSet
                }))
                .Where(x => x.Entry.State == ImageEntryState.Loaded)
                .OrderBy(x => x.Visible)
                .ThenBy(x => x.IsCurrent)
                .ThenBy(x => x.Entry.LastUsedTick ?? long.MinValue)
                .ThenByDescending(x => DistanceFromViewportCenterSquared(x.Entry))
                .ToArray();

            foreach (var candidate in candidates)
            {
                if (totalLoadedBytes <= HardMemoryLimitBytes)
                {
                    break;
                }

                totalLoadedBytes -= candidate.Entry.ByteSize ?? 0;
                candidate.Entry.Unload();
            }
        }

        private void RecomputeVisibleSet()
        {
            mutableVisibleSet.Clear();

            var orderedWorkingSets = workingSets
                .OrderByDescending(ws => ws.IsFallback)
                .ThenBy(ws => ws.ZoomLevel);

            foreach (var workingSet in orderedWorkingSets)
            {
                mutableVisibleSet.AddRange(workingSet.VisibleSet);
            }

            OnVisibleSetChanged();
        }

        private ZoomableCanvasWorkingSet? GetCurrentWorkingSet()
            => workingSets.FirstOrDefault(ws => !ws.IsFallback && ws.ZoomLevel == CurrentZoomLevel);

        private ZoomableCanvasWorkingSet BuildWorkingSet(int zoomLevel)
        {
            var tileCount = source.GetTileCountForZoomLevel(zoomLevel);
            var imageEntries = new List<ImageEntry>(tileCount.Width * tileCount.Height);

            for (var tileY = 0; tileY < tileCount.Height; tileY++)
            {
                for (var tileX = 0; tileX < tileCount.Width; tileX++)
                {
                    var currentTileX = tileX;
                    var currentTileY = tileY;
                    var canvasRectangle = source.GetCanvasRectangleForTile(zoomLevel, currentTileX, currentTileY);
                    var canvasImage = new CanvasImage
                    {
                        Factory = options => source.LoadTileAsync(options),
                        SizeEstimator = () => (long)source.TilePixelSize.Width * source.TilePixelSize.Height * 4,
                        CanvasX = canvasRectangle.X,
                        CanvasY = canvasRectangle.Y,
                        Width = canvasRectangle.Width,
                        Height = canvasRectangle.Height,
                        OnlyAtZoomLevel = zoomLevel
                    };

                    var imageEntry = new ImageEntry(uiControl, cancellationToken => new FactoryOptions
                    {
                        CancellationToken = cancellationToken,
                        Kind = LoadedImageKind.ZoomableCanvas,
                        ZoomLevel = zoomLevel,
                        TileX = currentTileX,
                        TileY = currentTileY,
                        TileEdgeLength = source.TilePixelSize.Width
                    })
                    {
                        EntryKey = new ImageEntryKey
                        {
                            Kind = LoadedImageKind.ZoomableCanvas,
                            ZoomLevel = zoomLevel,
                            CanvasX = canvasRectangle.X,
                            CanvasY = canvasRectangle.Y
                        },
                        CanvasImage = canvasImage
                    };
                    imageEntries.Add(imageEntry);
                }
            }

            var workingSet = new ZoomableCanvasWorkingSet(Epoch, zoomLevel, imageEntries);
            foreach (var imageEntry in imageEntries)
            {
                imageEntry.Parent = workingSet;
            }

            return workingSet;
        }

        private long GetTotalLoadedBytes()
            => workingSets.Sum(ws => ws.TotalLoadedBytes);

        private bool IntersectsViewport(ImageEntry entry)
            => GetCanvasRectangle(entry).IntersectsWith(ViewportCanvasCoordinates);

        private Rectangle GetCanvasRectangle(ImageEntry entry)
            => new(entry.CanvasImage.CanvasX,
                entry.CanvasImage.CanvasY,
                entry.CanvasImage.Width,
                entry.CanvasImage.Height);

        private float DistanceFromViewportCenterSquared(ImageEntry entry)
        {
            var canvasRectangle = GetCanvasRectangle(entry);
            var viewportCenterX = ViewportCanvasCoordinates.Left + (ViewportCanvasCoordinates.Width / 2f);
            var viewportCenterY = ViewportCanvasCoordinates.Top + (ViewportCanvasCoordinates.Height / 2f);
            var entryCenterX = canvasRectangle.Left + (canvasRectangle.Width / 2f);
            var entryCenterY = canvasRectangle.Top + (canvasRectangle.Height / 2f);
            var deltaX = entryCenterX - viewportCenterX;
            var deltaY = entryCenterY - viewportCenterY;
            return (deltaX * deltaX) + (deltaY * deltaY);
        }

        private void OnVisibleSetChanged() => VisibleSetChanged?.Invoke(this, EventArgs.Empty);
    }
}
