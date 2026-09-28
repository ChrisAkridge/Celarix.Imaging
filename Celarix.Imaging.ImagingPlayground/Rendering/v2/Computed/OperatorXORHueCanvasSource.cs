using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using DrawingSize = System.Drawing.Size;

namespace Celarix.Imaging.ImagingPlayground.Rendering.v2.Computed
{
    public sealed class OperatorXORHueCanvasSource : PixelIndependentCanvasSource
    {
        private static readonly Rgba32[] hueWheel = new Rgba32[4096];

        public override string Name => "Operators: Bitwise XOR (12-bit hue wheel)";

        public override DrawingSize Level0TileCount => new DrawingSize(4, 4);

        public OperatorXORHueCanvasSource()
        {
            for (var i = 0; i < 4096; i++)
            {
                hueWheel[i] = NumberToHue(i);
            }
        }

        public override Rgba32 GetPixelValue(int x, int y)
        {
            var number = (x ^ y) & 0xFFF; // Keep only the lowest 12 bits
            return hueWheel[number];
        }

        private static Rgba32 NumberToHue(int number)
        {
            if (number < 0 || number > 4095)
            {
                throw new ArgumentOutOfRangeException(nameof(number));
            }

            var hue = number * 360f / 4096f;
            var hsv = new SixLabors.ImageSharp.ColorSpaces.Hsv(hue, 1f, 1f);
            return ColorSpaceConverter.ToRgb(hsv);
        }
    }
}
