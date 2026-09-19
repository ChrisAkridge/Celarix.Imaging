using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;

namespace Celarix.Imaging.BinaryDrawing
{
    /// <summary>
    /// Describes how raw bytes from a stream are decoded into pixels,
    /// encoding both the bit depth and channel layout in a single value.
    /// Use <see cref="DefaultPalettes.GetBitDepthAndPalette"/> to resolve
    /// a value to the underlying <c>bitDepth</c> and palette that
    /// <see cref="Drawer"/> methods accept.
    /// </summary>
    public enum PixelConversion
    {
        /// <summary>
        /// 1 bit per pixel. Each byte yields 8 pixels, MSB first.
        /// 0 → black (#000000FF), 1 → white (#FFFFFFFF).
        /// </summary>
        To1BppBlackWhite,

        /// <summary>
        /// 2 bits per pixel. Each byte yields 4 pixels, MSB first.
        /// Maps to evenly-spaced grayscale shades (black, ⅓, ⅔, white).
        /// </summary>
        To2BppGrayscale,

        /// <summary>
        /// 4 bits per pixel. Each byte yields 2 pixels, MSB nybble first.
        /// Maps to 16 evenly-spaced grayscale shades (black → white).
        /// </summary>
        To4BppGrayscale,

        /// <summary>
        /// 4 bits per pixel. Each byte yields 2 pixels, MSB nybble first.
        /// Bit layout: <c>RGGB</c>. R and B are 1-bit (black or full); G is 2-bit (thirds).
        /// </summary>
        To4BppRGB121,

        /// <summary>
        /// 4 bits per pixel. Each byte yields 2 pixels, MSB nybble first.
        /// Bit layout: <c>RGBA</c>. All channels are 1-bit (0x00 or 0xFF).
        /// </summary>
        To4BppRGBA1111,

        /// <summary>
        /// 8 bits per pixel. Each byte is one pixel.
        /// Maps to 256 evenly-spaced grayscale shades (identity: index == gray level).
        /// </summary>
        To8BppGrayscale,

        /// <summary>
        /// 8 bits per pixel. Each byte is one pixel.
        /// Bit layout: <c>RRRGGGBB</c>. R and G are 3-bit (sevenths); B is 2-bit (thirds).
        /// </summary>
        To8BppRGB332,

        /// <summary>
        /// 8 bits per pixel. Each byte is one pixel.
        /// Bit layout: <c>RRGGBBAA</c>. All channels are 2-bit (thirds).
        /// </summary>
        To8BppRGBA2222,

        /// <summary>
        /// 16 bits per pixel. Every 2 bytes yield 1 pixel.
        /// Bit layout: <c>RRRRRGGG GGGBBBBB</c>. R and B are 5-bit; G is 6-bit.
        /// </summary>
        To16BppRGB565,

        /// <summary>
        /// 16 bits per pixel. Every 2 bytes yield 1 pixel.
        /// Bit layout: <c>RRRRGGGG BBBBAAAA</c>. All channels are 4-bit (fifteenths).
        /// </summary>
        To16BppRGBA4444,

        /// <summary>
        /// 24 bits per pixel. Every 3 bytes yield 1 pixel.
        /// Bit layout: <c>RRRRRRRR GGGGGGGG BBBBBBBB</c>. Full 8-bit channels; standard RGB888.
        /// </summary>
        To24BppRGB888,

        /// <summary>
        /// 24 bits per pixel. Every 3 bytes yield 1 pixel.
        /// Bit layout: <c>RRRRRRGG GGGGBBBB BBAAAAAA</c>. All channels are 6-bit (sixty-thirds).
        /// </summary>
        /// <remarks>
        /// This format requires new decoding logic in <c>SetPixelOnImage</c> and is
        /// currently treated as a placeholder (decoded identically to
        /// <see cref="To24BppRGB888"/>). Full support will be added in a future refactor.
        /// </remarks>
        To24BppRGBA6666,

        /// <summary>
        /// 32 bits per pixel. Every 4 bytes yield 1 pixel.
        /// Bit layout: <c>RRRRRRRR GGGGGGGG BBBBBBBB AAAAAAAA</c>. Full 8-bit channels; standard RGBA8888.
        /// </summary>
        To32BppRGBA8888,
    }
}
