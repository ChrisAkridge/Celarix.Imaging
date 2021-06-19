using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.IO
{
    public sealed class ImageCache : IDisposable
    {
        private readonly int cacheSize;
        private readonly Dictionary<string, Image<Rgba32>> cache = new Dictionary<string, Image<Rgba32>>();

        public ImageCache(int cacheSize)
        {
            this.cacheSize = cacheSize;
        }

        public Image<Rgba32> Load(string path)
        {
            if (cache.ContainsKey(path))
            {
                return cache[path];
            }

            var image = Image.Load<Rgba32>(path);

            if (cache.Count == cacheSize)
            {
                var imageToEvict = cache.Keys.First();
                cache[imageToEvict].Dispose();
                cache.Remove(imageToEvict);
            }
            
            cache.Add(path, image);

            return image;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            foreach (var cacheItem in cache)
            {
                cacheItem.Value.Dispose();
            }
        }
    }
}
