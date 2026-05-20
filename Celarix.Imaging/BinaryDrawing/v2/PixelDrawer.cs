using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    internal static class PixelDrawer
    {
        public static void FillPixelBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            switch (options.PixelFormat)
            {
                case PixelFormat.Binary1Bpp:
                    Fill1BppBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Binary2Bpp:
                    Fill2BppBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Binary3Bpp:
                    Fill3BppBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Binary4Bpp:
                    Fill4BppBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Binary8Bpp:
                    Fill8BppBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Binary16Bpp:
                    Fill16BppBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Binary24Bpp:
                    Fill24BppBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Binary32Bpp:
                    Fill32BppBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Float16:
                    FillFloat16Buffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Float32:
                    FillFloat32Buffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                case PixelFormat.Float64:
                    FillFloat64Buffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void Fill1BppBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            var palette = GetPaletteOrThrow(options, 1, 2, ColorMode.Grayscale);

            pixelsWritten = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                var currentByte = byteBuffer[i];
                for (var bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    var pixelValue = (currentByte >> (7 - bitIndex)) & 1;
                    var pixelIndex = (i * 8) + bitIndex;
                    if (pixelIndex >= pixelBuffer.Length)
                    {
                        break;
                    }
                    pixelBuffer[pixelIndex] = palette[pixelValue];
                    pixelsWritten += 1;
                }
            }
        }

        private static void Fill2BppBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            var palette = GetPaletteOrThrow(options, 2, 4, ColorMode.Grayscale);
            pixelsWritten = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                var currentByte = byteBuffer[i];
                for (var bitPairIndex = 0; bitPairIndex < 4; bitPairIndex++)
                {
                    var pixelValue = (currentByte >> (6 - (bitPairIndex * 2))) & 0b11;
                    var pixelIndex = (i * 4) + bitPairIndex;
                    if (pixelIndex >= pixelBuffer.Length)
                    {
                        break;
                    }
                    pixelBuffer[pixelIndex] = palette[pixelValue];
                    pixelsWritten += 1;
                }
            }
        }

        private static void Fill3BppBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            var palette = GetPaletteOrThrow(options, 3, 8, options.ColorMode);
            pixelsWritten = 0;
            var pixelIndex = 0;

            for (var i = 0; i < bytesRead; i += 3)
            {
                var hi = byteBuffer[i];
                var mid = (i + 1) < bytesRead ? byteBuffer[i + 1] : (byte)0;
                var lo = (i + 2) < bytesRead ? byteBuffer[i + 2] : (byte)0;

                // 00011122 23334445 55666777
                // 76543210 76543210 76543210
                var _0 = hi >> 5;
                var _1 = (hi >> 2) & 0b111;
                var _2 = (hi & 0b11) << 1 | (mid >> 7);
                var _3 = (mid >> 4) & 0b111;
                var _4 = (mid >> 1) & 0b111;
                var _5 = (mid & 0b1) << 2 | (lo >> 6);
                var _6 = (lo >> 3) & 0b111;
                var _7 = lo & 0b111;

                var pixelsToWrite = Math.Min(8, pixelBuffer.Length - pixelIndex);

                for (var p = 0; p < pixelsToWrite; p++)
                {
                    var pixelValue = p switch
                    {
                        0 => _0,
                        1 => _1,
                        2 => _2,
                        3 => _3,
                        4 => _4,
                        5 => _5,
                        6 => _6,
                        7 => _7,
                        _ => throw new InvalidOperationException()
                    };
                    pixelBuffer[pixelIndex++] = palette[pixelValue];
                    pixelsWritten++;
                }

                if (pixelIndex >= pixelBuffer.Length) break;
            }
        }

        private static void Fill4BppBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            var palette = GetPaletteOrThrow(options, 4, 16, options.ColorMode);
            pixelsWritten = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                var currentByte = byteBuffer[i];
                var hiNibble = currentByte >> 4;
                var loNibble = currentByte & 0b1111;
                var firstPixelIndex = i * 2;
                if (firstPixelIndex >= pixelBuffer.Length)
                {
                    break;
                }
                pixelBuffer[firstPixelIndex] = palette[hiNibble];
                pixelsWritten += 1;
                var secondPixelIndex = firstPixelIndex + 1;
                if (secondPixelIndex >= pixelBuffer.Length)
                {
                    break;
                }
                pixelBuffer[secondPixelIndex] = palette[loNibble];
                pixelsWritten += 1;
            }
        }

        private static void Fill8BppBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            var palette = GetPaletteOrThrow(options, 8, 256, options.ColorMode);
            pixelsWritten = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                var currentByte = byteBuffer[i];
                var pixelIndex = i;
                if (pixelIndex >= pixelBuffer.Length)
                {
                    break;
                }
                pixelBuffer[pixelIndex] = palette[currentByte];
                pixelsWritten += 1;
            }
        }

        private static void Fill16BppBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            var palette = GetPaletteOrThrow(options, 16, 65536, options.ColorMode);
            pixelsWritten = 0;
            for (var i = 0; i < bytesRead; i += 2)
            {
                var hi = byteBuffer[i];
                var lo = (i + 1) < bytesRead ? byteBuffer[i + 1] : (byte)0;
                var pixelIndex = i / 2;
                if (pixelIndex >= pixelBuffer.Length)
                {
                    break;
                }
                pixelBuffer[pixelIndex] = palette[(hi << 8) | lo];
                pixelsWritten += 1;
            }
        }

        private static void Fill24BppBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            pixelsWritten = 0;
            for (var i = 0; i < bytesRead; i += 3)
            {
                var hi = byteBuffer[i];
                var mid = (i + 1) < bytesRead ? byteBuffer[i + 1] : (byte)0;
                var lo = (i + 2) < bytesRead ? byteBuffer[i + 2] : (byte)0;
                var pixelIndex = i / 3;
                if (pixelIndex >= pixelBuffer.Length)
                {
                    break;
                }
                pixelBuffer[pixelIndex] = TwentyFourBitsToPixel(hi, mid, lo, options.ColorMode);
                pixelsWritten += 1;
            }
        }

        private static void Fill32BppBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            pixelsWritten = 0;
            for (var i = 0; i < bytesRead; i += 4)
            {
                var red = byteBuffer[i];
                var green = (i + 1) < bytesRead ? byteBuffer[i + 1] : (byte)0;
                var blue = (i + 2) < bytesRead ? byteBuffer[i + 2] : (byte)0;
                var alpha = (i + 3) < bytesRead ? byteBuffer[i + 3] : (byte)0;
                var pixelIndex = i / 4;
                if (pixelIndex >= pixelBuffer.Length)
                {
                    break;
                }
                pixelBuffer[pixelIndex] = new Rgba32(red, green, blue, alpha);
                pixelsWritten += 1;
            }
        }

        private static void FillFloat16Buffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            pixelsWritten = 0;

            for (var i = 0; i < bytesRead; i += 2)
            {
                var hi = byteBuffer[i];
                var lo = (i + 1) < bytesRead ? byteBuffer[i + 1] : (byte)0;
                var halfBits = unchecked((ushort)((hi << 8) | lo));
                var sign = halfBits >> 15;
                var exponent = (halfBits >> 10) & 0b11111;
                var mantissa = halfBits & 0b11_11111111;

                Rgba32 guardColor;
                if (exponent == 0)
                {
                    // Subnormal
                    guardColor = new Rgba32(0x30, 0x30, 0x30, 0xFF);
                }
                else if (exponent == 31)
                {
                    if (mantissa == 0)
                    {
                        // Infinity
                        guardColor = new Rgba32(0, 0xFF, 0, 0xFF);
                    }
                    else
                    {
                        // NaN
                        guardColor = new Rgba32(0xFF, 0, 0, 0xFF);
                    }
                }
                else
                {
                    // Normal
                    guardColor = new Rgba32(0xA0, 0xA0, 0xA0, 0xFF);
                    mantissa |= (1 << 10);  // set implicit leading 1 bit
                }

                var firstPixelIndex = (i / 2) * 6;
                var lastPixelIndex = firstPixelIndex + 5;
                var distanceToEndOfBuffer = pixelBuffer.Length - lastPixelIndex;
                if (distanceToEndOfBuffer < 0)
                {
                    lastPixelIndex -= -distanceToEndOfBuffer;
                }
                for (var p = firstPixelIndex; p <= lastPixelIndex; p++)
                {
                    var pixel = (p - firstPixelIndex) switch
                    {
                        0 => sign == 0 ? new Rgba32(0xFF, 0xFF, 0xFF, 0xFF) : new Rgba32(0, 0, 0, 0xFF),
                        1 => guardColor,
                        2 => HalfMantissaPixel(mantissa),
                        3 => guardColor,
                        4 => HalfExponentPixel(exponent),
                        5 => guardColor,
                        _ => throw new InvalidOperationException()
                    };
                    pixelBuffer[p] = pixel;
                    pixelsWritten += 1;
                }
            }
        }

        private static void FillFloat32Buffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            pixelsWritten = 0;

            for (var i = 0; i < bytesRead; i += 4)
            {
                var _3 = byteBuffer[i];
                var _2 = (i + 1) < bytesRead ? byteBuffer[i + 1] : (byte)0;
                var _1 = (i + 2) < bytesRead ? byteBuffer[i + 2] : (byte)0;
                var _0 = (i + 3) < bytesRead ? byteBuffer[i + 3] : (byte)0;
                var singleBits = unchecked((int)((_3 << 24) | (_2 << 16) | (_1 << 8) | _0));
                var sign = singleBits >> 31;
                var exponent = (singleBits >> 23) & 0xFF;
                var mantissa = singleBits & 0x7FFFFF;

                Rgba32 guardColor;
                if (exponent == 0)
                {
                    // Subnormal
                    guardColor = new Rgba32(0x30, 0x30, 0x30, 0xFF);
                }
                else if (exponent == 255)
                {
                    if (mantissa == 0)
                    {
                        // Infinity
                        guardColor = new Rgba32(0, 0xFF, 0, 0xFF);
                    }
                    else
                    {
                        // NaN
                        guardColor = new Rgba32(0xFF, 0, 0, 0xFF);
                    }
                }
                else
                {
                    // Normal
                    guardColor = new Rgba32(0xA0, 0xA0, 0xA0, 0xFF);
                    mantissa |= (1 << 23);  // set implicit leading 1 bit
                }

                var firstPixelIndex = (i / 4) * 6;
                var lastPixelIndex = firstPixelIndex + 5;
                var distanceToEndOfBuffer = pixelBuffer.Length - lastPixelIndex;
                if (distanceToEndOfBuffer < 0)
                {
                    lastPixelIndex -= -distanceToEndOfBuffer;
                }
                for (var p = firstPixelIndex; p <= lastPixelIndex; p++)
                {
                    var pixel = (p - firstPixelIndex) switch
                    {
                        0 => sign == 0 ? new Rgba32(0xFF, 0xFF, 0xFF, 0xFF) : new Rgba32(0, 0, 0, 0xFF),
                        1 => guardColor,
                        2 => new Rgba32((byte)(mantissa >> 16), (byte)((mantissa >> 8) & 0xFF), (byte)(mantissa & 0xFF), 0xFF),
                        3 => guardColor,
                        4 => new Rgba32((byte)exponent, (byte)exponent, (byte)exponent, 0xFF),
                        5 => guardColor,
                        _ => throw new InvalidOperationException()
                    };
                    pixelBuffer[p] = pixel;
                    pixelsWritten += 1;
                }
            }
        }

        private static void FillFloat64Buffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            pixelsWritten = 0;

            for (var i = 0; i < bytesRead; i += 8)
            {
                var _7 = byteBuffer[i];
                var _6 = (i + 1) < bytesRead ? byteBuffer[i + 1] : (byte)0;
                var _5 = (i + 2) < bytesRead ? byteBuffer[i + 2] : (byte)0;
                var _4 = (i + 3) < bytesRead ? byteBuffer[i + 3] : (byte)0;
                var _3 = (i + 4) < bytesRead ? byteBuffer[i + 4] : (byte)0;
                var _2 = (i + 5) < bytesRead ? byteBuffer[i + 5] : (byte)0;
                var _1 = (i + 6) < bytesRead ? byteBuffer[i + 6] : (byte)0;
                var _0 = (i + 7) < bytesRead ? byteBuffer[i + 7] : (byte)0;
                var doubleBits = unchecked((ulong)(((ulong)_7 << 56)
                    | ((ulong)_6 << 48)
                    | ((ulong)_5 << 40)
                    | ((ulong)_4 << 32)
                    | ((ulong)_3 << 24)
                    | ((ulong)_2 << 16)
                    | ((ulong)_1 << 8)
                    | _0));
                var sign = doubleBits >> 63;
                var exponent = (doubleBits >> 52) & 0x7FF;
                var mantissa = doubleBits & 0xFFFFFFFFFFFFF;

                Rgba32 guardColor;
                if (exponent == 0)
                {
                    // Subnormal
                    guardColor = new Rgba32(0x30, 0x30, 0x30, 0xFF);
                }
                else if (exponent == 2047)
                {
                    if (mantissa == 0)
                    {
                        // Infinity
                        guardColor = new Rgba32(0, 0xFF, 0, 0xFF);
                    }
                    else
                    {
                        // NaN
                        guardColor = new Rgba32(0xFF, 0, 0, 0xFF);
                    }
                }
                else
                {
                    // Normal
                    guardColor = new Rgba32(0xA0, 0xA0, 0xA0, 0xFF);
                    mantissa |= (1UL << 52);  // set implicit leading 1 bit
                }

                var firstPixelIndex = (i / 8) * 8;
                var lastPixelIndex = firstPixelIndex + 7;
                var distanceToEndOfBuffer = pixelBuffer.Length - lastPixelIndex;
                if (distanceToEndOfBuffer < 0)
                {
                    lastPixelIndex -= -distanceToEndOfBuffer;
                }
                for (var p = firstPixelIndex; p <= lastPixelIndex; p++)
                {
                    var pixel = (p - firstPixelIndex) switch
                    {
                        0 => sign == 0 ? new Rgba32(0xFF, 0xFF, 0xFF, 0xFF) : new Rgba32(0, 0, 0, 0xFF),
                        1 => guardColor,
                        2 => DoubleHighMantissaPixel((int)(mantissa >> 48)),
                        3 => new Rgba32((uint)(((mantissa >> 24) & 0xFFFFFF) << 8) | 0xFF),
                        4 => new Rgba32((uint)((mantissa & 0xFFFFFF) << 8) | 0xFF),
                        5 => guardColor,
                        6 => DoubleExponentPixel((int)exponent),
                        7 => guardColor,
                        _ => throw new InvalidOperationException()
                    };
                    pixelBuffer[p] = pixel;
                    pixelsWritten += 1;
                }
            }
        }

        private static IReadOnlyList<Rgba32> GetPaletteOrThrow(DrawOptions options, int bitDepth, int requiredColors, ColorMode colorMode)
        {
            if (options.ColorMode == ColorMode.UserPalette)
            {
                if (options.UserPalette == null || options.UserPalette.Length < requiredColors)
                {
                    throw new ArgumentException($"User palette must contain at least {requiredColors} colors for {bitDepth}bpp images.");
                }
                return options.UserPalette;
            }
            else
            {
                return DefaultPalettes.GetPalette(bitDepth, colorMode.ToV1ColorMode());
            }
        }

        private static Rgba32 TwentyFourBitsToPixel(byte hi, byte mid, byte lo, ColorMode colorMode)
        {
            if (colorMode == ColorMode.RGB)
            {
                return new Rgba32(hi, mid, lo, 255);
            }
            else if (colorMode == ColorMode.RGBA)
            {
                // RRRRRRGG GGGGBBBB BBAAAAAA
                // 76543210 76543210 76543210
                var red = hi >> 2;
                var green = ((hi & 0b11) << 6) | (mid >> 4);
                var blue = ((mid & 0b1111) << 4) | (lo >> 6);
                var alpha = lo & 0b111111;

                var redScaled = (byte)(255f * (red / (float)63));
                var greenScaled = (byte)(255f * (green / (float)63));
                var blueScaled = (byte)(255f * (blue / (float)63));
                var alphaScaled = (byte)(255f * (alpha / (float)63));
                return new Rgba32(redScaled, greenScaled, blueScaled, alphaScaled);
            }
            else
            {
                throw new InvalidOperationException($"24-bit pixel data is not valid for color mode {colorMode}.");
            }
        }

        private static Rgba32 HalfMantissaPixel(int mantissa)
        {
            // 11 bit mantissa, RRR RGGGGBBB
            var red = mantissa >> 7;
            var green = (mantissa >> 3) & 0b1111;
            var blue = mantissa & 0b111;

            var redScaled = (byte)(255f * (red / (float)15));
            var greenScaled = (byte)(255f * (green / (float)15));
            var blueScaled = (byte)(255f * (blue / (float)7));
            return new Rgba32(redScaled, greenScaled, blueScaled, 0xFF);
        }

        private static Rgba32 HalfExponentPixel(int exponent)
        {
            // 5 bit exponent, GGGGG
            var scaled = (byte)(255f * (exponent / (float)31));
            return new Rgba32(scaled, scaled, scaled, 0xFF);
        }

        private static Rgba32 DoubleHighMantissaPixel(int highMantissa)
        {
            // Top 5 bits of the 53 bit mantissa, RRGGB
            var red = highMantissa >> 3;
            var green = (highMantissa >> 1) & 0b11;
            var blue = (highMantissa & 0b1) != 0 ? 0xFF : 0x00;

            var redScaled = (byte)(255f * (red / (float)3));
            var greenScaled = (byte)(255f * (green / (float)3));
            return new Rgba32(redScaled, greenScaled, blue, 0xFF);
        }

        private static Rgba32 DoubleExponentPixel(int exponent)
        {
            // 11 bit exponent, RRR RGGGGBBB
            var red = exponent >> 7;
            var green = (exponent >> 3) & 0b1111;
            var blue = exponent & 0b111;

            var redScaled = (byte)(255f * (red / (float)15));
            var greenScaled = (byte)(255f * (green / (float)15));
            var blueScaled = (byte)(255f * (blue / (float)7));
            return new Rgba32(redScaled, greenScaled, blueScaled, 0xFF);
        }
    }
}
