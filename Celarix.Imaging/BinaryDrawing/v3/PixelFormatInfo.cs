using Celarix.Imaging.BinaryDrawing.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3
{
    internal sealed class PixelFormatInfo
    {
        public PixelFormat Format { get; }
        public IReadOnlyList<ColorMode> SupportedColorModes { get; }
        public int SourceBytesPerGroup { get; }
        public int OutputPixelsPerGroup { get; }
        public int StripeWidth { get; }
        public int PaletteRequiredCount { get; }

        private PixelFormatInfo(PixelFormat format,
            IReadOnlyList<ColorMode> supportedColorModes,
            int sourceBytesPerGroup,
            int outputPixelsPerGroup,
            int stripeWidth,
            int paletteRequiredCount)
        {
            Format = format;
            SupportedColorModes = supportedColorModes;
            SourceBytesPerGroup = sourceBytesPerGroup;
            OutputPixelsPerGroup = outputPixelsPerGroup;
            StripeWidth = stripeWidth;
            PaletteRequiredCount = paletteRequiredCount;
        }

        public static readonly PixelFormatInfo OneBpp = new(PixelFormat.Binary1Bpp, [ColorMode.Paletted, ColorMode.Grayscale], 1, 8, 8, 2);
        public static readonly PixelFormatInfo TwoBpp = new(PixelFormat.Binary2Bpp, [ColorMode.Paletted, ColorMode.Grayscale], 1, 4, 4, 4);
        public static readonly PixelFormatInfo ThreeBpp = new(PixelFormat.Binary3Bpp, [ColorMode.Paletted, ColorMode.Grayscale, ColorMode.Rgb], 3, 8, 3, 8);
        public static readonly PixelFormatInfo FourBpp = new(PixelFormat.Binary4Bpp, [ColorMode.Paletted, ColorMode.Grayscale, ColorMode.Rgb, ColorMode.Rgba], 1, 2, 2, 16);
        public static readonly PixelFormatInfo EightBpp = new(PixelFormat.Binary8Bpp, [ColorMode.Paletted, ColorMode.Grayscale, ColorMode.Rgb, ColorMode.Rgba], 1, 1, 1, 256);
        public static readonly PixelFormatInfo SixteenBpp = new(PixelFormat.Binary16Bpp, [ColorMode.Paletted, ColorMode.Rgb, ColorMode.Rgba], 2, 1, 1, 65536);
        public static readonly PixelFormatInfo TwentyFourBpp = new(PixelFormat.Binary24Bpp, [ColorMode.Rgb, ColorMode.Rgba], 3, 1, 1, 0);
        public static readonly PixelFormatInfo ThirtyTwoBpp = new(PixelFormat.Binary32Bpp, [ColorMode.Rgb, ColorMode.Rgba], 4, 1, 1, 0);
        public static readonly PixelFormatInfo Float16 = new(PixelFormat.Float16, [ColorMode.Floating], 2, 7, 7, 0);
        public static readonly PixelFormatInfo Float32 = new(PixelFormat.Float32, [ColorMode.Floating], 4, 7, 7, 0);
        public static readonly PixelFormatInfo Float64 = new(PixelFormat.Float64, [ColorMode.Floating], 8, 9, 9, 0);


        public static PixelFormatInfo GetFormat(PixelFormat format)
        {
            return format switch
            {
                PixelFormat.Binary1Bpp => OneBpp,
                PixelFormat.Binary2Bpp => TwoBpp,
                PixelFormat.Binary3Bpp => ThreeBpp,
                PixelFormat.Binary4Bpp => FourBpp,
                PixelFormat.Binary8Bpp => EightBpp,
                PixelFormat.Binary16Bpp => SixteenBpp,
                PixelFormat.Binary24Bpp => TwentyFourBpp,
                PixelFormat.Binary32Bpp => ThirtyTwoBpp,
                _ => throw new NotImplementedException()
            };
        }
    }
}
