using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Rendering
{
    internal sealed class ImageCache
    {
        private const long MemoryBudget = 512 * 1024 * 1024; // 512 MB

        private readonly Dictionary<CanvasImage, ImageEntry> _cache = new Dictionary<CanvasImage, ImageEntry>();
        private readonly InfiniteCanvasControl _control;
        private long _lastEvictionTick = 0;

        private long TotalBytesLoaded => _cache.Values.Sum(entry => entry.ByteSize);

        public ImageCache(InfiniteCanvasControl control)
        {
            _control = control;
        }

        public void Add(CanvasImage image)
        {
            var entry = new ImageEntry(image);
            _cache.Add(image, entry);
        }

        public bool Remove(CanvasImage image)
        {
            if (_cache.TryGetValue(image, out var entry))
            {
                entry.Evict();
                return _cache.Remove(image);
            }
            return false;
        }

        public void Clear()
        {
            foreach (var entry in _cache.Values)
            {
                entry.Evict();
            }
            _cache.Clear();
        }

        public void UpdateVisibility(Viewport viewport)
        {
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
            return visibleImages;
        }

        private void Evict()
        {
            _lastEvictionTick = Environment.TickCount64;
            var evictableEntries = _cache.Where(kvp => kvp.Value.IsEvictable).ToList();
            var totalBytesLoaded = TotalBytesLoaded;
            if (evictableEntries.Count <= 1 || totalBytesLoaded < MemoryBudget)
            {
                // Special case: if there's only one loaded image, never evict it.
                return;
            }

            // Evict least recently used first
            while (totalBytesLoaded > MemoryBudget && evictableEntries.Count > 0)
            {
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
