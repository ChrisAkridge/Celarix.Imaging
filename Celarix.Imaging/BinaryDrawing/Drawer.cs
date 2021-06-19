using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Celarix.Imaging.Collections;
using Celarix.Imaging.Extensions;
using Celarix.Imaging.IO;
using Celarix.Imaging.Progress;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
namespace Celarix.Imaging.BinaryDrawing
{
	public static class Drawer
    {
        private const long ReportEveryNPixels = 8192;

        public static Image<Rgba32> Draw(Stream stream,
            int bitDepth,
            IReadOnlyList<Rgba32> palette,
            CancellationToken cancellationToken,
            IProgress<DrawingProgress> progress)
        {
            ValidateBitDepthAndPalette(bitDepth, palette?.Count);

            var pixelCount = GetPixelCount(stream, bitDepth);
            var (width, height) = Utilities.GetSizeFromCount(pixelCount);
            var pixelEnumerator = GetPixelEnumeratorFromStream(stream, bitDepth);

            var image = new Image<Rgba32>(width, height);
            var drawnPixels = 0L;

            foreach (var pixel in pixelEnumerator)
			{
				var x = (int)(drawnPixels % width);
				var y = (int)(drawnPixels / width);

                SetPixelOnImage(image, x, y, pixel, bitDepth, palette);

				drawnPixels += 1;
				if (drawnPixels % ReportEveryNPixels != 0) { continue; }

				if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }
				progress?.Report(new DrawingProgress { DrawnPixels = drawnPixels, TotalPixels = pixelCount });
			}

            return image;
        }
        public static Image<Rgba32> DrawFixedSize(Size size,
            Stream stream,
            int bitDepth,
            IReadOnlyList<Rgba32> palette,
            CancellationToken cancellationToken,
            IProgress<DrawingProgress> progress)
		{
			ValidateBitDepthAndPalette(bitDepth, palette?.Count);
			var totalPixels = (long)size.Width * size.Height;
            var totalBytes = (int)Math.Ceiling(totalPixels * bitDepth / 8m);
			var pixelEnumerator = GetPixelEnumeratorFromStream(stream, bitDepth, totalBytes);

			var image = new Image<Rgba32>(size.Width, size.Height);
			var drawnPixels = 0L;

			foreach (var pixel in pixelEnumerator)
			{
				var x = (int)(drawnPixels % size.Width);
				var y = (int)(drawnPixels / size.Width);

				SetPixelOnImage(image, x, y, pixel, bitDepth, palette);

				drawnPixels += 1;
				if (drawnPixels == totalPixels) { break; }
				if (drawnPixels % ReportEveryNPixels != 0) { continue; }

				if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }

				progress?.Report(new DrawingProgress { DrawnPixels = drawnPixels, TotalPixels = totalPixels });
			}

            return image;
        }

        public static Image<Rgba32> DrawFixedSizeWithSourceText(Size size,
            NamedMultiStream stream,
            int bitDepth,
            IReadOnlyList<Rgba32> palette,
            CancellationToken cancellationToken,
            IProgress<DrawingProgress> progress)
        {
            ValidateBitDepthAndPalette(bitDepth, palette?.Count);

            stream.NameBuffer = new List<string>();
            var textHeight = Utilities.GetTextHeight(size.Height);
            var totalPixels = (long)size.Width * (size.Height - textHeight);
            var totalBytes = (int)Math.Ceiling(totalPixels * bitDepth / 8m);
            var pixelEnumerator = GetPixelEnumeratorFromStream(stream, bitDepth, totalBytes);

            var image = new Image<Rgba32>(size.Width, size.Height, Color.Black);
            var drawnPixels = 0L;

            foreach (var pixel in pixelEnumerator)
            {
                var x = (int)(drawnPixels % size.Width);
                var y = (int)((drawnPixels / size.Width) + textHeight);

                SetPixelOnImage(image, x, y, pixel, bitDepth, palette);

                drawnPixels += 1;
                if (drawnPixels == totalPixels) { break; }

                if (drawnPixels % ReportEveryNPixels != 0) { continue; }

                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }

                progress?.Report(new DrawingProgress { DrawnPixels = drawnPixels, TotalPixels = totalPixels });
            }

            var fileNames = stream.NameBuffer.Distinct().ToList();
            var fileText = fileNames.Count == 1
                ? fileNames[0]
                : $"{fileNames[0]} +{fileNames.Count - 1}";
            DrawSourceOnImage(image, fileText);

            return image;
        }

        public static Image<Rgba32> Sort(Image<Rgba32> image,
            CancellationToken cancellationToken,
            IProgress<DrawingProgress> progress)
        {
            var pixels = new List<Rgba32>();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    pixels.Add(image[x, y]);
                }

                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(new DrawingProgress { DrawnPixels = y * image.Width, TotalPixels = image.Width * image.Height * 2});
            }

            var sortedPixels = pixels.OrderBy(p => (p.R << 24) | (p.G << 16) | (p.B << 8) | p.A).ToList();
            var sortedImage = new Image<Rgba32>(image.Width, image.Height);

            int pixelIndex = 0;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    sortedImage[x, y] = sortedPixels[pixelIndex];
                    pixelIndex++;
                }

                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(new DrawingProgress
                {
                    DrawnPixels = (image.Width * image.Height) + (y * image.Width),
                    TotalPixels = (image.Width * image.Height) * 2
                });
            }

            return sortedImage;
        }

        public static UniqueColorsImage<Rgba32> UniqueColors(Image<Rgba32> image,
            CancellationToken cancellationToken,
            IProgress<DrawingProgress> progress)
        {
            var pixels = new List<Rgba32>();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++) { pixels.Add(image[x, y]); }

                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(new DrawingProgress
                    { DrawnPixels = y * image.Width, TotalPixels = image.Width * image.Height * 2 });
            }

            var uniqueColors = pixels.Distinct().ToList();
            var (uniqueImageWidth, uniqueImageHeight) = Utilities.GetSizeFromCount(uniqueColors.Count);
            var uniqueImage = new Image<Rgba32>(uniqueImageWidth, uniqueImageHeight);

            int pixelIndex = 0;
            for (int y = 0; y < uniqueImageHeight; y++)
            {
                for (int x = 0; x < uniqueImageWidth; x++)
                {
                    uniqueImage[x, y] = (pixelIndex < uniqueColors.Count)
                        ? uniqueColors[pixelIndex]
                        : Color.Black.ToPixel<Rgba32>();
                    pixelIndex++;
                }

                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(new DrawingProgress
                {
                    DrawnPixels = y * uniqueImageWidth,
                    TotalPixels = (uniqueImageWidth * uniqueImageHeight)
                });
            }

            return new UniqueColorsImage<Rgba32>(uniqueColors.Count, uniqueImage);
        }

        public static byte[] ToRaw(Image<Rgba32> image,
            CancellationToken cancellationToken,
            IProgress<DrawingProgress> progress)
        {
            var bytes = new byte[image.Width * image.Height * 4];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image[x, y];
                    var byteIndex = (y * image.Height) + x * image.Width * 4;

                    bytes[byteIndex] = pixel.R;
                    bytes[byteIndex + 1] = pixel.G;
                    bytes[byteIndex + 2] = pixel.B;
                    bytes[byteIndex + 3] = pixel.A;
                }

                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(new DrawingProgress
                {
                    DrawnPixels = y * image.Width,
                    TotalPixels = image.Width * image.Height
                });
            }

            return bytes;
        }

        private static IEnumerable<int> GetPixelEnumeratorFromStream(Stream stream, int bitDepth, int bufferSize = 1048576)
        {
            var streamEnumerable = new StreamEnumerable(stream);
            return streamEnumerable.EnumeratePixels(bitDepth, bufferSize);
        }

        private static void SetPixelOnImage(Image<Rgba32> image,
            int x,
            int y,
            int pixel,
            int bitDepth,
            IReadOnlyList<Rgba32> palette)
        {
            if (palette != null) { image[x, y] = palette[pixel]; }
            else
            {
                var color = bitDepth switch
                {
                    24 => new Rgba32((byte)(pixel >> 16),
                        (byte)((pixel >> 8) & 0xFF),
                        (byte)(pixel & 0xFF),
                        255),
                    32 => new Rgba32((byte)(pixel >> 24),
                        (byte)((pixel >> 16) & 0xFF),
                        (byte)((pixel >> 8) & 0xFF),
                        (byte)(pixel & 0xFF)),
                    _ => throw new ArgumentException(nameof(bitDepth))
                };

                image[x, y] = color;
            }
        }

        private static void DrawSourceOnImage(Image<Rgba32> image, string text)
        {
            var font = SystemFonts.CreateFont("Consolas", 20f);

            while (TextMeasurer.Measure(text, new RendererOptions(font)).Width > image.Width)
            {
                if (!Utilities.TryShortenFilePath(text, out text)) { break; }
            }

            image.Mutate(ctx => ctx.DrawText(text, font, Color.White, PointF.Empty));
        }

        private static void ValidateBitDepthAndPalette(int bitDepth, int? paletteSize)
        {
            var valid = bitDepth switch
			{
				1 => paletteSize == 2,
				2 => paletteSize == 4,
                4 => paletteSize == 16,
                8 => paletteSize == 256,
                16 => paletteSize == 65536,
                24 => paletteSize == null,
                32 => paletteSize == null,
				_ => false
			};

            if (!valid)
            {
                throw new ArgumentException($"A bit depth of {bitDepth} with a palette of {paletteSize?.ToString() ?? "no"} colors is not valid.");
            }
		}

        private static long GetPixelCount(Stream stream, int bitDepth) =>
            bitDepth switch
            {
                1 => stream.Length * 8,
                2 => stream.Length * 4,
                4 => stream.Length * 2,
                8 => stream.Length,
                16 => (long)Math.Ceiling(stream.Length / 2m),
                24 => (long)Math.Ceiling(stream.Length / 3m),
                32 => (long)Math.Ceiling(stream.Length / 4m),
                _ => throw new ArgumentException(nameof(bitDepth))
            };
	}
}
