using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging
{
    public enum ColorMode
    {
        Grayscale,
        Rgb,
        Argb,
        Paletted
    }

    public enum CCIFCompressionMode : byte
    {
        NoCompression,
        ZlibCompression
    }

    public enum CCIFColorMode : byte
    {
        Reserved,
        OneBitBW,
        FourBitPaletted,
        EightBitPaletted,
        SevenBitPalettedWithAlpha,
        SixteenBitPaletted,
        FifteenBitPalettedWithAlpha,
        TwentyFourBitRGB,
        ThirtyTwoBitRGBA
    }

    internal enum ImageAlphaLevel
    {
        NoAlpha,
        OneBitAlpha,
        EightBitAlpha
    }
}