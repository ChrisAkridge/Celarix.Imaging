using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering
{
    internal sealed class ImageCache
    {
        private const long EvictInvisibleImagesBytesUsedThreshold = 512 * 1024 * 1024; // 512 MB

        private readonly Dictionary<CanvasImage, ImageEntry> _cache = new Dictionary<CanvasImage, ImageEntry>();
        private readonly InfiniteCanvasControl _control;
        private long _lastEvictionTick = 0;

        private long TotalBytesLoaded => _cache.Values.Sum(entry => entry.ByteSize);

        public long MaxMemoryBytes { get; set; } = 4L * 1024L * 1024L * 1024L; // Default to 4 GB

        public ImageCache(InfiniteCanvasControl control)
        {
            _control = control;
        }

        public void Add(CanvasImage image)
        {
            Debug.WriteLine("ImageCache: Adding image at position " + image.Position);
            var entry = new ImageEntry(image);
            _cache.Add(image, entry);
        }

        public bool Remove(CanvasImage image)
        {
            Debug.WriteLine("ImageCache: Removing image at position " + image.Position);
            if (_cache.TryGetValue(image, out var entry))
            {
                entry.Evict();
                return _cache.Remove(image);
            }
            return false;
        }

        public void Clear()
        {
            Debug.WriteLine("ImageCache: Clearing all images");
            foreach (var entry in _cache.Values)
            {
                entry.Evict();
            }
            _cache.Clear();
        }

        public void UpdateVisibility(Viewport viewport)
        {
            Debug.WriteLine($"ImageCache: Updating visibility for viewport at {viewport.Location} with size {viewport.Size} and zoom level {viewport.ZoomLevel}");
            var usedBytes = TotalBytesLoaded;

            // Start with all images as candidates for visibility
            var wanted = new HashSet<CanvasImage>(_cache.Keys);

            // Remove any which have a zoom level set AND that level is different from the viewport's zoom level
            wanted.RemoveWhere(image => image.OnlyAtZoomLevel.HasValue && image.OnlyAtZoomLevel.Value != viewport.ZoomLevel);

            // Remove any which don't intersect with the viewport
            wanted.RemoveWhere(image => !viewport.Intersects(image));
            var unwanted = _cache.Keys.Except(wanted);

            // Get the KVPs for each candidate
            var wantedEntries = wanted.Select(c => new KeyValuePair<CanvasImage, ImageEntry>(c, _cache[c]));
            var unwantedEntries = unwanted.Select(c => new KeyValuePair<CanvasImage, ImageEntry>(c, _cache[c]));

            foreach (var wantedEntry in wantedEntries)
            {
                var imageEntry = wantedEntry.Value;
                usedBytes += imageEntry.EstimateMemoryUsageOnLoad();
                if (usedBytes > MaxMemoryBytes)
                {
                    Debug.WriteLine($"ImageCache: Memory budget exceeded when trying to load image at position {imageEntry.Position}. Used bytes: {usedBytes}, max bytes: {MaxMemoryBytes}");
                    break;
                }

                if (imageEntry.LoadState == ImageEntryLoadState.Unloaded)
                {
                    imageEntry.StartLoad(_control);
                }
            }

            foreach (var unwantedEntry in unwantedEntries)
            {
                var imageEntry = unwantedEntry.Value;
                if (imageEntry.LoadState == ImageEntryLoadState.Loading)
                {
                    imageEntry.Cancel();
                }
                else if (imageEntry.LoadState == ImageEntryLoadState.Loaded)
                {
                    imageEntry.IsEvictable = true;
                }
            }

            var now = Environment.TickCount64;
            if (now - _lastEvictionTick > 1000)
            {
                Evict();
            }
        }

        public SKImage? GetImage(CanvasImage image)
        {
            if (_cache.TryGetValue(image, out var entry) && entry.LoadState == ImageEntryLoadState.Loaded)
            {
                entry.LastUsedTick = Environment.TickCount64;
                return entry.SKImage;
            }
            return null;
        }

        public List<ImageEntry> Cull(Viewport viewport)
        {
            var visibleImages = new List<ImageEntry>();
            foreach (var kvp in _cache)
            {
                var image = kvp.Key;
                var entry = kvp.Value;
                if (viewport.Intersects(image) && entry.LoadState == ImageEntryLoadState.Loaded)
                {
                    entry.LastUsedTick = Environment.TickCount64;
                    visibleImages.Add(entry);
                }
            }
            Debug.WriteLine($"ImageCache: Culled to {visibleImages.Count} visible images");
            return visibleImages;
        }

        private void Evict()
        {
            Debug.WriteLine("ImageCache: Evicting images to stay within memory budget");
            _lastEvictionTick = Environment.TickCount64;
            var evictableEntries = _cache.Where(kvp => kvp.Value.IsEvictable).ToList();
            var totalBytesLoaded = TotalBytesLoaded;
            if (evictableEntries.Count <= 1 || totalBytesLoaded < EvictInvisibleImagesBytesUsedThreshold)
            {
                // Special case: if there's only one loaded image, never evict it.
                return;
            }

            // Evict least recently used first
            while (totalBytesLoaded > EvictInvisibleImagesBytesUsedThreshold && evictableEntries.Count > 0)
            {
                Debug.WriteLine($"ImageCache: Total bytes loaded {totalBytesLoaded}, evicting least recently used image");
                var leastRecentlyUsed = evictableEntries.OrderBy(kvp => kvp.Value.LastUsedTick).First();
                totalBytesLoaded -= leastRecentlyUsed.Value.ByteSize;
                leastRecentlyUsed.Value.Evict();
                evictableEntries.Remove(leastRecentlyUsed);
            }

            // Behold. The one time where this line of code actually makes sense.
            GC.Collect();
        }
    }
}
