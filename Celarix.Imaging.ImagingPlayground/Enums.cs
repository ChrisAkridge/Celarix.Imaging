using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground
{
    internal enum PixelFormat
    {
        Binary1Bpp,
        Binary2Bpp,
        Binary3Bpp,
        Binary4Bpp,
        Binary8Bpp,
        Binary16Bpp,
        Binary24Bpp,
        Binary32Bpp,
        Decimal1DigitPerPixel,
        Decimal2DigitPerPixel,
        Decimal4DigitPerPixel,
        Decimal8DigitPerPixel,
        Float16,
        Float32,
        Float64,
        TextMap
    }

    internal enum ColorMode
    {
        Grayscale,
        RGB,
        RGBA,
        UserPalette
    }

    internal enum PixelLayout
    {
        Raster,
        Striped
    }

    internal enum SizeMode
    {
        Automatic,
        FixedWidth,
        FixedSize
    }

    internal enum TitleMode
    {
        None,
        OnePerImage,
        OnePerFile
    }

    internal enum TargetMode
    {
        SingleImage,
        ZoomableCanvas,
        MultipleToFolder
    }
}
