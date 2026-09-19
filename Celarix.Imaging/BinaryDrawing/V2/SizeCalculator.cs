using System;
using SixLabors.ImageSharp;

namespace Celarix.Imaging.BinaryDrawing.V2
{
    public static class SizeCalculator
    {
        public static long GetPixelCountForFormat(long byteCount, PixelFormat format)
        {
            return format switch
            {
                PixelFormat.Bpp1 => byteCount * 8,
                PixelFormat.Bpp2 => byteCount * 4,
                PixelFormat.Bpp4 => byteCount * 2,
                PixelFormat.Bpp8 => byteCount,
                PixelFormat.Bpp16 => (long)Math.Ceiling(byteCount / 2d),
                PixelFormat.Bpp24 => (long)Math.Ceiling(byteCount / 3d),
                PixelFormat.Bpp32 => (long)Math.Ceiling(byteCount / 4d),
                PixelFormat.FloatSingle => (long)Math.Ceiling(byteCount / 4d) * 5,
                PixelFormat.FloatDouble => (long)Math.Ceiling(byteCount / 8d) * 7,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        public static int GetStripeWidth(PixelFormat format)
        {
            return format switch
            {
                PixelFormat.Bpp1 => 8,
                PixelFormat.Bpp2 => 4,
                PixelFormat.Bpp4 => 4, // Outline says: "4 BPP... Stripe width 4." Wait, outline updated to "Stripe width 4"? Let me check what the user actually said. The user said: "4 BPP (2px/byte): ... Stripe width 4." But then 8 BPP Stripe width 2, 16 BPP Stripe width 2, 24 BPP Stripe width 1, 32 BPP 1, Single 5, Double 7. Let me double check user diff. Yes.
                PixelFormat.Bpp8 => 2,
                PixelFormat.Bpp16 => 2,
                PixelFormat.Bpp24 => 1,
                PixelFormat.Bpp32 => 1,
                PixelFormat.FloatSingle => 5,
                PixelFormat.FloatDouble => 7,
                _ => 1
            };
        }

        public static Size CalculateDimensions(long totalPixels, BinaryDrawingOptions options)
        {
            if (options.FixedSize.HasValue && options.OutputType != OutputType.FixedSizeFolder)
            {
                return options.FixedSize.Value;
            }

            if (options.FixedWidth.HasValue && options.OutputType != OutputType.FixedSizeFolder)
            {
                int width = options.FixedWidth.Value;
                if (options.PixelOrder == PixelOrder.Stripe)
                {
                    int stripeWidth = GetStripeWidth(options.PixelFormat);
                    if (width % stripeWidth != 0)
                    {
                        width += stripeWidth - (width % stripeWidth);
                    }
                }
                long height = (long)Math.Ceiling((double)totalPixels / width);
                return new Size(width, (int)height); // assuming height fits in int for simplicity, but large canvases might need bigger
            }

            // Standard for Layout
            if (options.PixelOrder == PixelOrder.Raster)
            {
                int width = (int)Math.Floor(Math.Sqrt(totalPixels));
                int height = width;
                while ((long)width * height < totalPixels)
                {
                    height++;
                }
                return new Size(width, height);
            }
            else // Stripe
            {
                int stripeWidth = GetStripeWidth(options.PixelFormat);
                long virtualHeight = (long)Math.Ceiling((double)totalPixels / stripeWidth);
                int colHeight = (int)Math.Floor(Math.Sqrt(virtualHeight));
                if (colHeight == 0) colHeight = 1;
                
                int columns = (int)Math.Ceiling((double)virtualHeight / colHeight);
                int width = columns * stripeWidth;
                return new Size(width, colHeight);
            }
        }
    }
}
