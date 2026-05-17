using SkiaSharp;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2
{
    internal sealed class ImageEntry : IDisposable
    {
        private readonly Control uiControl;
        private readonly Func<CancellationToken, FactoryOptions> factoryOptionsFactory;
        private Task<SKImage>? activeLoadTask;
        private bool disposed;

        public ImageEntryState State { get; private set; } = ImageEntryState.Unloaded;
        public required ImageEntryKey EntryKey { get; init; }
        public ZoomableCanvasWorkingSet? Parent { get; set; }
        public required CanvasImage CanvasImage { get; init; }
        public CancellationTokenSource? LoadCancellationTokenSource { get; private set; }
        public SKImage? Image { get; private set; }
        public long? ByteSize { get; private set; }
        public long? LastUsedTick { get; private set; }
        public int? LoadGeneration { get; private set; }

        public event EventHandler? ImageLoadedOrUnloaded;

        public ImageEntry(Control uiControl,
            Func<CancellationToken, FactoryOptions> factoryOptionsFactory)
        {
            this.uiControl = uiControl ?? throw new ArgumentNullException(nameof(uiControl));
            this.factoryOptionsFactory = factoryOptionsFactory ?? throw new ArgumentNullException(nameof(factoryOptionsFactory));
        }

        public void BeginLoad()
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (State is ImageEntryState.Loaded or ImageEntryState.Loading)
            {
                return;
            }

            var nextGeneration = (LoadGeneration ?? 0) + 1;
            LoadGeneration = nextGeneration;
            State = ImageEntryState.Loading;
            var parentEpoch = Parent?.Epoch;

            LoadCancellationTokenSource?.Dispose();
            LoadCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = LoadCancellationTokenSource.Token;
            var options = factoryOptionsFactory(cancellationToken);
            activeLoadTask = CanvasImage.Factory(options);

            _ = activeLoadTask.ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    PostLoadCompleted(task.Result, nextGeneration, parentEpoch);
                }
                else if (task.IsCanceled)
                {
                    PostLoadCanceled(nextGeneration, parentEpoch);
                }
                else if (task.IsFaulted)
                {
                    PostLoadFaulted(nextGeneration, parentEpoch, task.Exception);
                }
            }, TaskScheduler.Default);
        }

        public void CancelLoad()
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (State != ImageEntryState.Loading)
            {
                return;
            }

            LoadCancellationTokenSource?.Cancel();
            LoadCancellationTokenSource?.Dispose();
            LoadCancellationTokenSource = null;
            activeLoadTask = null;
            State = ImageEntryState.Unloaded;
        }

        public void OnLoadCompleted(SKImage image, int loadGeneration, int? parentEpoch, long byteSize)
        {
            ArgumentNullException.ThrowIfNull(image);
            ObjectDisposedException.ThrowIf(disposed, this);

            if (State != ImageEntryState.Loading
                || LoadGeneration != loadGeneration
                || (Parent != null && (Parent.Epoch != parentEpoch || !Parent.ContainsEntry(this))))
            {
                image.Dispose();
                return;
            }

            LoadCancellationTokenSource?.Dispose();
            LoadCancellationTokenSource = null;
            activeLoadTask = null;

            Image?.Dispose();
            Image = image;
            ByteSize = byteSize;
            LastUsedTick = Environment.TickCount64;
            State = ImageEntryState.Loaded;
            ImageLoadedOrUnloaded?.Invoke(this, EventArgs.Empty);
        }

        public void Unload()
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            if (State == ImageEntryState.Loading)
            {
                CancelLoad();
            }

            if (Image != null)
            {
                Image.Dispose();
                Image = null;
                ByteSize = null;
                LastUsedTick = null;
                State = ImageEntryState.Unloaded;
                ImageLoadedOrUnloaded?.Invoke(this, EventArgs.Empty);
            }
            else if (State != ImageEntryState.Faulted)
            {
                State = ImageEntryState.Unloaded;
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            LoadCancellationTokenSource?.Cancel();
            LoadCancellationTokenSource?.Dispose();
            LoadCancellationTokenSource = null;
            activeLoadTask = null;

            Image?.Dispose();
            Image = null;
            ByteSize = null;
            LastUsedTick = null;
            State = ImageEntryState.Disposed;
        }

        private void PostLoadCompleted(SKImage image, int loadGeneration, int? parentEpoch)
        {
            if (uiControl.IsDisposed)
            {
                image.Dispose();
                return;
            }

            uiControl.BeginInvoke(new Action(() =>
            {
                if (disposed)
                {
                    image.Dispose();
                    return;
                }

                OnLoadCompleted(image, loadGeneration, parentEpoch, (long)image.Width * image.Height * 4);
            }));
        }

        private void PostLoadCanceled(int loadGeneration, int? parentEpoch)
        {
            if (uiControl.IsDisposed)
            {
                return;
            }

            uiControl.BeginInvoke(new Action(() =>
            {
                if (disposed)
                {
                    return;
                }

                if (State == ImageEntryState.Loading
                    && LoadGeneration == loadGeneration
                    && (Parent == null || (Parent.Epoch == parentEpoch && Parent.ContainsEntry(this))))
                {
                    LoadCancellationTokenSource?.Dispose();
                    LoadCancellationTokenSource = null;
                    activeLoadTask = null;
                    State = ImageEntryState.Unloaded;
                }
            }));
        }

        private void PostLoadFaulted(int loadGeneration, int? parentEpoch, AggregateException? exception)
        {
            if (uiControl.IsDisposed)
            {
                return;
            }

            uiControl.BeginInvoke(new Action(() =>
            {
                if (disposed)
                {
                    return;
                }

                if (State == ImageEntryState.Loading
                    && LoadGeneration == loadGeneration
                    && (Parent == null || (Parent.Epoch == parentEpoch && Parent.ContainsEntry(this))))
                {
                    LoadCancellationTokenSource?.Dispose();
                    LoadCancellationTokenSource = null;
                    activeLoadTask = null;
                    State = ImageEntryState.Faulted;
                }
            }));
        }
    }

    internal enum ImageEntryState
    {
        Unloaded,
        Loading,
        Loaded,
        Faulted,
        Disposed
    }
}
