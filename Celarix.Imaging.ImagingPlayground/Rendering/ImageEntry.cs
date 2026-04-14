using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering
{
    internal sealed class ImageEntry
    {
        private readonly CanvasImage _canvasImage;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _loadTask;
        private SKImage? _skImage;

        public ImageEntryLoadState LoadState { get; private set; }
        public long ByteSize { get; private set; }
        public SKImage? SKImage
        {
            get => _skImage;
            internal set
            {
                _skImage = value;
                if (value != null)
                {
                    ByteSize = value.Width * value.Height * 4; // Assuming 4 bytes per pixel (RGBA)
                    LastUsedTick = DateTime.UtcNow.Ticks;
                }
                else
                {
                    ByteSize = 0;
                }
            }
        }
        public bool IsEvictable { get; set; }
        public long LastUsedTick { get; set; }
        public SKPoint Position => _canvasImage.Position;

        public ImageEntry(CanvasImage canvasImage)
        {
            _canvasImage = canvasImage;
            LoadState = ImageEntryLoadState.Unloaded;
        }

        public void StartLoad(Control control)
        {
            if (LoadState == ImageEntryLoadState.Loaded || LoadState == ImageEntryLoadState.Loading) { return; }
            
            LoadState = ImageEntryLoadState.Loading;
            _cancellationTokenSource = new CancellationTokenSource();

            _loadTask = _canvasImage.Factory(_cancellationTokenSource.Token)
                .ContinueWith(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        LoadState = ImageEntryLoadState.Unloaded;
                        return;
                    }

                    control.BeginInvoke(() =>
                    {
                        _skImage?.Dispose();
                        _skImage = t.Result;
                        ByteSize = (long)t.Result.Width * t.Result.Height * 4;
                        control.Invalidate();
                    });
                });
        }

        public void Cancel()
        {
            if (LoadState != ImageEntryLoadState.Loading) { return; }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            LoadState = ImageEntryLoadState.Unloaded;
        }

        public void Evict()
        {
            _skImage?.Dispose();
            _skImage = null;
            ByteSize = 0;
            IsEvictable = false;
        }
    }
}
