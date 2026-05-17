using System.Drawing;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    internal sealed class SingleImageCache : IImageCache
    {
        private readonly List<ImageEntry> visibleSet;
        private bool disposed;

        public Rectangle ViewportCanvasCoordinates { get; private set; }
        public Rectangle ViewportControlCoordinates { get; private set; }
        public IReadOnlyList<ImageEntry> VisibleSet => visibleSet;
        public ImageEntry Entry { get; }

        public long SoftMemoryLimitBytes { get; private set; }
        public long HardMemoryLimitBytes { get; private set; }

        public event EventHandler? VisibleSetChanged;

        public SingleImageCache(Control uiControl,
            Rectangle viewportControlCoordinates,
            CanvasImage canvasImage,
            long softMemoryLimitBytes = 4L * 1024L * 1024L * 1024L,
            long hardMemoryLimitBytes = 8L * 1024L * 1024L * 1024L)
        {
            ArgumentNullException.ThrowIfNull(uiControl);
            ArgumentNullException.ThrowIfNull(canvasImage);

            ViewportControlCoordinates = viewportControlCoordinates;
            SoftMemoryLimitBytes = softMemoryLimitBytes;
            HardMemoryLimitBytes = Math.Max(hardMemoryLimitBytes, softMemoryLimitBytes);

            Entry = new ImageEntry(uiControl, cancellationToken => new FactoryOptions
            {
                CancellationToken = cancellationToken,
                Kind = LoadedImageKind.SingleImage
            })
            {
                EntryKey = new ImageEntryKey
                {
                    Kind = LoadedImageKind.SingleImage,
                    ZoomLevel = null,
                    CanvasX = canvasImage.CanvasX,
                    CanvasY = canvasImage.CanvasY
                },
                CanvasImage = canvasImage
            };
            Entry.ImageLoadedOrUnloaded += Entry_ImageLoadedOrUnloaded;
            Entry.BeginLoad();

            visibleSet = new List<ImageEntry> { Entry };
        }

        public void ViewportChanged(Rectangle newViewportCanvasCoordinates)
        {
            ViewportCanvasCoordinates = newViewportCanvasCoordinates;
            OnVisibleSetChanged();
        }

        public void SetViewportControlCoordinates(Rectangle newViewportControlCoordinates)
        {
            ViewportControlCoordinates = newViewportControlCoordinates;
            OnVisibleSetChanged();
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
            Entry.ImageLoadedOrUnloaded -= Entry_ImageLoadedOrUnloaded;
            Entry.Dispose();
            visibleSet.Clear();
        }

        private void Entry_ImageLoadedOrUnloaded(object? sender, EventArgs e)
        {
            OnVisibleSetChanged();
        }

        private void OnVisibleSetChanged() => VisibleSetChanged?.Invoke(this, EventArgs.Empty);
    }
}
