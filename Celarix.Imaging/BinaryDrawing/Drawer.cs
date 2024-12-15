using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Celarix.Imaging.Collections;
using Celarix.Imaging.IO;
using Celarix.Imaging.Misc;
using Celarix.Imaging.Progress;
using Celarix.Imaging.Utilities;
using Celarix.Imaging.ZoomableCanvas;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Celarix.Imaging.BinaryDrawing
{
	public static class Drawer
    {
        public static Image<Rgba32> Draw(Stream stream,
            int bitDepth,
            IReadOnlyList<Rgba32> palette,
            CancellationToken cancellationToken,
            IProgress<DrawingProgress> progress)
        {
            ValidateBitDepthAndPalette(bitDepth, palette?.Count);

            var pixelCount = GetPixelCount(stream, bitDepth);
            var (width, height) = Helpers.GetSizeFromCount(pixelCount);
            var pixelEnumerator = GetPixelEnumeratorFromStream(stream, bitDepth);

            var image = new Image<Rgba32>(width, height);
            var drawnPixels = 0L;

            foreach (var pixel in pixelEnumerator)
			{
				var x = (int)(drawnPixels % width);
				var y = (int)(drawnPixels / width);

                SetPixelOnImage(image, x, y, pixel, bitDepth, palette);

				drawnPixels += 1;
				if (drawnPixels % LibraryConfiguration.Instance.BinaryDrawingReportsProgressEveryNPixels != 0) { continue; }

				if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }

                progress?.Report(new DrawingProgress
                {
                    DrawnPixels = drawnPixels, TotalPixels = pixelCount
                });
			}

            return image;
        }

        public static void DrawCanvas(Stream stream,
            string outputFolderPath,
            int bitDepth,
            IReadOnlyList<Rgba32> palette,
            CancellationToken cancellationToken,
            IProgress<DrawingProgress> progress)
        {
            ValidateBitDepthAndPalette(bitDepth, palette?.Count);

            var pixelCount = GetPixelCount(stream, bitDepth);
            var size = Helpers.GetSizeFromCount(pixelCount);
            var (canvasWidth, _) = Helpers.GetCanvasSizeFromImageSize(size);
            var tileEdgeLength = LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength;
            var pixelEnumerator = GetPixelEnumeratorFromStream(stream, bitDepth);
            var drawnPixels = 0;
            var rowsDrawnForTileRow = 0;
            var pixelYPosition = 0;
            var pixelsDrawnOnRow = 0;
            var rowImages = new Image<Rgba32>[canvasWidth];
            
            InitializeCanvasRowImages(rowImages);

            foreach (var pixel in pixelEnumerator)
            {
                if (pixelsDrawnOnRow == size.Width)
                {
                    // A row has been drawn, move to the next one.
                    pixelsDrawnOnRow = 0;
                    rowsDrawnForTileRow += 1;
                    pixelYPosition += 1;
                }

                if (rowsDrawnForTileRow == tileEdgeLength)
                {
                    // We've finished drawing 256 rows, so we can save all the images.
                    var tileIndexY = (pixelYPosition / tileEdgeLength) - 1;

                    for (var tileIndexX = 0; tileIndexX < rowImages.Length; tileIndexX++)
                    {
                        var rowImage = rowImages[tileIndexX];
                        CanvasGenerator.SaveLevel0CellImage(new Point(tileIndexX, tileIndexY), rowImage, outputFolderPath);
                        rowImage.Dispose();
                    }
                    
                    InitializeCanvasRowImages(rowImages);
                    rowsDrawnForTileRow = 0;
                }

                var pixelXTile = pixelsDrawnOnRow / tileEdgeLength;
                var pixelXPositionInTile = pixelsDrawnOnRow % tileEdgeLength;
                var pixelYPositionInTile = pixelYPosition % tileEdgeLength;
                SetPixelOnImage(rowImages[pixelXTile], pixelXPositionInTile, pixelYPositionInTile, pixel, bitDepth, palette);
                pixelsDrawnOnRow += 1;
                drawnPixels += 1;

                if (drawnPixels % LibraryConfiguration.Instance.BinaryDrawingReportsProgressEveryNPixels != 0) { continue; }

                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }

                progress?.Report(new DrawingProgress
                {
                    DrawnPixels = drawnPixels, TotalPixels = pixelCount
                });
            }

            var lastTileIndexY = pixelYPosition / tileEdgeLength;

            for (var tileIndexX = 0; tileIndexX < rowImages.Length; tileIndexX++)
            {
                var rowImage = rowImages[tileIndexX];
                CanvasGenerator.SaveLevel0CellImage(new Point(tileIndexX, lastTileIndexY), rowImage, outputFolderPath);
                rowImage.Dispose();
            }
        }
        
        public static Image<Rgba32> DrawFixedSize(PartiallyKnownSize size,
            Stream stream,
            int bitDepth,
            IReadOnlyList<Rgba32> palette,
            CancellationToken cancellationToken,
            IProgress<DrawingProgress> progress)
		{
			ValidateBitDepthAndPalette(bitDepth, palette?.Count);
			var totalPixelsInStream = (stream.Length * bitDepth) / 8;
            var (width, height) = GetOtherSizeComponent(size, totalPixelsInStream);
            var totalPixelsInImage = (long)width * height;
            var totalBytes = (int)Math.Ceiling((totalPixelsInImage * bitDepth) / 8m);
			var pixelEnumerator = GetPixelEnumeratorFromStream(stream, bitDepth, totalBytes);

			var image = new Image<Rgba32>(width, height);
			var drawnPixels = 0L;

			foreach (var pixel in pixelEnumerator)
			{
				var x = (int)(drawnPixels % width);
				var y = (int)(drawnPixels / width);

				SetPixelOnImage(image, x, y, pixel, bitDepth, palette);

				drawnPixels += 1;
				if (drawnPixels == totalPixelsInImage) { break; }
				if (drawnPixels % LibraryConfiguration.Instance.BinaryDrawingReportsProgressEveryNPixels != 0) { continue; }

				if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }

                progress?.Report(new DrawingProgress
                {
                    DrawnPixels = drawnPixels, TotalPixels = totalPixelsInImage
                });
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
            var (width, height) = size;
            var textHeight = Helpers.GetTextHeight(height);
            var totalPixels = (long)width * (height - textHeight);
            var totalBytes = (int)Math.Ceiling((totalPixels * bitDepth) / 8m);
            var pixelEnumerator = GetPixelEnumeratorFromStream(stream, bitDepth, totalBytes);

            var image = new Image<Rgba32>(width, height, Color.Black);
            var drawnPixels = 0L;

            foreach (var pixel in pixelEnumerator)
            {
                var x = (int)(drawnPixels % width);
                var y = (int)((drawnPixels / width) + textHeight);

                SetPixelOnImage(image, x, y, pixel, bitDepth, palette);

                drawnPixels += 1;
                if (drawnPixels == totalPixels) { break; }

                if (drawnPixels % LibraryConfiguration.Instance.BinaryDrawingReportsProgressEveryNPixels != 0) { continue; }

                if (cancellationToken.IsCancellationRequested) { throw new TaskCanceledException(); }

                progress?.Report(new DrawingProgress
                {
                    DrawnPixels = drawnPixels, TotalPixels = totalPixels
                });
            }

            var fileNames = stream.NameBuffer.Distinct().ToList();
            var fileText = fileNames.Count == 1
                ? fileNames[0]
                : $"{fileNames[0]} +{fileNames.Count - 1}";
            DrawSourceOnImage(image, fileText);

            return image;
        }

        public static Image<Rgba32> Sort(Image<Rgba32> image,
	        SortMode sortMode,
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

                progress?.Report(new DrawingProgress
                {
                    DrawnPixels = y * image.Width, TotalPixels = image.Width * image.Height * 2
                });
            }

            Func<Rgba32, int> keySelector = sortMode switch
			{
				SortMode.RGB => p => (p.R << 24) | (p.G << 16) | (p.B << 8) | p.A,
				SortMode.HSV => p =>
				{
					var hsv = RGBToHSV(p);
					return (hsv.Hue << 16) | (hsv.Saturation << 8) | hsv.Value;
				},
				SortMode.YCbCr => p =>
				{
					var yCbCr = p.ToYCbCr();
					return (yCbCr.Y << 16) | (yCbCr.Cb << 8) | yCbCr.Cr;
				},
				_ => throw new ArgumentException(nameof(sortMode))
			};
            
            var sortedPixels = pixels.OrderBy(keySelector).ToList();
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
            var pixels = new HashSet<Rgba32>();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++) { pixels.Add(image[x, y]); }

                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(new DrawingProgress
                {
                    DrawnPixels = y * image.Width, TotalPixels = image.Width * image.Height * 2
                });
            }

            var uniqueColors = pixels.ToList();
            var (uniqueImageWidth, uniqueImageHeight) = Helpers.GetSizeFromCount(uniqueColors.Count);
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
                    var byteIndex = (y * image.Height) + (x * image.Width * 4);

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

        private static IEnumerable<int> GetPixelEnumeratorFromStream(Stream stream, int bitDepth, int bufferSize = 1048576) =>
            new StreamEnumerable(stream).EnumeratePixels(bitDepth, bufferSize);

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

            while (TextMeasurer.MeasureSize(text, new TextOptions(font)).Width > image.Width)
            {
                if (!Helpers.TryShortenFilePath(text, out text)) { break; }
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

        private static void InitializeCanvasRowImages(IList<Image<Rgba32>> images)
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i] = new Image<Rgba32>(LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength,
                    LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength, Color.Black);
            }
        }

        private static Size GetOtherSizeComponent(PartiallyKnownSize partiallyKnownSize, long totalPixels)
        {
	        if (partiallyKnownSize.Width == null && partiallyKnownSize.Height == null)
	        {
		        throw new ArgumentException("At least one dimension must be known.");
	        }

	        var width = 0;
	        var height = 0;

	        if (partiallyKnownSize.Width == null && partiallyKnownSize.Height != null)
	        {
		        width = (int)(totalPixels / partiallyKnownSize.Height.Value);
		        height = partiallyKnownSize.Height.Value;
	        }
	        else if (partiallyKnownSize is { Width: not null, Height: null })
	        {
		        width = partiallyKnownSize.Width.Value;
		        height = (int)(totalPixels / partiallyKnownSize.Width.Value);
	        }
	        else if (partiallyKnownSize is { Width: not null, Height: not null })
	        {
		        width = partiallyKnownSize.Width.Value;
		        height = partiallyKnownSize.Height.Value;
	        }
	        
	        return new Size(width, height);
        }

        private static (int Hue, int Saturation, int Value) RGBToHSV(Rgba32 pixel)
        {
	        var r = pixel.R / 255f;
			var g = pixel.G / 255f;
			var b = pixel.B / 255f;

			var max = Math.Max(r, Math.Max(g, b));
			var min = Math.Min(r, Math.Min(g, b));
			var delta = max - min;

			var hue = 0;
			var saturation = 0;
			var value = (int)(max * 255);

			if (delta != 0)
			{
				saturation = (int)((delta / max) * 255);

				if (Math.Abs(max - r) < 0.001d)
				{
					hue = (int)((g - b) / delta);
				}
				else if (Math.Abs(max - g) < 0.001d)
				{
					hue = (int)((b - r) / delta) + 2;
				}
				else if (Math.Abs(max - b) < 0.001d)
				{
					hue = (int)((r - g) / delta) + 4;
				}

				hue *= 60;
				if (hue < 0) { hue += 360; }
			}

			return (hue, saturation, value);
        }

        private static (int Y, int Cb, int Cr) ToYCbCr(this Rgba32 pixel)
        {
            var y = (int)((0.299 * pixel.R) + (0.587 * pixel.G) + (0.114 * pixel.B));
            var cb = (int)((128 - (0.168736 * pixel.R) - (0.331264 * pixel.G)) + (0.5 * pixel.B));
            var cr = (int)((128 + (0.5 * pixel.R)) - (0.418688 * pixel.G) - (0.081312 * pixel.B));

            return (y, cb, cr);
        }
    }
}
