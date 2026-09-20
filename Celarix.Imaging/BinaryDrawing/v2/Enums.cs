using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    public enum PixelFormat
    {
        Binary1Bpp,
        Binary2Bpp,
        Binary3Bpp,
        Binary4Bpp,
        Binary8Bpp,
        Binary16Bpp,
        Binary24Bpp,
        Binary32Bpp,
        Float16,
        Float32,
        Float64,
        TextMap
    }

    public enum ColorMode
    {
        Grayscale,
        RGB,
        RGBA,
        UserPalette
    }

    public enum PixelLayout
    {
        Raster,
        Striped
    }

    public enum SizeMode
    {
        Automatic,
        FixedWidth,
        FixedSize
    }

    public enum TitleMode
    {
        None,
        OnePerImage,
        OnePerFile
    }

    public enum TargetMode
    {
        SingleImage,
        ZoomableCanvas,
        MultipleToFolder
    }

    public enum BandDirection
    {
        /// <summary>
        /// The next image is below the previous one, so the images are arranged in horizontal bands.
        /// </summary>
        Horizontal,

        /// <summary>
        /// The next image is to the right of the previous one, so the images are arranged in vertical bands.
        /// </summary>
        Vertical
    }

    public enum TextMapEncoding
    {
        None,
        ASCII,
        UTF8,
        UTF16LE,
        UTF16BE,
        UTF32LE,
        UTF32BE
    }

    public enum DrawResultKind
    {
        Failure,
        SingleImageInMemory,
        StripedImageInTempFolder,
        ZoomableCanvas,
        MultipleToFolder
    }

    public enum DrawProgressPhase
    {
        NotStarted,
        DrawingPixels,
        // TODO: figure out exactly how we're going to draw to decimal
        BuildingTextMap,
        Completed
    }

    internal enum SingleImageSizingResult
    {
        Success,
        MissingFixedSizes,
        NotWideEnoughForOneStripe
    }

    internal enum SingleImageSizingType
    {
        PixelBlock,
        TitleBar,
        FixedSize
    }
}
