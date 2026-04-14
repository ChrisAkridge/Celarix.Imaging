using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine("ImageEntry: Entering StartLoad");
            if (LoadState == ImageEntryLoadState.Loaded || LoadState == ImageEntryLoadState.Loading)
            {
                Debug.WriteLine("ImageEntry: Already loaded or loading, skipping StartLoad");
                return;
            }
            
            LoadState = ImageEntryLoadState.Loading;
            _cancellationTokenSource = new CancellationTokenSource();

            _loadTask = _canvasImage.Factory(_cancellationTokenSource.Token)
                .ContinueWith(t =>
                {
                    Debug.WriteLine($"ImageEntry: Load task completed with status {t.Status}, starting UI update");
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        Debug.WriteLine($"ImageEntry: Load task was {(t.IsCanceled ? "canceled" : "faulted")}, resetting state");
                        LoadState = ImageEntryLoadState.Unloaded;
                        return;
                    }

                    LoadState = ImageEntryLoadState.Loaded;
                    control.BeginInvoke(() =>
                    {
                        Debug.WriteLine($"ImageEntry: Updating UI with loaded image, disposing old image if exists");
                        _skImage?.Dispose();
                        _skImage = t.Result;
                        ByteSize = (long)t.Result.Width * t.Result.Height * 4;
                        control.Invalidate();
                    });
                });
        }

        public void Cancel()
        {
            Debug.WriteLine("ImageEntry: Entering Cancel");
            if (LoadState != ImageEntryLoadState.Loading)
            {
                Debug.WriteLine("ImageEntry: Not currently loading, skipping Cancel");
                return;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            LoadState = ImageEntryLoadState.Unloaded;
        }

        public void Evict()
        {
            Debug.WriteLine("ImageEntry: Entering Evict");
            _skImage?.Dispose();
            _skImage = null;
            ByteSize = 0;
            IsEvictable = false;
        }
    }
}
