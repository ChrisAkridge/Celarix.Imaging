using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering
{
    internal sealed class ImageCache
    {
        private const long MaxMemoryBytes = 500 * 1024 * 1024; // 500 MB

        private List<CanvasImage> _images = new();
        private long _allocatedMemoryBytes = 0L;
        private Viewport viewport;

        public Viewport Viewport { get => viewport; set => viewport = value; }

        public ImageCache()
        {
            // Initialize a default viewport, will be updated later
            Viewport = new Viewport(new SKPoint(0, 0), new SKSize(1000, 1000));
        }

        public void Add(CanvasImage image)
        {
            _images.Add(image);
            if (image.Image == null)
            {
                LoadImage(image);
            }
            LoadAndEvict();
        }

        public void Remove(CanvasImage image)
        {
            _images.Remove(image);
            if (image.Image != null)
            {
                _allocatedMemoryBytes -= image.Image.EstimateImageMemoryUsage();
                image.Image.Dispose();
                image.Image = null;
            }
        }

        public void Clear()
        {
            foreach (var image in _images)
            {
                image.Image?.Dispose();
            }
            _images.Clear();
            _allocatedMemoryBytes = 0L;
        }

        private void LoadImage(CanvasImage image)
        {
            var createdImage = image.Factory();
            image.Image = createdImage;
            _allocatedMemoryBytes += createdImage.EstimateImageMemoryUsage();
        }

        private void LoadAndEvict()
        {
            // Loading phase
            foreach (var image in _images)
            {
                if (image.Image != null || !Viewport.Intersects(image))
                {
                    continue;
                }
                LoadImage(image);
            }

            // Eviction phase
            if (_allocatedMemoryBytes <= MaxMemoryBytes
                || _images.Count <= 1 /* special case: never evict the only image */)
            {
                return;
            }
        }
    }
}
