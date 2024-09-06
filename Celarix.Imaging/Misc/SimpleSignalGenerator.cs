using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.Misc
{
	public static class SimpleSignalGenerator
	{
		private const int SampleRate = 48000;
		private const int PixelsPerSecond = 500;
		private const int SamplesPerPixel = SampleRate / PixelsPerSecond;
		private const int RedChannelFrequency = 500;
		private const int GreenChannelFrequency = 1000;
		private const int BlueChannelFrequency = 1500;
		private const int HBlankSampleCount = 100;

		public static string GetImageStatsString(Image<Rgba32> image)
		{
			var scanlineVisiblePartSampleCount = image.Width * SamplesPerPixel;
			var stats = new StringBuilder();

			stats.AppendLine($"Image dimensions: {image.Width}x{image.Height}");
			stats.AppendLine(
				$"(Simple) Visible part of scanline: {TimeSpan.FromSeconds((double)scanlineVisiblePartSampleCount / SampleRate)}");

			return stats.ToString();
		}

		public static void GenerateSimpleSignal(Image<Rgba32> image, string outputPath)
		{
			var scanlineVisiblePartSampleCount = image.Width * SamplesPerPixel;
			
			using var output = new BinaryWriter(File.OpenWrite(outputPath));
			var scanlineBuffer = new float[scanlineVisiblePartSampleCount];

			for (int scanline = 0; scanline < image.Height; scanline++)
			{
				for (var sample = 0; sample < scanlineVisiblePartSampleCount; sample++)
				{
					var x = sample / SamplesPerPixel;
					var pixel = image[x, scanline];
					var red = pixel.R / 255f;
					var green = pixel.G / 255f;
					var blue = pixel.B / 255f;
					
					// The red channel is a 500Hz sine wave multiplied by red.
					scanlineBuffer[sample] = (float)(Math.Sin((2 * Math.PI * RedChannelFrequency * sample) / SampleRate) * red);
					
					// Add the green channel, a 1 kHz sine wave multiplied by green.
					scanlineBuffer[sample] += (float)(Math.Sin((2 * Math.PI * GreenChannelFrequency * sample) / SampleRate) * green);
					
					// Add the blue channel, a 1.5 kHz sine wave multiplied by blue.
					scanlineBuffer[sample] += (float)(Math.Sin((2 * Math.PI * BlueChannelFrequency * sample) / SampleRate) * blue);
					
					// Add a DC bias of 0.1 to fit the signal into the -0.9 to +1.1 range.
					scanlineBuffer[sample] += 0.1f;
					
					// Multiply the signal by 0.9 to fit the signal into the -0.9 to +1.1 range.
					scanlineBuffer[sample] *= 0.9f;
				}
				
				// Write the scanline to the output.
				foreach (var sample in scanlineBuffer)
				{
					output.Write(sample);
				}
				
				// Write 100 samples of -1 for the HBlank.
				for (int i = 0; i < HBlankSampleCount; i++)
				{
					output.Write(-1f);
				}
			}
		}
	}
}
