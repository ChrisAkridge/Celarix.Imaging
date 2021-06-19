// Enums.cs
//
// Contains enumerations for ByteView.

namespace Celarix.Imaging.ByteView
{
    /// <summary>
    /// An enumeration of the supported bit depths.
    /// </summary>
    public enum BitDepth
    {
        /// <summary>
        /// A placeholder value.
        /// </summary>
        Invalid,

        /// <summary>
        /// A bit depth which uses one bit per pixel.
        /// </summary>
        OneBpp,

        /// <summary>
        /// A bit depth which uses two bits per pixel.
        /// </summary>
        TwoBpp,

        /// <summary>
        /// A bit depth which uses four bits per pixel.
        /// </summary>
        FourBpp,

        /// <summary>
        /// A bit depth which uses eight bits per pixel.
        /// </summary>
        EightBpp,

        /// <summary>
        /// A bit depth which uses sixteen bits per pixel.
        /// </summary>
        SixteenBpp,

        /// <summary>
        /// A bit depth which uses twenty-four bits per pixel.
        /// </summary>
        TwentyFourBpp,

        /// <summary>
        /// A bit depth which uses thirty-two bits per pixel.
        /// </summary>
        ThirtyTwoBpp,
    }

    /// <summary>
    /// An enumeration of the supported color modes.
    /// </summary>
    public enum ColorMode
    {
        /// <summary>
        /// A placeholder value.
        /// </summary>
        Invalid,

        /// <summary>
        /// A color mode containing black, shades of grey, and white.
        /// </summary>
        Grayscale,

        /// <summary>
        /// A color mode that maps bits of a pixel to red, green, and blue channels.
        /// </summary>
        RGB,

        /// <summary>
        /// A color mode that maps bits of a pixel to alpha (transparency), red,
        /// green, and blue channels.
        /// </summary>
        ARGB,

        /// <summary>
        /// A color mode that maps a number to a predefined ARGB color.
        /// </summary>
        Paletted
    }
}
