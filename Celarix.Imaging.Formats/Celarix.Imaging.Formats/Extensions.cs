using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.Formats
{
    internal static class Extensions
    {
        public static bool Equals(this Rgb24 a, Rgb24 b) => (a.R == b.R) && (a.G == b.G) && (a.B == b.B);
    }
}
