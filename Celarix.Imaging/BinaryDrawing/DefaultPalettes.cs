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
        private static Rgba32[] oneBppGrayscale;
        private static Rgba32[] twoBppGrayscale;
        private static Rgba32[] fourBppGrayscale;
        private static Rgba32[] fourBppRgb121;
        private static Rgba32[] eightBppGrayscale;
        private static Rgba32[] eightBppRgb332;
        private static Rgba32[] eightBppArgb2222;
        private static Rgba32[] sixteenBppRgb565;
        private static Rgba32[] sixteenBppArgb4444;

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
                default: throw new ArgumentException("No default palette for this color mode.");
            }

            throw new InvalidOperationException("Unreachable code.");
        }

        /// <summary>
        /// Generates the color entries for each palette.
        /// </summary>
        private static void CreatePalettes()
        {
            byte[] twoBitRange = GenerateRange(4);
            byte[] threeBitRange = GenerateRange(8);
            byte[] fourBitRange = GenerateRange(16);
            byte[] fiveBitRange = GenerateRange(32);
            byte[] sixBitRange = GenerateRange(64);

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
                byte red = (byte)(0xFF * ((i & 0x08) >> 3)); // 0x08 == 1000_2
                byte green = twoBitRange[(i & 0x06) >> 1]; // 0x06 == 0110_2
                byte blue = (byte)(0xFF * (i & 0x01));
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
                byte red = threeBitRange[(i & 0xE0) >> 5]; // 0xE0 == 11100000_2
                byte green = threeBitRange[(i & 0x1C) >> 2]; // 0x1C = 00011100_2
                byte blue = threeBitRange[i & 0x03]; // 0x03 == 00000011_2
                eightBppRgb332[i] = new Rgba32(red, green, blue, 255);
            }

            // 8bpp ARGB
            for (var i = 0; i < 256; i++)
            {
                byte alpha = twoBitRange[(i & 0xC0) >> 6]; // 0xC0 == 11000000_2
                byte red = twoBitRange[(i & 0x30) >> 4]; // 0x30 == 00110000_2
                byte green = twoBitRange[(i & 0x0C) >> 2]; // 0x0C == 00001100_2
                byte blue = twoBitRange[i & 0x03]; // 0x03 == 00000011_2
                eightBppArgb2222[i] = new Rgba32(red, green, blue, alpha);
            }

            // 16bpp RGB
            for (var i = 0; i < 65536; i++)
            {
                byte red = fiveBitRange[(i & 0xF800) >> 11]; // 0xF800 == 11111000 00000000_2
                byte green = sixBitRange[(i & 0x07E0) >> 5]; // 0x07E0 == 00000111 11100000_2
                byte blue = fiveBitRange[i & 0x001F]; // 0x001F == 00000000 00011111_2
                sixteenBppRgb565[i] = new Rgba32(red, green, blue, 255);
            }

            // 16bpp ARGB
            for (var i = 0; i < 65536; i++)
            {
                byte alpha = fourBitRange[(i & 0xF000) >> 12]; // 0xF000 == 11110000 00000000_2
                byte red = fourBitRange[(i & 0x0F00) >> 8]; // 0x0F000 == 00001111 00000000_2
                byte green = fourBitRange[(i & 0x00F0) >> 4]; // 0x00F0 == 00000000 11110000_2
                byte blue = fourBitRange[i & 0x000F]; // 0x000F == 00000000 00001111_2
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
