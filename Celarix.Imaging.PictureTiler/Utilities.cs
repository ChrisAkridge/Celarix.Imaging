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
        public static bool IsFileAnImage(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension == ".jpg" || extension == ".png";
		}

        public static IEnumerable<Image<Rgba32>> ImageEnumerable(IList<string> imageFilePaths) =>
            imageFilePaths.Select(Image.Load<Rgba32>);
    }
}
