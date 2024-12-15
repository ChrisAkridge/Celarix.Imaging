using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celarix.Imaging.Misc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.ByteViewCLI
{
	internal sealed class ChromaPlaygroundService
	{
		public void RunExtractColorChannel(string inputPath, string outputPath, ColorChannel channel,
			string? ffmpegPath)
		{
			if (ffmpegPath == null)
			{
				Console.WriteLine($"Extracting {Enum.GetName(channel)!.ToLowerInvariant()} channel from image {inputPath}...");
				ExtractColorChannel(inputPath, outputPath, channel);
			}
			else
			{
				Console.WriteLine($"Extracting {Enum.GetName(channel)!.ToLowerInvariant()} channel from frames in video {inputPath}...");
				FFMPEGService.RunActionOnVideo(f => ExtractColorChannel(f, f, channel),
					ffmpegPath, inputPath, outputPath);
			}
		}
		
		public void RunExtractBitPlane(string inputPath, string outputPath, string channel, int bit,
			string? ffmpegPath)
		{
			string loggingChannel = channel.Equals("all", StringComparison.InvariantCultureIgnoreCase)
				? "all channels"
				: $"{channel} channel";
			
			if (ffmpegPath == null)
			{
				Console.WriteLine($"Extracting bit plane {bit} on {loggingChannel} from image {inputPath}...");
				ExtractBitPlane(inputPath, outputPath, channel, bit);
			}
			else
			{
				Console.WriteLine($"Extracting bit plane {bit} on {loggingChannel} from frames in video {inputPath}...");
				FFMPEGService.RunActionOnVideo(f => ExtractBitPlane(f, f, channel, bit),
					ffmpegPath, inputPath, outputPath);
			}
		}

		public void RunChromaSubsample(string inputPath, string outputPath, string subsamplingMode,
			string? ffmpegPath)
		{
			if (ffmpegPath == null)
			{
				Console.WriteLine($"Applying {subsamplingMode} subsampling to image {inputPath}...");
				ChromaSubsample(inputPath, outputPath, subsamplingMode);
			}
			else
			{
				Console.WriteLine($"Applying {subsamplingMode} subsampling to frames in video {inputPath}...");
				FFMPEGService.RunActionOnVideo(f => ChromaSubsample(f, f, subsamplingMode),
					ffmpegPath, inputPath, outputPath);
			}
		}

		public void RunReduceBitDepth(string inputPath, string outputPath, int bitDepth, string colorMode,
			string? ffmpegPath)
		{
			if (ffmpegPath == null)
			{
				Console.WriteLine($"Reducing bit depth to {bitDepth} bits in {colorMode} mode for image {inputPath}...");
				ReduceBitDepth(inputPath, outputPath, bitDepth, colorMode);
			}
			else
			{
				Console.WriteLine($"Reducing bit depth to {bitDepth} bits in {colorMode} mode for frames in video {inputPath}...");
				FFMPEGService.RunActionOnVideo(f => ReduceBitDepth(f, f, bitDepth, colorMode),
					ffmpegPath, inputPath, outputPath);
			}
		}

		public void RunExportMockNTSCSignal(string inputPath, string outputPath)
		{
			Console.WriteLine($"Generating mock NTSC signal for {inputPath}");
			var image = Image.Load<Rgba32>(inputPath);
			var stats = MockNTSCSignalGenerator.GetImageStatsString(image);
			Console.WriteLine(stats);
			MockNTSCSignalGenerator.GenerateMockNTSCSignal(image, outputPath);
		}
		
		public void RunExportSimpleSignal(string inputPath, string outputPath)
		{
			Console.WriteLine($"Generating simple signal for {inputPath}");
			var image = Image.Load<Rgba32>(inputPath);
			var stats = SimpleSignalGenerator.GetImageStatsString(image);
			Console.WriteLine(stats);
			SimpleSignalGenerator.GenerateSimpleSignal(image, outputPath);
		}

		private static void ExtractColorChannel(string inputPath, string outputPath, ColorChannel channel)
		{
			var image = Image.Load<Rgba32>(inputPath);
			ChromaPlaygroundHelpers.MutateImage(image,
				p => ChromaPlaygroundHelpers.GetColorChannel(p, channel));

			if (inputPath == outputPath)
			{
				File.Delete(inputPath);
			}
			image.SaveAsPng(outputPath);
		}

		private static void ExtractBitPlane(string inputPath, string outputPath, string channel, int bit)
		{
			var image = Image.Load<Rgba32>(inputPath);

			Func<Rgba32, Rgba32> transform;
			ColorChannel? imagingChannel = null;
			if (channel.Equals("all", StringComparison.InvariantCultureIgnoreCase))
			{
				transform = p => ChromaPlaygroundHelpers.GetRGBBitPlane(p, bit);
			}
			else
			{
				imagingChannel = channel.ToLowerInvariant() switch
				{
					"red" => ColorChannel.Red,
					"green" => ColorChannel.Green,
					"blue" => ColorChannel.Blue,
					_ => throw new ArgumentException("Invalid color channel for bit plane extraction.")
				};
				transform = p => ChromaPlaygroundHelpers.GetBitPlane(p, imagingChannel.Value, bit);
			}
			
			ChromaPlaygroundHelpers.MutateImage(image, transform);
			
			if (inputPath == outputPath)
			{
				File.Delete(inputPath);
			}
			image.SaveAsPng(outputPath);
		}
		
		private static void ChromaSubsample(string inputPath, string outputPath, string subsamplingMode)
		{
			var image = Image.Load<Rgba32>(inputPath);

			ChromaPlaygroundHelpers.ChromaSubsample(image, subsamplingMode switch
			{
				"4:2:2" => ChromaSubsamplingMode.YCbCr422,
				"4:2:0" => ChromaSubsamplingMode.YCbCr420,
				"4:1:1" => ChromaSubsamplingMode.YCbCr411,
				"8:1:1" => ChromaSubsamplingMode.YCbCr811,
				"16:1:1" => ChromaSubsamplingMode.YCbCr1611,
				"256:1:1" => ChromaSubsamplingMode.YCbCr25611,
				_ => throw new ArgumentException("Invalid subsampling mode.")
			});

			if (inputPath == outputPath)
			{
				File.Delete(inputPath);
			}
			image.SaveAsPng(outputPath);
		}
		
		private static void ReduceBitDepth(string inputPath, string outputPath, int bitDepth, string colorMode)
		{
			var image = Image.Load<Rgba32>(inputPath);

			if (colorMode.Equals("topn", StringComparison.InvariantCultureIgnoreCase))
			{
				ChromaPlaygroundHelpers.MutateReduceBitDepthTopColors(image, bitDepth);
			}
			else if (colorMode.Equals("grayscale", StringComparison.InvariantCultureIgnoreCase))
			{
				ReduceToGrayscale(image, bitDepth);
			}
			else if (colorMode.Equals("shift", StringComparison.InvariantCultureIgnoreCase))
			{
				ReduceByShifting(image, bitDepth);
			}

			if (inputPath == outputPath)
			{
				File.Delete(inputPath);
			}
			image.SaveAsPng(outputPath);
		}

		internal static void ReduceByShifting(Image<Rgba32> image, int bitDepth)
		{
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					var pixel = image[x, y];
					float red;
					float green;
					float blue;
						
					switch (bitDepth)
					{
						case 16:
							// RGB 5:6:5
							red = (pixel.R >> 3) / 31f;
							green = (pixel.G >> 2) / 63f;
							blue = (pixel.B >> 3) / 31f;

							break;
						case 8:
							// RGB 3:3:2
							red = (pixel.R >> 5) / 7f;
							green = (pixel.G >> 5) / 7f;
							blue = (pixel.B >> 6) / 3f;

							break;
						case 4:
							// RGB 1:2:1
							red = (pixel.R >> 4) / 15f;
							green = (pixel.G >> 5) / 7f;
							blue = (pixel.B >> 4) / 15f;

							break;
						default:
							throw new ArgumentException("Invalid bit depth for shift mode, must be at least 4.");
					}
						
					image[x, y] = new Rgba32((byte)(red * 255f),
						(byte)(green * 255f),
						(byte)(blue * 255f),
						pixel.A);
				}
			}
		}

		internal static void ReduceToGrayscale(Image<Rgba32> image, int bitDepth)
		{
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					var pixel = image[x, y];
					var luminance = (byte)Math.Round((pixel.R * 0.2126) + (pixel.G * 0.7152) + (pixel.B * 0.0722));
					luminance >>= 8 - bitDepth;
					image[x, y] = new Rgba32(luminance, luminance, luminance, pixel.A);
				}
			}
		}
	}
}
