using Serilog;
using SkiaSharp;
using System.Drawing;
using System.Linq;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    internal sealed class StripedImageCache : IImageCache
    {
        private readonly List<ImageEntry> entries = new();
        private readonly List<ImageEntry> visibleSet = new();
        private bool disposed;

        public Rectangle ViewportCanvasCoordinates { get; private set; }
        public Rectangle ViewportControlCoordinates { get; private set; }
        public IReadOnlyList<ImageEntry> VisibleSet => visibleSet;
        public IReadOnlyList<Rectangle> StripeRects { get; }
        public IReadOnlyList<ImageEntry> Entries => entries;

        public long SoftMemoryLimitBytes { get; private set; }
        public long HardMemoryLimitBytes { get; private set; }

        public event EventHandler? VisibleSetChanged;

        public StripedImageCache(Control uiControl,
            Rectangle viewportControlCoordinates,
            IReadOnlyList<Rectangle> stripeRects,
            Func<FactoryOptions, int, Task<SKImage>> stripeFactory,
            long softMemoryLimitBytes = 1L * 1024L * 1024L * 1024L,
            long hardMemoryLimitBytes = 2L * 1024L * 1024L * 1024L)
        {
            ArgumentNullException.ThrowIfNull(uiControl);
            ArgumentNullException.ThrowIfNull(stripeRects);
            ArgumentNullException.ThrowIfNull(stripeFactory);

            ViewportControlCoordinates = viewportControlCoordinates;
            StripeRects = stripeRects.ToArray();
            SoftMemoryLimitBytes = softMemoryLimitBytes;
            HardMemoryLimitBytes = Math.Max(hardMemoryLimitBytes, softMemoryLimitBytes);

            for (var i = 0; i < StripeRects.Count; i++)
            {
                var stripeIndex = i;
                var stripeRect = StripeRects[stripeIndex];
                var canvasImage = new CanvasImage
                {
                    Factory = options => stripeFactory(options, stripeIndex),
                    SizeEstimator = () => (long)stripeRect.Width * stripeRect.Height * 4,
                    CanvasX = stripeRect.X,
                    CanvasY = stripeRect.Y,
                    Width = stripeRect.Width,
                    Height = stripeRect.Height
                };

                var entry = new ImageEntry(uiControl, cancellationToken => new StripedFactoryOptions(cancellationToken, stripeIndex))
                {
                    EntryKey = new ImageEntryKey
                    {
                        Kind = LoadedImageKind.Striped,
                        ZoomLevel = null,
                        CanvasX = stripeRect.X,
                        CanvasY = stripeRect.Y
                    },
                    CanvasImage = canvasImage
                };
                entry.ImageLoadedOrUnloaded += Entry_ImageLoadedOrUnloaded;
                entries.Add(entry);
            }
        }

        public void ViewportChanged(Rectangle newViewportCanvasCoordinates)
        {
            ViewportCanvasCoordinates = newViewportCanvasCoordinates;
            RecomputeVisibleSet();
            CleanupForSoftMemoryLimit();
        }

        public void SetViewportControlCoordinates(Rectangle newViewportControlCoordinates)
        {
            ViewportControlCoordinates = newViewportControlCoordinates;
            OnVisibleSetChanged();
            Log.Debug("Viewport moved to {Coordinates}", ViewportCanvasCoordinates);
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

        public void RecomputeVisibleSet()
        {
            visibleSet.Clear();

            foreach (var entry in entries)
            {
                var stripeRect = new Rectangle(entry.CanvasImage.CanvasX,
                    entry.CanvasImage.CanvasY,
                    entry.CanvasImage.Width,
                    entry.CanvasImage.Height);
                var intersectsViewport = stripeRect.IntersectsWith(ViewportCanvasCoordinates);

                if (intersectsViewport)
                {
                    if (entry.State is ImageEntryState.Unloaded or ImageEntryState.Faulted)
                    {
                        entry.BeginLoad();
                    }

                    if (entry.State == ImageEntryState.Loaded)
                    {
                        visibleSet.Add(entry);
                    }
                }
                else if (entry.State == ImageEntryState.Loading)
                {
                    entry.CancelLoad();
                }
            }

            OnVisibleSetChanged();
        }

        public void CleanupForSoftMemoryLimit()
        {
            if (SoftMemoryLimitBytes < 0)
            {
                return;
            }
            var totalLoadedBytes = entries.Sum(e => e.ByteSize ?? 0);
            if (totalLoadedBytes <= SoftMemoryLimitBytes)
            {
                return;
            }

            Log.Debug("Loaded bytes {TotalLoadedBytes} exceed soft memory limit {SoftMemoryLimitBytes}; starting cleanup",
                totalLoadedBytes.FormatBytes(),
                SoftMemoryLimitBytes.FormatBytes());

            var viewportCenterY = ViewportCanvasCoordinates.Top + (ViewportCanvasCoordinates.Height / 2f);
            var offscreenLoadedEntries = entries
                .Where(e => e.State == ImageEntryState.Loaded
                    && !new Rectangle(e.CanvasImage.CanvasX,
                        e.CanvasImage.CanvasY,
                        e.CanvasImage.Width,
                        e.CanvasImage.Height).IntersectsWith(ViewportCanvasCoordinates))
                .OrderByDescending(e => DistanceFromViewportCenterSquared(e.CanvasImage, viewportCenterY))
                .ThenBy(e => e.LastUsedTick ?? long.MinValue)
                .ToArray();

            foreach (var entry in offscreenLoadedEntries)
            {
                if (totalLoadedBytes <= SoftMemoryLimitBytes)
                {
                    break;
                }

                totalLoadedBytes -= entry.ByteSize ?? 0;
                entry.Unload();
            }

            Log.Debug("Finished soft memory limit cleanup; total loaded bytes now {TotalLoadedBytes}",
                totalLoadedBytes.FormatBytes());
        }

        public void SetSoftMemoryLimit(long bytes)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(bytes);

            SoftMemoryLimitBytes = bytes;
            if (HardMemoryLimitBytes < SoftMemoryLimitBytes)
            {
                HardMemoryLimitBytes = SoftMemoryLimitBytes;
            }

            CleanupForSoftMemoryLimit();
        }

        public void SetHardMemoryLimit(long bytes)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(bytes);

            HardMemoryLimitBytes = Math.Max(bytes, SoftMemoryLimitBytes);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            foreach (var entry in entries)
            {
                entry.ImageLoadedOrUnloaded -= Entry_ImageLoadedOrUnloaded;
                entry.Dispose();
            }

            entries.Clear();
            visibleSet.Clear();
        }

        private void Entry_ImageLoadedOrUnloaded(object? sender, EventArgs e)
        {
            CleanupForSoftMemoryLimit();
            RecomputeVisibleSet();
        }

        private static float DistanceFromViewportCenterSquared(CanvasImage canvasImage, float viewportCenterY)
        {
            var stripeCenterY = canvasImage.CanvasY + (canvasImage.Height / 2f);
            var deltaY = stripeCenterY - viewportCenterY;
            return deltaY * deltaY;
        }

        private void OnVisibleSetChanged() => VisibleSetChanged?.Invoke(this, EventArgs.Empty);
    }
}
