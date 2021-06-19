using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.Formats
{
    public static class CCIF
    {
        public static IReadOnlyList<byte> ImageToCCIF(Image<Rgba32> image, CCIFCompressionMode compressionMode)
        {
            // === Image Analysis ===
            //  1. Look at the alpha values of each pixel.
            //      a. If we find one that has alpha from 1 to 254, then we know this image is ImageAlphaLevel.EightBitAlpha.
            //      b. Otherwise, keep looking. If all pixels have alpha of either 0 or 255, then we know this image is ImageAlphaLevel.OneBitAlpha.
            //      c. Otherwise, we know this image is ImageAlphaLevel.NoAlpha.
            //  2. Count the distinct colors in the image, ignoring alpha.
            //  3. If the color count is less than 65,536, create a palette that is ordered by 
            //  4. Choose the color mode.
            //      a. If the palette contains only black, white, or both, color mode is 1bpp.
            //      b. If the alpha level is NoAlpha:
            //          i. Select either 4bpp, 8bpp, or 16bpp for color counts of <= 16, <= 256, or <= 65536, respectively.
            //          ii. Select 24bpp for > 65536 colors.
            //      c. If the alpha level is OneBitAlpha:
            //          i. Select either 7+1bpp or 15+1bpp for color counts of <= 128 or <= 32768, respectively.
            //          ii. Select 32bpp for > 32768 colors.
            //      d. If the alpha level is EightBitAlpha, select 32bpp.
            // === Write the Header ===
            //  1. Allocate a 14-byte ExpandableMemoryStream for the header.
            //  2. Write "CLXI" in big endian as the magic number.
            //  3. Write image.Width, then image.Height.
            //  4. Write the color mode and compression mode.
            // === Write the Palette ===
            //     Write a 0x00 byte for 1bpp, 24bpp, and 32bpp. Otherwise,
            //  1. Write an Int32 for the color count of the palette. Each entry is 24 bits.
            //  2. Write the colors of the palette from first index to last.
            // === Get the Uncompressed Image Data ===
            //  1. Determine the width*height in completed chunks, then the width and height of the partial chunks.
            //  2. Begin filling in the complete chunks, left to right, top to bottom:
            //      a. Write an 0xFF byte to start the complete chunk.
            //      b. Write the pixels using separate methods that write in each color mode, along with the palette.
            //  3. Continue with the partial chunks, if any, first along the right edge (including the bottom-right corner), then along the bottom.
            //      a. Write the width of the chunk between 0 and 7 in the top three bits of the chunk header.
            //      b. Write the height of the chunk between 0 and 7 in the next three bits of the chunk header.
            //      c. Write the pixels, left-to-right, top to bottom.
            // === Compress the Data ===
            //  1. Run the data through ZLib in another ExpandableMemoryStream, if required.

            return null;
        }

        private static IEnumerable<Rgb24> GetDistinctColorsInCountOrder(Image<Rgba32> image)
        {
            // Go through the image, counting each instance of the color and how often it appears.
            // Store the distinct colors in the list passed in, with [0] being the most common color, and so forth...
            
            var colors = new Dictionary<Rgb24, int>(65536);
            
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Rgba32 colorWithAlpha = image[x, y];
                    var color = new Rgb24(colorWithAlpha.R, colorWithAlpha.G, colorWithAlpha.B);

                    if (colors.ContainsKey(color)) { colors[color] += 1; }
                    else { colors[color] = 1; }
                }
            }

            return colors.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key);
        }
    }
}
