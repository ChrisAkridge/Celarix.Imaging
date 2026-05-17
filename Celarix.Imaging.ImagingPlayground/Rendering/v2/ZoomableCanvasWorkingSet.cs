using System.Drawing;
using System.Linq;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    internal sealed class ZoomableCanvasWorkingSet : IDisposable
    {
        private Rectangle currentViewport;
        private readonly List<ImageEntry> mutableVisibleSet = new();
        private bool recomputingVisibleSet;
        private bool visibleSetDirty;
        private bool disposed;

        public WorkingSetState State { get; private set; } = WorkingSetState.Building;
        public int Epoch { get; }
        public int ZoomLevel { get; }
        public bool IsFallback { get; private set; }
        public IReadOnlyList<ImageEntry> ImageEntries { get; }
        public IReadOnlyList<ImageEntry> VisibleSet => mutableVisibleSet;

        public long TotalLoadedBytes => ImageEntries.Sum(e => e.ByteSize ?? 0L);
        public int ActiveLoadCount => ImageEntries.Count(e => e.State == ImageEntryState.Loading);

        public event EventHandler? VisibleSetChanged;

        public ZoomableCanvasWorkingSet(int epoch,
            int zoomLevel,
            IEnumerable<ImageEntry> imageEntries)
        {
            Epoch = epoch;
            ZoomLevel = zoomLevel;
            ImageEntries = imageEntries?.ToList()
                ?? throw new ArgumentNullException(nameof(imageEntries));

            foreach (var imageEntry in ImageEntries)
            {
                imageEntry.ImageLoadedOrUnloaded += ImageEntry_ImageLoadedOrUnloaded;
            }

            State = WorkingSetState.Active;
        }

        public void SetFallback(Rectangle viewport)
        {
            if (IsFallback)
            {
                return;
            }

            IsFallback = true;
            State = WorkingSetState.Superseded;
            currentViewport = viewport;

            foreach (var imageEntry in ImageEntries)
            {
                if (imageEntry.State == ImageEntryState.Loading)
                {
                    imageEntry.CancelLoad();
                }

                var canvasRectangle = new Rectangle(
                    imageEntry.CanvasImage.CanvasX,
                    imageEntry.CanvasImage.CanvasY,
                    imageEntry.CanvasImage.Width,
                    imageEntry.CanvasImage.Height);
                if (!currentViewport.IntersectsWith(canvasRectangle) && imageEntry.State == ImageEntryState.Loaded)
                {
                    imageEntry.Unload();
                }
            }

            RequestVisibleSetRecompute();
        }

        public void ImageLoadedOrUnloaded()
        {
            RequestVisibleSetRecompute();
        }

        public void ViewportChanged(Rectangle viewport)
        {
            currentViewport = viewport;
            RequestVisibleSetRecompute();
        }

        public bool ContainsEntry(ImageEntry imageEntry) => ImageEntries.Contains(imageEntry);

        public bool HasVisibleCoverage() => mutableVisibleSet.Count > 0;

        public bool FullyCoversViewport(Rectangle viewport)
        {
            if (viewport.Width <= 0 || viewport.Height <= 0)
            {
                return false;
            }

            long coveredArea = 0;
            foreach (var imageEntry in mutableVisibleSet)
            {
                var canvasRectangle = new Rectangle(
                    imageEntry.CanvasImage.CanvasX,
                    imageEntry.CanvasImage.CanvasY,
                    imageEntry.CanvasImage.Width,
                    imageEntry.CanvasImage.Height);
                var intersection = Rectangle.Intersect(viewport, canvasRectangle);
                if (!intersection.IsEmpty)
                {
                    coveredArea += (long)intersection.Width * intersection.Height;
                }
            }

            return coveredArea >= (long)viewport.Width * viewport.Height;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            foreach (var imageEntry in ImageEntries)
            {
                imageEntry.ImageLoadedOrUnloaded -= ImageEntry_ImageLoadedOrUnloaded;
                imageEntry.Dispose();
            }

            mutableVisibleSet.Clear();
        }

        private void ImageEntry_ImageLoadedOrUnloaded(object? sender, EventArgs e)
        {
            ImageLoadedOrUnloaded();
        }

        private void RequestVisibleSetRecompute()
        {
            if (recomputingVisibleSet)
            {
                visibleSetDirty = true;
                return;
            }

            do
            {
                visibleSetDirty = false;
                recomputingVisibleSet = true;
                RecomputeVisibleSetAndLoadsCore();
                recomputingVisibleSet = false;
            } while (visibleSetDirty);
        }

        private void RecomputeVisibleSetAndLoadsCore()
        {
            mutableVisibleSet.Clear();

            foreach (var imageEntry in ImageEntries)
            {
                var canvasRectangle = new Rectangle(
                    imageEntry.CanvasImage.CanvasX,
                    imageEntry.CanvasImage.CanvasY,
                    imageEntry.CanvasImage.Width,
                    imageEntry.CanvasImage.Height);

                if (!currentViewport.IntersectsWith(canvasRectangle))
                {
                    if (imageEntry.State == ImageEntryState.Loading)
                    {
                        imageEntry.CancelLoad();
                    }
                    else if (IsFallback && imageEntry.State == ImageEntryState.Loaded)
                    {
                        imageEntry.Unload();
                    }

                    continue;
                }

                if (!IsFallback && (imageEntry.State is ImageEntryState.Unloaded or ImageEntryState.Faulted))
                {
                    imageEntry.BeginLoad();
                }

                if (imageEntry.State == ImageEntryState.Loaded)
                {
                    mutableVisibleSet.Add(imageEntry);
                }
            }

            State = IsFallback
                ? WorkingSetState.Superseded
                : ActiveLoadCount > 0 ? WorkingSetState.Loading : WorkingSetState.Active;
            OnVisibleSetChanged();
        }

        private void OnVisibleSetChanged() => VisibleSetChanged?.Invoke(this, EventArgs.Empty);
    }

    internal enum WorkingSetState
    {
        Building,
        Loading,
        Active,
        Superseded,
        Canceled
    }
}
