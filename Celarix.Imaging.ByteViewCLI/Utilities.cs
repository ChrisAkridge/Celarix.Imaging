using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.ByteViewCLI
{
    internal static class Utilities
    {
        public static void SaveImage(string outputPath, Image<Rgba32> image)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            image.SaveAsPng(outputPath);
            var savedFileInfo = new FileInfo(outputPath);
            Console.WriteLine($"Image saved to {savedFileInfo.FullName} ({savedFileInfo.Length:#,###} bytes).");
        }
    }
}
