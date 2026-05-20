using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging
{
    public static class Extensions
    {
        internal static ColorMode ToV1ColorMode(this BinaryDrawing.v2.ColorMode v2Mode)
        {
            return v2Mode switch
            {
                BinaryDrawing.v2.ColorMode.Grayscale => ColorMode.Grayscale,
                BinaryDrawing.v2.ColorMode.RGB => ColorMode.Rgb,
                BinaryDrawing.v2.ColorMode.RGBA => ColorMode.Argb,
                BinaryDrawing.v2.ColorMode.UserPalette => ColorMode.Paletted,
                _ => throw new ArgumentOutOfRangeException(nameof(v2Mode))
            };
        }
    }
}
