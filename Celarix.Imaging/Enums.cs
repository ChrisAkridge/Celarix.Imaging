using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging
{
    public enum ColorMode
    {
        Grayscale,
        Rgb,
        Rgba,
        Paletted,
        Floating
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
    
    public enum ColorChannel
    {
	    Red,
	    Green,
	    Blue,
	    Alpha,
	    Y,
	    Cb,
	    Cr,
	    CbPlusCr,
	    Hue,
	    Saturation,
	    Value
    }
    
    public enum ChromaSubsamplingMode
	{
		// Full-resolution chroma
	    YCbCr444,
	    // 2x1 block of chroma
	    YCbCr422,
	    // 2x2 block of chroma
	    YCbCr420,
	    // 4x1 block of chroma
	    YCbCr411,
	    // Non-standard, 8x8 block of chroma
	    YCbCr811,
	    // Non-standard, 16x16 block of chroma
	    YCbCr1611,
	    // VERY non-standard, 256x256 block of chroma
	    YCbCr25611
	}

	public enum SortMode
	{
		RGB,
		HSV,
		YCbCr
	}

    public enum NamedStreamReadResult
    {
        NotEndOfFile,
        EndOfFile,
        EndOfStream
    }

    public enum ImageLoadAttemptResult
    {
        Success,
        InvalidFilePath,
        FileNotFound,
        InvalidFile,
        UnknownError
    }

    internal enum FloatPixelFormat
    {
        Half,
        Single,
        Double
    }

    internal enum FloatKind
    {
        Normal,
        Subnormal,
        Infinity,
        NaN
    }

    /// <summary>
    /// Supported bit depths for the minimalist V4 Binary Drawing support. Values chosen for practicality
    /// over total coverage, as while implementing, for example, 3 bits per pixel is possible, it's not
    /// very useful except as a completionist exercise. Capped at 24 bits to ensure that no data loss
    /// is possible (i.e. in 32-bit RGBA mode, #33669900 and #22446600 both appear fully transparent
    /// and cannot be told apart.
    /// </summary>
    public enum BinaryDrawingBitDepths
    {
        OneBit = 1,
        FourBit = 4,
        EightBit = 8,
        SixteenBit = 16,
        TwentyFourBit = 24
    }
}