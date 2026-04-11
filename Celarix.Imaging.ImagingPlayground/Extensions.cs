using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground
{
    internal static class Extensions
    {
        public static long EstimateImageMemoryUsage(this Image<Rgba32> image)
        {
            // Each pixel is 4 bytes (R, G, B, A)
            return (long)image.Width * image.Height * 4;
        }
    }
}
