using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.Misc
{
	public static class MockNTSCSignalGenerator
	{
		private const int SampleRate = 48000;
		private const int PixelsPerSecond = 500;
		private const int SamplesPerPixel = SampleRate / PixelsPerSecond;
		private const int CbCrCarrierFrequency = 2000;

		public static string GetImageStatsString(Image<Rgba32> image)
		{
			var scanlineVisiblePartSampleCount = image.Width * SamplesPerPixel;
			var hBlankSampleCount = (long)(scanlineVisiblePartSampleCount * 0.247d);
			var stats = new StringBuilder();
			
			stats.AppendLine($"Image dimensions: {image.Width}x{image.Height}");
			stats.AppendLine($"Visible part of scanline: {TimeSpan.FromSeconds((double)scanlineVisiblePartSampleCount / SampleRate)}");
			stats.AppendLine($"HBlank: {TimeSpan.FromSeconds((double)hBlankSampleCount / SampleRate)}");

			return stats.ToString();
		}
		
		public static void GenerateMockNTSCSignal(Image<Rgba32> image, string outputPath)
		{
			var scanlineVisiblePartSampleCount = image.Width * SamplesPerPixel;
			var hBlankSampleCount = (long)(scanlineVisiblePartSampleCount * 0.247d);
			var frontPorchSampleCount = (long)(hBlankSampleCount * 0.095d);
			var syncPulseSampleCount = (long)(hBlankSampleCount * 0.2995d);
			var backPorchPreColorburstSampleCount = (long)(hBlankSampleCount * 0.06055d);
			var colorburstSampleCount = (long)(hBlankSampleCount * 0.1778d);
			var backPorchPostColorburstSampleCount = (long)(hBlankSampleCount * 0.36715d);
			
			// Build one scanline at a time and write it to the output.
			// File format is raw, 48 kHz audio with single-precision floating-point samples.
			using var output = new BinaryWriter(File.OpenWrite(outputPath));

			var cbBuffer = new float[scanlineVisiblePartSampleCount];
			// Below buffer is used for Cr and for the combined Cb + Cr.
			var chromaBuffer = new float[scanlineVisiblePartSampleCount];
			// Below buffer is used for luma and for the combined luma + chroma.
			var scanlineBuffer = new float[scanlineVisiblePartSampleCount];
			var hBlankBuffer = new float[hBlankSampleCount];
			
			var imageYBuffer = new float[image.Width];
			var imageCbBuffer = new float[image.Width];
			var imageCrBuffer = new float[image.Width];

			for (int scanline = 0; scanline < image.Height; scanline++)
			{
				// Fill the image buffers.
				for (int x = 0; x < image.Width; x++)
				{
					var pixel = image[x, scanline];
					var rgb = new Rgb(pixel.R / 255f, pixel.G / 255f, pixel.B / 255f);
					var yCbCr = ColorSpaceConverter.ToYCbCr(rgb);
					imageYBuffer[x] = yCbCr.Y;
					imageCbBuffer[x] = yCbCr.Cb;
					imageCrBuffer[x] = yCbCr.Cr;
				}
				
				FillCbSignalBuffer(imageCbBuffer, cbBuffer);
				FillCrSignalBuffer(imageCrBuffer, chromaBuffer);
				MergeChromaBuffers(cbBuffer, chromaBuffer);
				FillYSignalBuffer(imageYBuffer, scanlineBuffer);
				MergeLumaAndChroma(scanlineBuffer, chromaBuffer);
				FillHBlank(hBlankBuffer,
					frontPorchSampleCount,
					syncPulseSampleCount,
					backPorchPreColorburstSampleCount,
					colorburstSampleCount,
					backPorchPostColorburstSampleCount);
				
				// Write the scanline to the stream.
				foreach (var sample in scanlineBuffer)
				{
					output.Write(sample);
				}
				
				foreach (var sample in hBlankBuffer)
				{
					output.Write(sample);
				}
			}
		}

		private static void FillCbSignalBuffer(float[] cbPixels, float[] cbSamples)
		{
			// Cb is modulated as a 2 kHz sine wave at a 48 kHz sample rate
			// Each 4 cycles of the Cb carrier correspond to one of the cb values
			// This is equal to 24 samples per cycle and 96 samples per cb value
			for (int x = 0; x < cbPixels.Length; x++)
			{
				for (int sampleIndex = 0; sampleIndex < SamplesPerPixel; sampleIndex++)
				{
					float fullAmplitudeCbCarrier = (float)Math.Sin((2 * Math.PI * CbCrCarrierFrequency * sampleIndex) / SampleRate);
					float cbNormalized = cbPixels[x] / 255f;
					cbSamples[(x * SamplesPerPixel) + sampleIndex] = fullAmplitudeCbCarrier * cbNormalized;
				}
			}
		}
		
		private static void FillCrSignalBuffer(float[] crPixels, float[] crSamples)
		{
			// Cr is modulated as a 2 kHz cosine wave at a 48 kHz sample rate
			// Each 4 cycles of the Cr carrier correspond to one of the cr values
			// This is equal to 24 samples per cycle and 96 samples per cr value
			for (int x = 0; x < crPixels.Length; x++)
			{
				for (int sampleIndex = 0; sampleIndex < SamplesPerPixel; sampleIndex++)
				{
					float fullAmplitudeCrCarrier = (float)Math.Cos((2 * Math.PI * CbCrCarrierFrequency * sampleIndex) / SampleRate);
					float crNormalized = crPixels[x] / 255f;
					crSamples[(x * SamplesPerPixel) + sampleIndex] = fullAmplitudeCrCarrier * crNormalized;
				}
			}
		}
		
		private static void MergeChromaBuffers(float[] cbSamples, float[] chromaSamples)
		{
			// Add the sine and cosine waves together, then normalize to +/-1, combining into
			// chromaSamples.
			for (int i = 0; i < cbSamples.Length; i++)
			{
				chromaSamples[i] = (cbSamples[i] + chromaSamples[i]) / 2f;
			}
		}

		private static void FillYSignalBuffer(float[] yPixels, float[] ySamples)
		{
			// Y is just a flat line at steps of 1.8/255 across the +/-0.9 range,
			// changing every 96 samples.
			for (int x = 0; x < yPixels.Length; x++)
			{
				for (int sampleIndex = 0; sampleIndex < SamplesPerPixel; sampleIndex++)
				{
					var yPixel = yPixels[x];
					yPixel /= (255f / 1.8f); // normalize to 0 to 1.8
					yPixel -= 0.9f; // shift to -0.9 to 0.9 to give room for the chroma signal
					ySamples[(x * SamplesPerPixel) + sampleIndex] = yPixel;
				}
			}
		}
		
		private static void MergeLumaAndChroma(float[] lumaSamples, float[] chromaSamples)
		{
			for (int i = 0; i < lumaSamples.Length; i++)
			{
				// Reduce the chroma signal to +/-0.1 and add it to luma.
				lumaSamples[i] += chromaSamples[i] / 10f;

				// Now "crush" the signal to fit between -0.6 to +1 so we have room for HBlank.
				// First, we need to shrink the range from 2 down to 1.6.
				lumaSamples[i] *= 1.6f;
				// Then, we need to add a DC offset to shift the range to -0.6 to +1.
				lumaSamples[i] += 0.4f;
			}
		}

		private static void FillHBlank(float[] hBlankBuffer, long frontPorchSampleCount, long syncPulseSampleCount,
			long backPorchPreColorburstSampleCount, long colorburstSampleCount,
			long backPorchPostColorburstSampleCount)
		{
			long hBlankSample = 0;
			long targetSample = frontPorchSampleCount;

			while (hBlankSample < targetSample)
			{
				hBlankBuffer[hBlankSample] = -0.6f;
				hBlankSample++;
			}

			targetSample += syncPulseSampleCount;

			while (hBlankSample < targetSample)
			{
				hBlankBuffer[hBlankSample] = -1f;
				hBlankSample++;
			}

			targetSample += backPorchPreColorburstSampleCount;

			while (hBlankSample < targetSample)
			{
				hBlankBuffer[hBlankSample] = -0.6f;
				hBlankSample++;
			}

			targetSample += colorburstSampleCount;

			while (hBlankSample < targetSample)
			{
				// Color burst is a 2 kHz sine wave.
				hBlankBuffer[hBlankSample] = (float)Math.Sin((2 * Math.PI * 2000 * hBlankSample) / 48000);
				hBlankSample++;
			}

			targetSample += backPorchPostColorburstSampleCount;

			while (hBlankSample < targetSample)
			{
				hBlankBuffer[hBlankSample] = -0.6f;
				hBlankSample++;
			}
		}
	}
}
