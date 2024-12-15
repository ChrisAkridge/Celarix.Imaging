using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celarix.Imaging.BinaryDrawing;
using Celarix.Imaging.IO;
using Celarix.Imaging.Progress;
using Celarix.Imaging.ZoomableCanvas;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.ByteViewCLI
{
    internal sealed class BinaryDrawerService
    {
        private readonly Progress<DrawingProgress> progress = new Progress<DrawingProgress>();

        public BinaryDrawerService() => progress.ProgressChanged += Progress_ProgressChanged;

        private static void Progress_ProgressChanged(object? sender, DrawingProgress e)
        {
            var percentage = (int)Math.Round((e.DrawnPixels * 100m) / e.TotalPixels, 2);
            Console.WriteLine($"Drawing progress: {percentage}% ({e.DrawnPixels:#,###} of {e.TotalPixels:#,###} pixels).");
        }

        public void RunBinaryDraw(string inputPath, string outputPath, int bitDepth, string colorMode)
        {
            Console.WriteLine($"Drawing binary image for {inputPath}...");

            DrawFiles([inputPath], outputPath, bitDepth, colorMode);
        }

        public void RunBinaryDrawMultipleFiles(string inputOption, string input, string outputPath, int bitDepth, string colorMode)
        {
			var fileList = Utilities.LoadFilesFromInput(inputOption, input);
			Console.WriteLine($"Drawing binary image for {fileList.Length} files...");
			
			DrawFiles(fileList, outputPath, bitDepth, colorMode);
		}

        public void RunBinaryDrawFrames(string inputPath, string outputFolderPath, int bitDepth,
	        string colorMode, int width, int height)
        {
	        Console.WriteLine($"Drawing binary image frames for {inputPath}...");
	        DrawFrames([inputPath], outputFolderPath, bitDepth, colorMode, width, height, false);
        }
        
        public void RunBinaryDrawFramesMultipleFiles(string inputOption, string input, string outputFolderPath, int bitDepth,
	        string colorMode, int width, int height, bool printFileNames)
		{
	        var fileList = Utilities.LoadFilesFromInput(inputOption, input);
	        Console.WriteLine($"Drawing binary image frames for {fileList.Length} files...");
	        
	        DrawFrames(fileList, outputFolderPath, bitDepth, colorMode, width, height, printFileNames);
		}

		public void RunBinaryDrawCanvas(string inputPath, string outputFolderPath, int bitDepth,
			string colorMode)
		{
			Console.WriteLine($"Drawing zoomable canvas for {inputPath}...");
			DrawZoomableCanvas([inputPath], outputFolderPath, bitDepth, colorMode);
		}

		public void RunBinaryDrawCanvasMultipleFiles(string inputOption, string input, string outputFolderPath,
			int bitDepth, string colorMode)
		{
			var fileList = Utilities.LoadFilesFromInput(inputOption, input);
			Console.WriteLine($"Drawing zoomable canvas for {fileList.Length} files...");
			DrawZoomableCanvas(fileList, outputFolderPath, bitDepth, colorMode);
		}

		public void RunBinaryDrawFixedSize(string inputPath,
			string outputPath,
			int width,
			int height)
		{
			Console.WriteLine($"Drawing fixed-size binary image for {inputPath}...");
			var image = Drawer.DrawFixedSize(new PartiallyKnownSize(width, height),
				File.OpenRead(inputPath),
				24,
				null,
				CancellationToken.None,
				progress);
			Utilities.SaveImage(outputPath, image);
		}

		public void RunUniqueColors(string inputPath, string outputPath)
		{
			Console.WriteLine($"Getting unique colors for {inputPath}...");

			var uniqueColors = Drawer.UniqueColors(Image.Load<Rgba32>(inputPath),
				CancellationToken.None,
				progress);
			Console.WriteLine($"Unique colors: {uniqueColors.UniqueColors:#,###} / 16,777,216 ({((uniqueColors.UniqueColors / 16_777_216f) * 100f):F2}% of color space)");
			Utilities.SaveImage(outputPath, uniqueColors.Image);
		}
		
		public void RunSort(string inputPath, string outputPath, string sortMode)
		{
			Console.WriteLine($"Sorting {inputPath}...");
			var sortedImage = Drawer.Sort(Image.Load<Rgba32>(inputPath),
				sortMode.ToLowerInvariant() switch
				{
					"rgb" => SortMode.RGB,
					"hsv" => SortMode.HSV,
					"ycbcr" => SortMode.YCbCr,
					_ => throw new ArgumentException($"Invalid sort mode {sortMode}.")
				},
				CancellationToken.None,
				progress);
			Utilities.SaveImage(outputPath, sortedImage);
		}

		public void RunUniqueColorsInSpace(string inputPath, string outputPath, int bitDepth)
		{
			Console.WriteLine($"Placing unique colors for {inputPath} onto the space of all {bitDepth}-bit colors...");
			var image = Image.Load<Rgba32>(inputPath);
			
			if (bitDepth is 1 or 2)
			{
				ChromaPlaygroundService.ReduceToGrayscale(image, bitDepth);
			}
			else if (bitDepth is 4 or 8 or 16)
			{
				ChromaPlaygroundService.ReduceByShifting(image, bitDepth);
			}

			var spaceImageWidth = bitDepth switch
			{
				1 => 2,
				2 => 2,
				4 => 4,
				8 => 16,
				16 => 256,
				24 => 4096,
				_ => throw new ArgumentException($"Unsupported bit depth {bitDepth}.")
			};
			
			var spaceImageHeight = bitDepth == 1 ? 1 : spaceImageWidth;
			var colorSeen = new BitArray(16_777_216);
			
			for (var y = 0; y < image.Height; y++)
			{
				for (var x = 0; x < image.Width; x++)
				{
					var pixel = image[x, y];
					var color = (pixel.R << 16) | (pixel.G << 8) | pixel.B;
					colorSeen[color] = true;
				}
			}

			var spaceImage = new Image<Rgba32>(spaceImageWidth, spaceImageHeight, Rgba32.ParseHex("000000FF"));
			if (bitDepth == 1)
			{
				SetColorOnColorSpaceImage(0xFFFFFF, spaceImage, colorSeen, 1, 0);
			}
			else if (bitDepth == 2)
			{
				SetColorOnColorSpaceImage(0x555555, spaceImage, colorSeen, 0, 1);
				SetColorOnColorSpaceImage(0xAAAAAA, spaceImage, colorSeen, 0, 1);
				SetColorOnColorSpaceImage(0xFFFFFF, spaceImage, colorSeen, 1, 1);
			}
			else if (bitDepth == 4)
			{
				SetColorOnColorSpaceImage(0x0000FF, spaceImage, colorSeen, 1, 0);
				SetColorOnColorSpaceImage(0x005500, spaceImage, colorSeen, 2, 0);
				SetColorOnColorSpaceImage(0x0055FF, spaceImage, colorSeen, 3, 0);
				SetColorOnColorSpaceImage(0x00AA00, spaceImage, colorSeen, 0, 1);
				SetColorOnColorSpaceImage(0x00AAFF, spaceImage, colorSeen, 1, 1);
				SetColorOnColorSpaceImage(0x00FF00, spaceImage, colorSeen, 2, 1);
				SetColorOnColorSpaceImage(0x00FFFF, spaceImage, colorSeen, 3, 1);
				SetColorOnColorSpaceImage(0xFF0000, spaceImage, colorSeen, 0, 2);
				SetColorOnColorSpaceImage(0xFF00FF, spaceImage, colorSeen, 1, 2);
				SetColorOnColorSpaceImage(0xFF5500, spaceImage, colorSeen, 2, 2);
				SetColorOnColorSpaceImage(0xFF55FF, spaceImage, colorSeen, 3, 2);
				SetColorOnColorSpaceImage(0xFFAA00, spaceImage, colorSeen, 0, 3);
				SetColorOnColorSpaceImage(0xFFAAFF, spaceImage, colorSeen, 1, 3);
				SetColorOnColorSpaceImage(0xFFFF00, spaceImage, colorSeen, 2, 3);
				SetColorOnColorSpaceImage(0xFFFFFF, spaceImage, colorSeen, 3, 3);
			}
			else if (bitDepth == 8)
			{
				// Space image is 16x16
				// RGB 3:3:2
				// Due to the way we did this (by shifting the red and green channels by 5 and shifting the blue by 6),
				// our red and green levels are 0x00, 0x20, 0x40, 0x60, 0x80, 0xA0, 0xC0, 0xE0
				// and our blue levels are 0x00, 0x40, 0x80, 0xC0, so it turns out, uh, pure red, green,
				// and blue are not actually in the color space. Whoops.
				for (var red = 0; red < 0xFF; red += 0x20)
				{
					for (var green = 0; green < 0xFF; green += 0x20)
					{
						for (var blue = 0; blue < 0xFF; blue += 0x40)
						{
							var color = (red << 16) | (green << 8) | blue;
							int pixelIndex = red + (green >> 3) + (blue >> 6);
							int x = pixelIndex % 16;
							int y = pixelIndex / 16;
							SetColorOnColorSpaceImage(color, spaceImage, colorSeen, x, y);
						}
					}
				}
			}
			else if (bitDepth == 16)
			{
				// Space image is 256x256
				// RGB 5:6:5
				// Red and blue have 32 levels made by shifting the value 3 bits to the right:
				// 0x00, 0x08, 0x10 ... 0xF8
				// Green has 64 levels made by shifting the value 2 bits to the right:
				// 0x00, 0x04, 0x08 ... 0xFC
				for (var red = 0; red < 0xFF; red += 0x08)
				{
					for (var green = 0; green < 0xFF; green += 0x04)
					{
						for (var blue = 0; blue < 0xFF; blue += 0x08)
						{
							var color = (red << 11) | (green << 5) | blue;
							int pixelIndex = (red << 8) | (green << 3) | blue;
							int x = pixelIndex % 256;
							int y = pixelIndex / 256;
							SetColorOnColorSpaceImage(color, spaceImage, colorSeen, x, y);
						}
					}
				}
			}
			else if (bitDepth == 24)
			{
				// Space image is 4096x4096
				// RGB 8:8:8, no really fancy code needed
				for (var red = 0; red < 0xFF; red++)
				{
					for (var green = 0; green < 0xFF; green++)
					{
						for (var blue = 0; blue < 0xFF; blue++)
						{
							var color = (red << 16) | (green << 8) | blue;
							int pixelIndex = (red << 16) | (green << 8) | blue;
							int x = pixelIndex % 4096;
							int y = pixelIndex / 4096;
							SetColorOnColorSpaceImage(color, spaceImage, colorSeen, x, y);
						}
					}
				}
			}
			
			Utilities.SaveImage(outputPath, spaceImage);
		}

		private void DrawFiles(string[] fileList, string outputPath, int bitDepth, string colorMode)
		{
			var bitDepthAndColorMode = GetBitDepthAndColorMode(bitDepth, colorMode);
			var imagingColorMode = GetImagingColorMode(bitDepthAndColorMode);
			Console.WriteLine($"Bit depth and color mode: {bitDepthAndColorMode}");

			var source = new FileSource(fileList);
			var imageSizeBytes = source.FileSizes.Sum();
			Console.WriteLine($"File(s) size: {imageSizeBytes:#,###} bytes");

			IReadOnlyList<Rgba32>? palette = null;
			if (bitDepth is not (24 or 32))
			{
				palette = DefaultPalettes.GetPalette(bitDepth, imagingColorMode);
			}

			var imageSharpImage = Drawer.Draw(source.GetStream(),
				bitDepth,
				palette,
				CancellationToken.None,
				progress);
			Console.WriteLine($"Resulting image is {imageSharpImage.Width}x{imageSharpImage.Height} ({imageSharpImage.Width * imageSharpImage.Height:#,###} pixels)");
			Utilities.SaveImage(outputPath, imageSharpImage);
		}

		private void DrawFrames(string[] fileList,
			string outputFolderPath,
			int bitDepth,
			string colorMode,
			int width,
			int height,
			bool printFileNames)
		{
			var bitDepthAndColorMode = GetBitDepthAndColorMode(bitDepth, colorMode);
			var imagingColorMode = GetImagingColorMode(bitDepthAndColorMode);
			Console.WriteLine($"Bit depth and color mode: {bitDepthAndColorMode}");

			var stream = new NamedMultiStream(fileList);
			var textHeight = Math.Max(24, height / 30);
			var trueHeight = printFileNames
				? height - textHeight
				: height;
			var byteCountPerFrame = Math.Floor(bitDepth switch
			{
				1 => (width * trueHeight) / 8m,
				2 => (width * trueHeight) / 4m,
				4 => (width * trueHeight) / 2m,
				8 => width * trueHeight,
				16 => width * trueHeight * 2,
				24 => width * trueHeight * 3,
				32 => width * trueHeight * 4,
				_ => throw new ArgumentException($"Unsupported bit depth {bitDepth}.")
			});
			var imageSize = new Size(width, height);
			var allFilesSize = stream.Length;

			int totalFrames;
			if (allFilesSize % byteCountPerFrame == 0)
			{
				totalFrames = (int)(allFilesSize / byteCountPerFrame);
			}
			else
			{
				totalFrames = (int)(allFilesSize / byteCountPerFrame) + 1;
			}

			IReadOnlyList<Rgba32>? palette = null;

			if (bitDepth is not (24 or 32))
			{
				palette = DefaultPalettes.GetPalette(bitDepth, imagingColorMode);
			}

			for (var frameNumber = 0; frameNumber < totalFrames; frameNumber++)
			{
				Console.WriteLine($"Drawing frame {frameNumber + 1} of {totalFrames}...");
				var image = printFileNames
					? Drawer.DrawFixedSizeWithSourceText(imageSize,
						stream,
						bitDepth,
						palette,
						CancellationToken.None,
						progress)
					: Drawer.DrawFixedSize(new PartiallyKnownSize(width, height),
						stream,
						bitDepth,
						palette,
						CancellationToken.None,
						progress);
				var resultPath = Path.Combine(outputFolderPath, $"image_{frameNumber:D8}.png");
				Utilities.SaveImage(resultPath, image);
			}
		}

		private void DrawZoomableCanvas(string[] fileList,
			string outputFolderPath,
			int bitDepth,
			string colorMode)
		{
			var stream = new NamedMultiStream(fileList);
			var totalBytes = stream.Length;
			var tileEdgeLength = LibraryConfiguration.Instance.ZoomableCanvasTileEdgeLength;
			var tileBytes = tileEdgeLength * tileEdgeLength * (bitDepth / 8);
			var level0Tiles = (int)Math.Ceiling((decimal)totalBytes / tileBytes);
			var estimatedTotalTiles = (int)Math.Ceiling(level0Tiles * Math.Log(level0Tiles, 4d));
			var canvasSizeInTiles = GetSizeFromCount(level0Tiles);

			IReadOnlyList<Rgba32>? palette = null;
			if (bitDepth is not (24 or 32))
			{
				var bitDepthAndColorMode = GetBitDepthAndColorMode(bitDepth, colorMode);
				var imagingColorMode = GetImagingColorMode(bitDepthAndColorMode);
				palette = DefaultPalettes.GetPalette(bitDepth, imagingColorMode);
			}

			var level0Path = Path.Combine(outputFolderPath, "0");
			Directory.CreateDirectory(level0Path);

			var savedTiles = 0;
			for (var y = 0; y < canvasSizeInTiles.Height; y++)
			{
				for (var x = 0; x < canvasSizeInTiles.Width; x++)
				{
					Console.WriteLine($"Drawing tile #{savedTiles + 1} of {estimatedTotalTiles}...");
					var tile = Drawer.DrawFixedSize(new PartiallyKnownSize(tileEdgeLength, tileEdgeLength),
						stream,
						bitDepth,
						palette,
						CancellationToken.None,
						progress);
					var tilePath = Path.Combine(level0Path, $"{x}", $"{y}.png");
					Utilities.SaveImage(tilePath, tile);
					savedTiles += 1;
				}
			}

			var nextZoomLevel = 0;
			var zoomLevelProgress = new Progress<string>();
			zoomLevelProgress.ProgressChanged += (sender, e) =>
			{
				Console.WriteLine(e);
			};

			while (ZoomLevelGenerator.TryCombineImagesForNextZoomLevel(
				       Path.Combine(outputFolderPath, nextZoomLevel.ToString()),
				       outputFolderPath,
				       nextZoomLevel + 1,
				       zoomLevelProgress))
			{
				nextZoomLevel += 1;
			}
		}

		private static BitDepthAndColorMode GetBitDepthAndColorMode(int bitDepth, string colorMode) =>
			bitDepth switch
			{
				1 => BitDepthAndColorMode.Grayscale_1BPP,
				2 => BitDepthAndColorMode.Grayscale_2BPP,
				4 => colorMode switch
				{
					"grayscale" => BitDepthAndColorMode.Grayscale_4BPP,
					"rgb" => BitDepthAndColorMode.RGB121_4BPP,
					"rgba" => BitDepthAndColorMode.RGBA1111_4BPP,
					_ => throw new ArgumentException($"Unsupported color mode {colorMode} for 4 bit-per-pixel images.")
				},
				8 => colorMode switch
				{
					"grayscale" => BitDepthAndColorMode.Grayscale_8BPP,
					"rgb" => BitDepthAndColorMode.RGB332_8BPP,
					"rgba" => BitDepthAndColorMode.RGBA2222_8BPP,
					_ => throw new ArgumentException($"Unsupported color mode {colorMode} for 8 bit-per-pixel images.")
				},
				16 => colorMode switch
				{
					"grayscale" => BitDepthAndColorMode.RGB565_16BPP,
					"rgb" => BitDepthAndColorMode.RGB565_16BPP,
					"rgba" => BitDepthAndColorMode.RGBA4444_16BPP,
					_ => throw new ArgumentException($"Unsupported color mode {colorMode} for 16 bit-per-pixel images.")
				},
				24 => colorMode switch
				{
					"grayscale" => BitDepthAndColorMode.RGB888_24BPP,
					"rgb" => BitDepthAndColorMode.RGB888_24BPP,
					"rgba" => BitDepthAndColorMode.RGBA6666_24BPP,
					_ => throw new ArgumentException($"Unsupported color mode {colorMode} for 24 bit-per-pixel images.")
				},
				32 => BitDepthAndColorMode.RGBA8888_32BPP,
				_ => throw new ArgumentException($"Unsupported bit depth {bitDepth}.")
			};

		private static ColorMode GetImagingColorMode(BitDepthAndColorMode bitDepthAndColorMode) =>
			bitDepthAndColorMode switch
			{
				BitDepthAndColorMode.Grayscale_1BPP => ColorMode.Grayscale,
				BitDepthAndColorMode.Grayscale_2BPP => ColorMode.Grayscale,
				BitDepthAndColorMode.Grayscale_4BPP => ColorMode.Grayscale,
				BitDepthAndColorMode.RGB121_4BPP => ColorMode.Rgb,
				BitDepthAndColorMode.RGBA1111_4BPP => ColorMode.Argb,
				BitDepthAndColorMode.Grayscale_8BPP => ColorMode.Grayscale,
				BitDepthAndColorMode.RGB332_8BPP => ColorMode.Rgb,
				BitDepthAndColorMode.RGBA2222_8BPP => ColorMode.Argb,
				BitDepthAndColorMode.RGB565_16BPP => ColorMode.Rgb,
				BitDepthAndColorMode.RGBA4444_16BPP => ColorMode.Argb,
				BitDepthAndColorMode.RGB888_24BPP => ColorMode.Rgb,
				BitDepthAndColorMode.RGBA6666_24BPP => ColorMode.Argb,
				BitDepthAndColorMode.RGBA8888_32BPP => ColorMode.Argb,
				_ => throw new ArgumentException($"Unsupported bit depth and color mode {bitDepthAndColorMode}.")
			};

		private static Size GetSizeFromCount(long count)
		{
			var squareRoot = (long)Math.Sqrt(count);
			Size result;

			if (IsPerfectSquare(count)) { result = new Size((int)squareRoot, (int)squareRoot); }
			else
			{
				long height = squareRoot;
				long remainder = count - squareRoot * squareRoot;
				height += (int)Math.Ceiling((double)remainder / squareRoot);

				result = new Size((int)squareRoot, (int)height);
			}

			return result;
		}

		private static bool IsPerfectSquare(long n)
		{
			if (n < 1) { return false; }

			long squareRoot = (long)Math.Sqrt(n);

			return squareRoot * squareRoot == n;
		}

		private static void SetColorOnColorSpaceImage(int color, Image<Rgba32> spaceImage, BitArray colorsSeen, int x,
			int y)
		{
			if (colorsSeen[color])
			{
				spaceImage[x, y] = new Rgba32(color >> 24, (color & 0xFF00) >> 8, color & 0xFF);
			}
		}
	}
}
