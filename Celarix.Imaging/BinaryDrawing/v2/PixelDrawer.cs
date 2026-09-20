using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    internal static class PixelDrawer
    {
        public static void FillPixelBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            switch (options.PixelFormat)
            {
                case PixelFormat.Binary1Bpp:
                    Fill1BppBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void Fill1BppBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            int bytesRead,
            out int pixelsWritten)
        {
            Rgba32[] palette;
            if (options.ColorMode == ColorMode.UserPalette)
            {
                if (options.UserPalette == null || options.UserPalette.Length < 2)
                {
                    throw new ArgumentException("User palette must contain at least 2 colors for 1bpp images.");
                }
                palette = options.UserPalette;
            }
            else
            {
                palette = [.. DefaultPalettes.GetPalette(1, Imaging.ColorMode.Grayscale)];
            }

            pixelsWritten = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                var currentByte = byteBuffer[i];
                for (var bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    var pixelValue = (currentByte >> (7 - bitIndex)) & 1;
                    var pixelIndex = (i * 8) + bitIndex;
                    if (pixelIndex >= pixelBuffer.Length)
                    {
                        break;
                    }
                    pixelBuffer[pixelIndex] = palette[pixelValue];
                    pixelsWritten += 1;
                }
            }
        }
    }
}
