using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.Utilities
{
    public sealed class ImageLoadResult
    {
        public string FilePath { get; }
        public Image<Rgba32>? LoadedImage { get; }
        public Size? Size { get; }
        public ImageMetadata? Metadata { get; }
        public ImageLoadAttemptResult Result { get; }
        public Exception? Exception { get; }

        public ImageLoadResult(string filePath,
            Image<Rgba32>? loadedImage,
            ImageLoadAttemptResult result,
            Exception? exception = null)
        {
            FilePath = filePath;
            LoadedImage = loadedImage;
            Size = loadedImage?.Size;
            Metadata = loadedImage?.Metadata;
            Exception = exception;
            Result = result;
        }
    }
}
