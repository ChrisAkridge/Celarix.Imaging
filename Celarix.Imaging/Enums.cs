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

    /// <summary>
    /// Defines how pixels decoded from a binary stream are laid out onto an image.
    /// </summary>
    public enum DrawingMode
    {
        /// <summary>
        /// Pixels are written left-to-right, top-to-bottom (standard raster scan order).
        /// </summary>
        Raster,

        /// <summary>
        /// Pixels are written in vertical stripes. The stripe width is determined by the bit depth
        /// (1 bpp → 8px, 2 bpp → 4px, 4 bpp → 2px, ≥8 bpp → 1px). Each stripe is filled
        /// top-to-bottom before advancing one stripe-width to the right.
        /// </summary>
        Striped
    }
}