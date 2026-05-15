using Celarix.Imaging.Utilities;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    internal static class SingleImageSizer
    {
        private const int TitleBarHeight = 24;

        public static SingleImageSize GetSizeIfValid(DrawOptions options)
        {
            if (options.SizeMode == SizeMode.FixedSize)
            {
                if (!options.FixedWidth.HasValue
                    || options.FixedWidth.Value <= 0
                    || !options.FixedHeight.HasValue
                    || options.FixedHeight.Value <= 0)
                {
                    // TODO: add logging for this case
                    return new SingleImageSize(SingleImageSizingResult.MissingFixedSizes, []);
                }

                if (options.PixelLayout == PixelLayout.Striped
                    && options.FixedWidth.Value < Helpers.GetStripeWidth(options.PixelFormat))
                {
                    // TODO: add logging for this case
                    return new SingleImageSize(SingleImageSizingResult.NotWideEnoughForOneStripe, []);
                }

                // We don't care where title bars end up in fixed size mode, so just make them all one block.
                return new SingleImageSize(SingleImageSizingResult.Success,
                    [new SingleImageBlock(SingleImageSizingType.FixedSize,
                        new Rectangle(0, 0, options.FixedWidth!.Value, options!.FixedHeight.Value))]);
            }

            var pixelBlockCount = options.TitleMode is TitleMode.None or TitleMode.OnePerImage
                ? 1
                : options.ByteStream.TotalFiles;
            var pixelBlockPixels = new List<long>();
            if (pixelBlockCount == 1)
            {
                pixelBlockPixels.Add(CountPixelsForSize(options.PixelFormat, options.ByteStream.TotalLength));
            }
            else
            {
                for (var i = 0; i < pixelBlockCount; i++)
                {
                    var blockSize = options.ByteStream.GetFileLength(i);
                    pixelBlockPixels.Add(CountPixelsForSize(options.PixelFormat, blockSize));
                }
            }

            List<Size> pixelBlockSizes = SizePixelBlocks(options, pixelBlockPixels, null);
            var maxWidth = pixelBlockSizes.Max(s => s.Width);

            if (options.TitleMode == TitleMode.OnePerFile
                && options.SizeMode == SizeMode.Automatic)
            {
                pixelBlockSizes = SizePixelBlocks(options, pixelBlockPixels, maxWidth);
            }

            var blocks = new List<SingleImageBlock>();
            if (options.TitleMode is TitleMode.OnePerImage)
            {
                blocks.Add(new SingleImageBlock(SingleImageSizingType.TitleBar, new Rectangle(0, 0, maxWidth, TitleBarHeight)));
                blocks.Add(new SingleImageBlock(SingleImageSizingType.PixelBlock, new Rectangle(0, TitleBarHeight, maxWidth, pixelBlockSizes[0].Height)));
                return new SingleImageSize(SingleImageSizingResult.Success, blocks);
            }

            var currentY = 0;
            for (int i = 0; i < pixelBlockSizes.Count; i++)
            {
                if (options.TitleMode != TitleMode.None)
                {
                    blocks.Add(new SingleImageBlock(SingleImageSizingType.TitleBar, new Rectangle(0, currentY, maxWidth, TitleBarHeight)));
                    currentY += TitleBarHeight;
                }
                Size pixelBlockSize = pixelBlockSizes[i];
                blocks.Add(new SingleImageBlock(SingleImageSizingType.PixelBlock, new Rectangle(0, currentY, maxWidth, pixelBlockSize.Height)));
                currentY += pixelBlockSize.Height;
            }
            return new SingleImageSize(SingleImageSizingResult.Success, blocks);
        }

        private static List<Size> SizePixelBlocks(DrawOptions options, List<long> pixelBlockPixels, int? fixedWidthOverride)
        {
            var pixelBlockSizes = new List<Size>();
            foreach (var pixelBlockPixelCount in pixelBlockPixels)
            {
                Size blockSize;
                if (options.SizeMode == SizeMode.FixedWidth || fixedWidthOverride.HasValue)
                {
                    var width = fixedWidthOverride ?? options.FixedWidth!.Value;
                    blockSize = GetBlockSizeFixedWidth(pixelBlockPixelCount,
                        width,
                        options.PixelFormat,
                        options.PixelLayout);
                }
                else
                {
                    blockSize = GetBlockSizeAutomatic(pixelBlockPixelCount, options.PixelFormat, options.PixelLayout);
                }

                pixelBlockSizes.Add(blockSize);
            }

            return pixelBlockSizes;
        }

        private static long CountPixelsForSize(PixelFormat format, long bytes)
        {
            var bits = bytes * 8L;

            if (format == PixelFormat.Binary1Bpp)
            {
                return bytes * 8L;
            }
            else if (format == PixelFormat.Binary2Bpp)
            {
                return bytes * 4L;
            }
            else if (format == PixelFormat.Binary3Bpp)
            {
                var roundedBits = Helpers.RoundUpToMultiple(bits, 3L);
                return roundedBits / 3L;
            }
            else if (format == PixelFormat.Binary4Bpp)
            {
                return bytes * 2L;
            }
            else if (format == PixelFormat.Binary8Bpp)
            {
                return bytes;
            }
            else if (format == PixelFormat.Binary16Bpp)
            {
                return Helpers.RoundUpToMultiple(bytes, 2L) / 2L;
            }
            else if (format == PixelFormat.Binary24Bpp)
            {
                return Helpers.RoundUpToMultiple(bytes, 3L) / 3L;
            }
            else if (format == PixelFormat.Binary32Bpp)
            {
                return Helpers.RoundUpToMultiple(bytes, 4L) / 4L;
            }
            else if (format == PixelFormat.Float16)
            {
                return (Helpers.RoundUpToMultiple(bytes, 2L) / 2L) * 6L;
            }
            else if (format == PixelFormat.Float32)
            {
                return (Helpers.RoundUpToMultiple(bytes, 4L) / 4L) * 6L;
            }
            else if (format == PixelFormat.Float64)
            {
                return (Helpers.RoundUpToMultiple(bytes, 8L) / 8L) * 6L;
            }
            else
            {
                throw new ArgumentException($"Unsupported pixel format: {format}");
            }
        }

        private static Size GetBlockSizeAutomatic(long pixelCount,
            PixelFormat format,
            PixelLayout layout)
        {
            if (layout == PixelLayout.Raster)
            {
                var width = (int)Math.Ceiling(Math.Sqrt(pixelCount));
                var height = (int)Math.Ceiling((double)pixelCount / width);
                return new Size(width, height);
            }
            else
            {
                var stripeCount = GetStripeCount(pixelCount, format);
                var height = (int)Math.Ceiling(Math.Sqrt(stripeCount));
                var widthInStripes = (int)Math.Ceiling((double)stripeCount / height);
                var stripeWidth = Helpers.GetStripeWidth(format);
                var width = widthInStripes * stripeWidth;
                return new Size(width, height);
            }
        }

        private static Size GetBlockSizeFixedWidth(long pixelCount,
            int fixedWidth,
            PixelFormat format,
            PixelLayout layout)
        {
            if (layout == PixelLayout.Raster)
            {
                var height = (int)Math.Ceiling((double)pixelCount / fixedWidth);
                return new Size(fixedWidth, height);
            }
            else
            {
                var stripeWidth = Helpers.GetStripeWidth(format);
                var stripesPerRow = fixedWidth / stripeWidth;
                var stripeCount = GetStripeCount(pixelCount, format);
                var height = (int)Math.Ceiling((double)stripeCount / stripesPerRow);
                return new Size(fixedWidth, height);
            }
        }

        private static long GetStripeCount(long pixelCount, PixelFormat format)
        {
            int stripeWidth = Helpers.GetStripeWidth(format);
            return Helpers.RoundUpToMultiple(pixelCount, stripeWidth) / stripeWidth;
        }
    }
}
