namespace Celarix.Imaging.Formats
{
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