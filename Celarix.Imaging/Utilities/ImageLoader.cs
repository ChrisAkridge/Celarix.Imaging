using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.Utilities
{
    /// <summary>
    /// A utility class for robustly loading images from files, streams, or byte arrays, with support for various formats and error handling.
    /// </summary>
    public static class ImageLoader
    {

        public static ImageLoadResult LoadImage(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)
                || !Uri.IsWellFormedUriString(filePath, UriKind.Absolute))
            {
                return new ImageLoadResult(filePath, null, ImageLoadAttemptResult.InvalidFilePath);
            }
            else if (!File.Exists(filePath))
            {
                return new ImageLoadResult(filePath, null, ImageLoadAttemptResult.FileNotFound);
            }
            else
            {
                try
                {
                    var image = Image.Load<Rgba32>(filePath);
                    return new ImageLoadResult(filePath, image, ImageLoadAttemptResult.Success);
                }
                catch (OutOfMemoryException oomex)
                {
                    // This exception is thrown when the file is not a valid image format or is corrupted.
                    return new ImageLoadResult(filePath, null, ImageLoadAttemptResult.InvalidFile, oomex);
                }
                catch (Exception ex)
                {
                    // Catch any other exceptions that may occur during loading.
                    return new ImageLoadResult(filePath, null, ImageLoadAttemptResult.UnknownError, ex);
                }
            }
        }
    }
}
