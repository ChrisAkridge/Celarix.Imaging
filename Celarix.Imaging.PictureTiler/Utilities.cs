using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.PictureTiler
{
	internal static class Utilities
	{
        // WYLO: so we need to determine if a file is an image
        // by looking at the file, not just the extension
        // or maybe make it another post-processing step for FileAnalysis, i dunno
        public static bool IsFileAnImage(string filePath) =>
            Imaging.Utilities.ImageIdentifier.IsValidImageFile(filePath);

        public static IEnumerable<Image<Rgba32>> ImageEnumerable(IList<string> imageFilePaths) =>
            imageFilePaths.Select(Image.Load<Rgba32>);
    }
}
