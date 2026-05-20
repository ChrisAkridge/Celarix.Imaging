using Celarix.Imaging.IO.v2;
using Celarix.Imaging.Utilities;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    public static class Drawer
    {
        private const int TitleBarHeight = 24;
        private const string TitleBarFontFamily = "Consolas";
        private const float TitleBarFontSize = 16f;

        public static async Task<DrawResult> DrawAsync(DrawOptions options)
        {
            try
            {
                if (options.TargetMode == TargetMode.SingleImage)
                {
                    return await DrawSingleImageAsync(options);
                }
                else if (options.TargetMode == TargetMode.ZoomableCanvas)
                {
                    return await DrawZoomableCanvasAsync(options);
                }
                else if (options.TargetMode == TargetMode.MultipleToFolder)
                {
                    return await DrawMultipleToFolderAsync(options);
                }
                else
                {
                    throw new ArgumentException($"Unsupported target mode: {options.TargetMode}");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return DrawResult.Failure(ex);
            }
        }

        // Single image
        private static async Task<DrawResult> DrawSingleImageAsync(DrawOptions options)
        {
            var imageSize = SingleImageSizer.GetSizeIfValid(options);
            if (imageSize.Result != SingleImageSizingResult.Success) { return DrawResult.Failure(); }

            var totalSize = imageSize.TotalSize;
            var majorDimension = options.PixelLayout == PixelLayout.Raster
                ? totalSize.Width
                : totalSize.Height;
            if (options.SizeMode == SizeMode.FixedSize || majorDimension < 4096)
            {
                return new DrawResult
                {
                    Kind = DrawResultKind.SingleImageInMemory,
                    SingleImage = options.PixelLayout switch
                    {
                        PixelLayout.Raster => await DrawSingleImageRasterInMemoryAsync(options, imageSize),
                        PixelLayout.Striped => await DrawSingleImageStripedInMemoryAsync(options, imageSize),
                        _ => throw new ArgumentException($"Unsupported pixel layout: {options.PixelLayout}")
                    }
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static async Task<Image<Rgba32>> DrawSingleImageRasterInMemoryAsync(DrawOptions options, SingleImageSize imageSize)
        {
            Size totalSize = imageSize.TotalSize;
            var image = new Image<Rgba32>(totalSize.Width, totalSize.Height, Color.Black);
            var byteBuffer = new byte[1024 * 3]; // 3KB buffer so that all pixel formats can read a whole number of pixels out of it
            var pixelBuffer = new Rgba32[GetPixelBufferSize(options.PixelFormat, byteBuffer.Length)];
            var x = 0;
            var y = 0;

            if (options.TitleMode != TitleMode.None)
            {
                // Draw the top title bar first
                DrawTitleBar(options, image, ref y);
            }

            NamedStreamReadResult readResult;
            do
            {
                readResult = FillPixelBuffer(options, byteBuffer, pixelBuffer, out int pixelsWritten);
                DrawRasterPixels(pixelBuffer, image, pixelsWritten, ref x, ref y);

                if (readResult == NamedStreamReadResult.EndOfFile && options.TitleMode == TitleMode.OnePerFile)
                {
                    // Draw the next title bar
                    DrawTitleBar(options, image, ref y);
                    x = 0;
                }
            } while (readResult != NamedStreamReadResult.EndOfStream);

            return image;
        }

        private static async Task<Image<Rgba32>> DrawSingleImageStripedInMemoryAsync(DrawOptions options, SingleImageSize imageSize)
        {
            var totalSize = imageSize.TotalSize;
            var image = new Image<Rgba32>(totalSize.Width, totalSize.Height, Color.Black);
            var byteBuffer = new byte[1024 * 3]; // 3KB buffer so that all pixel formats can read a whole number of pixels out of it
            var pixelBuffer = new Rgba32[GetPixelBufferSize(options.PixelFormat, byteBuffer.Length)];
            var x = 0;
            var y = 0;
            var currentBlockIndex = 0;

            if (options.TitleMode != TitleMode.None)
            {
                // Draw the first title bar
                DrawTitleBar(options, image, ref y);

                // Grab the pixel block when we get started for real.
                currentBlockIndex += 1;
            }

            NamedStreamReadResult readResult;
            do
            {
                readResult = FillPixelBuffer(options, byteBuffer, pixelBuffer, out int pixelsWritten);
                DrawStripedPixels(options.PixelFormat, pixelBuffer, image, pixelsWritten, ref x, ref y, imageSize.Blocks[currentBlockIndex]);

                if (readResult == NamedStreamReadResult.EndOfFile)
                {
                    currentBlockIndex++;
                    if (currentBlockIndex >= imageSize.Blocks.Count)
                    {
                        break;
                    }
                    // Move to the next block's starting position
                    x = imageSize.Blocks[currentBlockIndex].Rectangle.X;
                    y = imageSize.Blocks[currentBlockIndex].Rectangle.Y;
                    if (options.TitleMode == TitleMode.OnePerFile)
                    {
                        // Draw the next title bar
                        DrawTitleBar(options, image, ref y);
                    }
                }
            } while (readResult != NamedStreamReadResult.EndOfStream);

            return image;
        }

        // Zoomable canvas
        private static async Task<DrawResult> DrawZoomableCanvasAsync(DrawOptions options)
        {
            throw new NotImplementedException();
        }

        // Multiple to folder
        private static async Task<DrawResult> DrawMultipleToFolderAsync(DrawOptions options)
        {
            throw new NotImplementedException();
        }

        // Helpers
        private static int GetPixelBufferSize(PixelFormat format, int byteBufferSize)
        {
            return format switch
            {
                PixelFormat.Binary1Bpp => byteBufferSize * 8,
                PixelFormat.Binary2Bpp => byteBufferSize * 4,
                PixelFormat.Binary3Bpp => (byteBufferSize / 3) * 8,
                PixelFormat.Binary4Bpp => byteBufferSize * 2,
                PixelFormat.Binary8Bpp => byteBufferSize,
                PixelFormat.Binary16Bpp => byteBufferSize / 2,
                PixelFormat.Binary24Bpp => byteBufferSize / 3,
                PixelFormat.Binary32Bpp => byteBufferSize / 4,
                PixelFormat.Float16 => (byteBufferSize / 2) * 6,
                PixelFormat.Float32 => (byteBufferSize / 4) * 6,
                PixelFormat.Float64 => (byteBufferSize / 8) * 8,
                _ => throw new ArgumentException($"Unsupported pixel format: {format}")
            };
        }

        private static NamedStreamReadResult FillPixelBuffer(DrawOptions options,
            byte[] byteBuffer,
            Rgba32[] pixelBuffer,
            out int pixelsWritten)
        {
            Array.Clear(pixelBuffer);
            var readResult = options.ByteStream.Read(byteBuffer, out var bytesRead);
            if (bytesRead == 0)
            {
                pixelsWritten = 0;
                return readResult;
            }

            PixelDrawer.FillPixelBuffer(options, byteBuffer, pixelBuffer, bytesRead, out pixelsWritten);
            return readResult;
        }

        private static void DrawTitleBar(DrawOptions options, Image<Rgba32> image, ref int y)
        {
            var font = SystemFonts.CreateFont(TitleBarFontFamily, TitleBarFontSize);
            var text = options.ByteStream.CurrentFileName;

            var fontRectangle = TextMeasurer.MeasureSize(text, new TextOptions(font));
            while (fontRectangle.Width > image.Width)
            {
                if (!Helpers.TryShortenFilePath(text, out text)) { break; }
                fontRectangle = TextMeasurer.MeasureSize(text, new TextOptions(font));
            }

            // Center-align the text in the title bar height
            var verticalMargin = (TitleBarHeight - fontRectangle.Height) / 2;
            var position = new PointF(0, y + verticalMargin);

            image.Mutate(ctx => ctx.DrawText(text, font, Color.White, position));
            y += TitleBarHeight;
        }

        private static void DrawRasterPixels(Rgba32[] pixelBuffer, Image<Rgba32> image, int pixelsWritten, ref int x, ref int y)
        {
            for (var i = 0; i < pixelsWritten; i++)
            {
                if (x >= image.Width)
                {
                    x = 0;
                    y++;
                    if (y >= image.Height)
                    {
                        break;
                    }
                }
                image[x, y] = pixelBuffer[i];
                x++;
            }
        }

        private static void DrawStripedPixels(PixelFormat pixelFormat, Rgba32[] pixelBuffer, Image<Rgba32> image, int pixelsWritten, ref int x, ref int y, SingleImageBlock block)
        {
            var stripeWidth = Helpers.GetStripeWidth(pixelFormat);

            for (var i = 0; i < pixelsWritten; i++)
            {
                if (y >= block.Rectangle.Bottom)
                {
                    if ((x + stripeWidth) > block.Rectangle.Right)
                    {
                        break;
                    }
                    x += stripeWidth;
                    y = block.Rectangle.Top;
                }

                image[x, y] = pixelBuffer[i];
                x += 1;
                if ((x - block.Rectangle.Left) % stripeWidth == 0)
                {
                    y++;
                    x -= stripeWidth;
                }
            }
        }
    }
}
