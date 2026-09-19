using System;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.BinaryDrawing.V2
{
    public static class PixelConverter
    {
        private static readonly Rgba32 GuardNormal = new Rgba32(160, 160, 160, 255); // #A0A0A0
        private static readonly Rgba32 GuardSubnormal = new Rgba32(48, 48, 48, 255); // #303030
        private static readonly Rgba32 GuardInfinity = new Rgba32(0, 255, 0, 255); // #00FF00
        private static readonly Rgba32 GuardNaN = new Rgba32(255, 0, 0, 255); // #FF0000

        public static void ConvertToPixels(ReadOnlySpan<byte> bytes, Span<Rgba32> pixels, PixelFormat format, Palette palette)
        {
            switch (format)
            {
                case PixelFormat.Bpp1:
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        byte b = bytes[i];
                        int px = i * 8;
                        pixels[px++] = palette.Colors[(b >> 7) & 1];
                        pixels[px++] = palette.Colors[(b >> 6) & 1];
                        pixels[px++] = palette.Colors[(b >> 5) & 1];
                        pixels[px++] = palette.Colors[(b >> 4) & 1];
                        pixels[px++] = palette.Colors[(b >> 3) & 1];
                        pixels[px++] = palette.Colors[(b >> 2) & 1];
                        pixels[px++] = palette.Colors[(b >> 1) & 1];
                        pixels[px]   = palette.Colors[b & 1];
                    }
                    break;
                case PixelFormat.Bpp2:
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        byte b = bytes[i];
                        int px = i * 4;
                        pixels[px++] = palette.Colors[(b >> 6) & 3];
                        pixels[px++] = palette.Colors[(b >> 4) & 3];
                        pixels[px++] = palette.Colors[(b >> 2) & 3];
                        pixels[px]   = palette.Colors[b & 3];
                    }
                    break;
                case PixelFormat.Bpp4:
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        byte b = bytes[i];
                        int px = i * 2;
                        pixels[px++] = palette.Colors[(b >> 4) & 15];
                        pixels[px]   = palette.Colors[b & 15];
                    }
                    break;
                case PixelFormat.Bpp8:
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        pixels[i] = palette.Colors[bytes[i]];
                    }
                    break;
                case PixelFormat.Bpp16:
                    for (int i = 0; i < bytes.Length / 2; i++)
                    {
                        int bx = i * 2;
                        ushort val = (ushort)((bytes[bx] << 8) | bytes[bx + 1]);
                        pixels[i] = palette.Colors[val];
                    }
                    if (bytes.Length % 2 != 0)
                    {
                        // Handle left over byte padded with 0
                        ushort val = (ushort)(bytes[^1] << 8);
                        pixels[bytes.Length / 2] = palette.Colors[val];
                    }
                    break;
                case PixelFormat.Bpp24:
                    for (int i = 0; i < bytes.Length / 3; i++)
                    {
                        int bx = i * 3;
                        pixels[i] = new Rgba32(bytes[bx], bytes[bx + 1], bytes[bx + 2], 255);
                    }
                    int left24 = bytes.Length % 3;
                    if (left24 > 0)
                    {
                        int bx = bytes.Length - left24;
                        byte r = bytes[bx];
                        byte g = left24 > 1 ? bytes[bx + 1] : (byte)0;
                        pixels[bytes.Length / 3] = new Rgba32(r, g, 0, 255);
                    }
                    break;
                case PixelFormat.Bpp32:
                    for (int i = 0; i < bytes.Length / 4; i++)
                    {
                        int bx = i * 4;
                        pixels[i] = new Rgba32(bytes[bx], bytes[bx + 1], bytes[bx + 2], bytes[bx + 3]);
                    }
                    int left32 = bytes.Length % 4;
                    if (left32 > 0)
                    {
                        int bx = bytes.Length - left32;
                        byte r = bytes[bx];
                        byte g = left32 > 1 ? bytes[bx + 1] : (byte)0;
                        byte b = left32 > 2 ? bytes[bx + 2] : (byte)0;
                        pixels[bytes.Length / 4] = new Rgba32(r, g, b, 255);
                    }
                    break;
                case PixelFormat.FloatSingle:
                    int singleCount = bytes.Length / 4;
                    for (int i = 0; i < singleCount; i++)
                    {
                        int bx = i * 4;
                        uint val = (uint)((bytes[bx] << 24) | (bytes[bx + 1] << 16) | (bytes[bx + 2] << 8) | bytes[bx + 3]);
                        ConvertSingleToPixels(val, pixels.Slice(i * 5, 5));
                    }
                    int leftSingle = bytes.Length % 4;
                    if (leftSingle > 0)
                    {
                        int bx = bytes.Length - leftSingle;
                        uint val = (uint)(bytes[bx] << 24);
                        if (leftSingle > 1) val |= (uint)(bytes[bx + 1] << 16);
                        if (leftSingle > 2) val |= (uint)(bytes[bx + 2] << 8);
                        ConvertSingleToPixels(val, pixels.Slice(singleCount * 5, 5));
                    }
                    break;
                case PixelFormat.FloatDouble:
                    int doubleCount = bytes.Length / 8;
                    for (int i = 0; i < doubleCount; i++)
                    {
                        int bx = i * 8;
                        ulong val = ((ulong)bytes[bx] << 56) |
                                    ((ulong)bytes[bx + 1] << 48) |
                                    ((ulong)bytes[bx + 2] << 40) |
                                    ((ulong)bytes[bx + 3] << 32) |
                                    ((ulong)bytes[bx + 4] << 24) |
                                    ((ulong)bytes[bx + 5] << 16) |
                                    ((ulong)bytes[bx + 6] << 8) |
                                    bytes[bx + 7];
                        ConvertDoubleToPixels(val, pixels.Slice(i * 7, 7));
                    }
                    int leftD = bytes.Length % 8;
                    if (leftD > 0)
                    {
                        int bx = bytes.Length - leftD;
                        ulong val = 0;
                        for (int k = 0; k < leftD; k++)
                        {
                            val |= (ulong)bytes[bx + k] << (56 - k * 8);
                        }
                        ConvertDoubleToPixels(val, pixels.Slice(doubleCount * 7, 7));
                    }
                    break;
            }
        }

        private static void ConvertSingleToPixels(uint singleBytes, Span<Rgba32> result)
        {
            uint exp = (singleBytes >> 23) & 0xFF;
            uint mantissa = singleBytes & 0x7FFFFF;

            Rgba32 guardColor;
            uint impliedBit = 0;

            if (exp == 0)
            {
                guardColor = GuardSubnormal;
            }
            else if (exp == 255)
            {
                guardColor = mantissa == 0 ? GuardInfinity : GuardNaN;
            }
            else
            {
                guardColor = GuardNormal;
                impliedBit = 1;
            }

            uint fullMantissa = (impliedBit << 23) | mantissa;

            result[0] = guardColor;                     // Guard
            result[1] = new Rgba32(                     // Mantissa 24-bit RGB 8:8:8
                (byte)((fullMantissa >> 16) & 0xFF),
                (byte)((fullMantissa >> 8) & 0xFF),
                (byte)(fullMantissa & 0xFF),
                255);
            result[2] = guardColor;                     // Guard
            result[3] = new Rgba32(                     // Exponent 8-bit RGB 3:3:2
                Palette.GenerateRampValue((int)((exp >> 5) & 0x07), 3),
                Palette.GenerateRampValue((int)((exp >> 2) & 0x07), 3),
                Palette.GenerateRampValue((int)(exp & 0x03), 2),
                255);
            result[4] = guardColor;                     // Guard
        }

        private static void ConvertDoubleToPixels(ulong doubleBytes, Span<Rgba32> result)
        {
            uint exp = (uint)((doubleBytes >> 52) & 0x7FF);
            ulong mantissa = doubleBytes & 0xFFFFFFFFFFFFF;

            Rgba32 guardColor;
            ulong impliedBit = 0;

            if (exp == 0)
            {
                guardColor = GuardSubnormal;
            }
            else if (exp == 2047)
            {
                guardColor = mantissa == 0 ? GuardInfinity : GuardNaN;
            }
            else
            {
                guardColor = GuardNormal;
                impliedBit = 1;
            }

            ulong fullMantissa = (impliedBit << 52) | mantissa; // 53 bits

            uint mHigh = (uint)(fullMantissa >> 29); // Top 24 bits
            uint mMed = (uint)((fullMantissa >> 5) & 0xFFFFFF); // Next 24 bits
            uint mLow = (uint)(fullMantissa & 0x1F); // Bottom 5 bits

            result[0] = guardColor;                     // Guard
            result[1] = new Rgba32(                     // High Mantissa 24-bit RGB 8:8:8
                (byte)((mHigh >> 16) & 0xFF),
                (byte)((mHigh >> 8) & 0xFF),
                (byte)(mHigh & 0xFF),
                255);
            result[2] = new Rgba32(                     // Med Mantissa 24-bit RGB 8:8:8
                (byte)((mMed >> 16) & 0xFF),
                (byte)((mMed >> 8) & 0xFF),
                (byte)(mMed & 0xFF),
                255);
            result[3] = new Rgba32(                     // Low Mantissa 5-bit RGB 2:2:1
                Palette.GenerateRampValue((int)((mLow >> 3) & 0x03), 2),
                Palette.GenerateRampValue((int)((mLow >> 1) & 0x03), 2),
                Palette.GenerateRampValue((int)(mLow & 0x01), 1),
                255);
            result[4] = guardColor;                     // Guard
            result[5] = new Rgba32(                     // Exponent 11-bit RGB 4:5:2
                Palette.GenerateRampValue((int)((exp >> 7) & 0x0F), 4),
                Palette.GenerateRampValue((int)((exp >> 2) & 0x1F), 5),
                Palette.GenerateRampValue((int)(exp & 0x03), 2),
                255);
            result[6] = guardColor;                     // Guard
        }
    }
}
