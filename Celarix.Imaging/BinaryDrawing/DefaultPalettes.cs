using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.BinaryDrawing
{
	public static class DefaultPalettes
    {
        private static readonly Rgba32[] oneBppGrayscale;
        private static readonly Rgba32[] twoBppGrayscale;
        private static readonly Rgba32[] fourBppGrayscale;
        private static readonly Rgba32[] fourBppRgb121;
        private static readonly Rgba32[] eightBppGrayscale;
        private static readonly Rgba32[] eightBppRgb332;
        private static readonly Rgba32[] eightBppArgb2222;
        private static readonly Rgba32[] sixteenBppRgb565;
        private static readonly Rgba32[] sixteenBppArgb4444;

        /// <summary>
        /// Contains the default 1 bit per pixel grayscale palette.
        /// </summary>
        public static IReadOnlyList<Rgba32> OneBppGrayscale => Array.AsReadOnly(oneBppGrayscale);

        /// <summary>
        /// Contains the default 2 bits per pixel grayscale palette.
        /// </summary>
        public static IReadOnlyList<Rgba32> TwoBppGrayscale => Array.AsReadOnly(twoBppGrayscale);

        /// <summary>
        /// Contains the default 4 bits per pixel grayscale palette.
        /// </summary>
        public static IReadOnlyList<Rgba32> FourBppGrayscale => Array.AsReadOnly(fourBppGrayscale);

        /// <summary>
        /// Contains the default 4 bits per pixel color palette, mapped in RGB 1:2:1.
        /// </summary>
        public static IReadOnlyList<Rgba32> FourBppRgb121 => Array.AsReadOnly(fourBppRgb121);

        /// <summary>
        /// Contains the default 8 bits per pixel grayscale palette.
        /// </summary>
        public static IReadOnlyList<Rgba32> EightBppGrayscale => Array.AsReadOnly(eightBppGrayscale);

        /// <summary>
        /// Contains the default 8 bits per pixel color palette, mapped in RGB 3:3:2.
        /// </summary>
        public static IReadOnlyList<Rgba32> EightBppRgb332 => Array.AsReadOnly(eightBppRgb332);

        /// <summary>
        /// Contains the default 8 bits per pixel color palette, mapped in RGBA 2:2:2:2.
        /// </summary>
        public static IReadOnlyList<Rgba32> EightBppArgb2222 => Array.AsReadOnly(eightBppArgb2222);

        /// <summary>
        /// Contains the default 16 bits per pixel color palette, mapped in RGB 5:6:5.
        /// </summary>
        public static IReadOnlyList<Rgba32> SixteenBppRgb565 => Array.AsReadOnly(sixteenBppRgb565);

        /// <summary>
        /// Contains the default 16 bits per pixel color palette, mapped in RGBA 4:4:4:4.
        /// </summary>
        public static IReadOnlyList<Rgba32> SixteenBppArgb4444 => Array.AsReadOnly(sixteenBppArgb4444);

        /// <summary>
        /// Initializes the static members of the <see cref="DefaultPalettes" /> class.
        /// </summary>
        static DefaultPalettes()
        {
            oneBppGrayscale = new Rgba32[2];
            twoBppGrayscale = new Rgba32[4];
            fourBppGrayscale = new Rgba32[16];
            fourBppRgb121 = new Rgba32[16];
            eightBppGrayscale = new Rgba32[256];
            eightBppRgb332 = new Rgba32[256];
            eightBppArgb2222 = new Rgba32[256];
            sixteenBppRgb565 = new Rgba32[65536];
            sixteenBppArgb4444 = new Rgba32[65536];

            CreatePalettes();
        }

        /// <summary>
        /// Returns the default palette for a given bit depth and color mode.
        /// </summary>
        /// <param name="bitDepth">The given bit depth.</param>
        /// <param name="mode">The given color mode.</param>
        /// <returns>The requested default palette.</returns>
        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public static IReadOnlyList<Rgba32> GetPalette(int bitDepth, ColorMode mode)
        {
            switch (mode)
            {
                case ColorMode.Grayscale:
                    switch (bitDepth)
                    {
                        case 1: return OneBppGrayscale;
                        case 2: return TwoBppGrayscale;
                        case 4: return FourBppGrayscale;
                        case 8: return EightBppGrayscale;
                    }
                    break;
                case ColorMode.Rgb:
                    switch (bitDepth)
                    {
                        case 4: return FourBppRgb121;
                        case 8: return EightBppRgb332;
                        case 16: return SixteenBppRgb565;
                    }
                    break;
                case ColorMode.Argb:
                    switch (bitDepth)
                    {
                        case 8: return EightBppArgb2222;
                        case 16: return SixteenBppArgb4444;
                    }
                    break;
                default: throw new ArgumentException($"No default palette for this color mode ({bitDepth}-bit {mode}).");
            }

            throw new InvalidOperationException("Unreachable code.");
        }

        /// <summary>
        /// Generates the color entries for each palette.
        /// </summary>
        private static void CreatePalettes()
        {
            var twoBitRange = GenerateRange(4);
            var threeBitRange = GenerateRange(8);
            var fourBitRange = GenerateRange(16);
            var fiveBitRange = GenerateRange(32);
            var sixBitRange = GenerateRange(64);

            // 1bpp grayscale
            oneBppGrayscale[0] = Color.Black;
            oneBppGrayscale[1] = Color.White;

            // 2bpp grayscale
            for (var i = 0; i < 4; i++)
            {
                twoBppGrayscale[i] = new Rgba32(twoBitRange[i], twoBitRange[i], twoBitRange[i], 255);
            }

            // 4bpp grayscale
            for (var i = 0; i < 16; i++)
            {
                fourBppGrayscale[i] = new Rgba32(fourBitRange[i], fourBitRange[i], fourBitRange[i], 255);
            }

            // 4bpp RGB
            for (var i = 0; i < 16; i++)
            {
                byte red = (byte)(0xFF * ((i & 0b1000) >> 3));
                byte green = twoBitRange[(i & 0b0110) >> 1];
                byte blue = (byte)(0xFF * (i & 0b0001));
                fourBppRgb121[i] = new Rgba32(red, green, blue, 255);
            }

            // 8bpp grayscale
            for (var i = 0; i < 256; i++)
            {
                byte gray = (byte)i;
                eightBppGrayscale[i] = new Rgba32(gray, gray, gray, 255);
            }

            // 8bpp RGB
            for (var i = 0; i < 256; i++)
            {
                byte red = threeBitRange[(i & 0b11100000) >> 5];
                byte green = threeBitRange[(i & 0b00011100) >> 2];
                byte blue = threeBitRange[i & 0b00000011];
                eightBppRgb332[i] = new Rgba32(red, green, blue, 255);
            }

            // 8bpp ARGB
            for (var i = 0; i < 256; i++)
            {
                byte alpha = twoBitRange[(i & 0b11000000) >> 6];
                byte red = twoBitRange[(i & 0b00110000) >> 4];
                byte green = twoBitRange[(i & 0b00001100) >> 2];
                byte blue = twoBitRange[i & 0b00000011];
                eightBppArgb2222[i] = new Rgba32(red, green, blue, alpha);
            }

            // 16bpp RGB
            for (var i = 0; i < 65536; i++)
            {
                byte red = fiveBitRange[(i & 0b11111000_00000000) >> 11];
                byte green = sixBitRange[(i & 0b00000111_11100000) >> 5];
                byte blue = fiveBitRange[i & 0b00000000_00011111];
                sixteenBppRgb565[i] = new Rgba32(red, green, blue, 255);
            }

            // 16bpp ARGB
            for (var i = 0; i < 65536; i++)
            {
                byte alpha = fourBitRange[(i & 0b11110000_00000000) >> 12]; // 0xF000 == 11110000 00000000_2
                byte red = fourBitRange[(i & 0b00001111_00000000) >> 8]; // 0x0F000 == 00001111 00000000_2
                byte green = fourBitRange[(i & 0b00000000_11110000) >> 4]; // 0x00F0 == 00000000 11110000_2
                byte blue = fourBitRange[i & 0b00000000_00001111]; // 0x000F == 00000000 00001111_2
                sixteenBppArgb4444[i] = new Rgba32(red, green, blue, alpha);
            }
        }

        /// <summary>
        /// Generates a certain number of values along the range of 0 to 255, inclusive.
        /// </summary>
        /// <param name="divisor">The number of values in the range.</param>
        /// <returns>A byte array containing the values along the range.</returns>
        private static byte[] GenerateRange(int divisor)
        {
            var result = new byte[divisor];
            for (int i = 0; i < divisor; i++)
            {
                result[i] = (byte)(255f * (i / (float)(divisor - 1)));
            }
            return result;
        }
    }
}
