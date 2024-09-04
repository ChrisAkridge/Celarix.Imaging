using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Celarix.Imaging.Misc
{
	public static class ChromaPlaygroundHelpers
	{
		private static byte ToY(byte r, byte g, byte b) => (byte)((0.299 * r) + (0.587 * g) + (0.114 * b));
		private static byte ToCb(byte r, byte g, byte b) => (byte)((128 - (0.168736 * r) - (0.331264 * g)) + (0.5 * b));
		private static byte ToCr(byte r, byte g, byte b) => (byte)((128 + (0.5 * r)) - (0.418688 * g) - (0.081312 * b));

		public static void MutateImage(Image<Rgba32> original, Func<Rgba32, Rgba32> transform)
		{
			for (int y = 0; y < original.Height; y++)
			{
				for (int x = 0; x < original.Width; x++)
				{
					original[x, y] = transform(original[x, y]);
				}
			}
		}
		
		public static Rgba32 GetColorChannel(Rgba32 pixel, ColorChannel channel)
		{
			switch (channel)
			{
				case ColorChannel.Red:
					return new Rgba32(pixel.R, 0, 0, 255);
				case ColorChannel.Green:
					return new Rgba32(0, pixel.G, 0, 255);
				case ColorChannel.Blue:
					return new Rgba32(0, 0, pixel.B, 255);
				case ColorChannel.Alpha:
					return new Rgba32(pixel.A, pixel.A, pixel.A, 255);
				case ColorChannel.Y:
					var y = ToY(pixel.R, pixel.G, pixel.B);
					return new Rgba32(y, y, y, 255);
				case ColorChannel.Cb:
					var cb = ToCb(pixel.R, pixel.G, pixel.B);
					// Map Cb to a blue - yellow gradient
					return new Rgba32(0, (byte)(255 - cb), cb, 255);
				case ColorChannel.Cr:
					var cr = ToCr(pixel.R, pixel.G, pixel.B);
					// Map Cr to a red-cyan gradient
					return new Rgba32(cr, (byte)(255 - cr), (byte)(255 - cr), 255);
				case ColorChannel.CbPlusCr:
					var cb2 = ToCb(pixel.R, pixel.G, pixel.B);
					var cr2 = ToCr(pixel.R, pixel.G, pixel.B);
					// Map Cb to a blue - yellow gradient
					// Map Cr to a red-cyan gradient
					return new Rgba32(cr2, (byte)(255 - cb2), (byte)(255 - cr2), 255);
				case ColorChannel.Hue:
					var hsv = new HSV(pixel)
					{
						// Map the pixel to the hue wheel at S = V = 1
						S = 1f,
						V = 1f
					};
					return hsv.ToRgba32();
				case ColorChannel.Saturation:
					var hsv2 = new HSV(pixel);
					return new Rgba32(ClampToByte(hsv2.S * 256f), ClampToByte(hsv2.S * 256f), ClampToByte(hsv2.S * 256f), 255);
				case ColorChannel.Value:
					var hsv3 = new HSV(pixel);
					return new Rgba32(ClampToByte(hsv3.V * 256f), ClampToByte(hsv3.V * 256f), ClampToByte(hsv3.V * 256f), 255);
				default:
					throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
			}
		}

		public static Rgba32 GetBitPlane(Rgba32 pixel, ColorChannel channel, int bitPlaneIndex)
		{
			if (channel is not (ColorChannel.Red or ColorChannel.Green or ColorChannel.Blue or ColorChannel.Alpha))
			{
				throw new ArgumentException("Bit planes are only supported for RGB and Alpha channels.", nameof(channel));
			}
			
			var originalValue = channel switch
			{
				ColorChannel.Red => pixel.R,
				ColorChannel.Green => pixel.G,
				ColorChannel.Blue => pixel.B,
				ColorChannel.Alpha => pixel.A,
				_ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
			};
			
			var bitValue = (byte)((originalValue >> bitPlaneIndex) & 1);
			var newValue = (byte)(bitValue * 255);
			
			return channel switch
			{
				ColorChannel.Red => new Rgba32(newValue, 0, 0, 255),
				ColorChannel.Green => new Rgba32(0, newValue, 0, 255),
				ColorChannel.Blue => new Rgba32(0, 0, newValue, 255),
				ColorChannel.Alpha => new Rgba32(newValue, newValue, newValue, 255),
				_ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
			};
		}

		public static Image<Rgba32> ChromaSubsample(Image<Rgba32> image, ChromaSubsamplingMode mode)
		{
			if (mode == ChromaSubsamplingMode.YCbCr444)
			{
				return image.Clone();
			}
			
			var luminanceFullImage = new Image<Rgba32>(image.Width, image.Height);
			var cbFullImage = new Image<Rgba32>(image.Width, image.Height);
			var crFullImage = new Image<Rgba32>(image.Width, image.Height);

			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					luminanceFullImage[x, y] = GetColorChannel(image[x, y], ColorChannel.Y);
					cbFullImage[x, y] = GetColorChannel(image[x, y], ColorChannel.Cb);
					crFullImage[x, y] = GetColorChannel(image[x, y], ColorChannel.Cr);
				}
			}
			
			var chrominanceScaleFactorX = mode switch
			{
				ChromaSubsamplingMode.YCbCr422 => 2,
				ChromaSubsamplingMode.YCbCr420 => 2,
				ChromaSubsamplingMode.YCbCr411 => 4,
				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
			};
			
			var chrominanceScaleFactorY = mode switch
			{
				ChromaSubsamplingMode.YCbCr422 => 1,
				ChromaSubsamplingMode.YCbCr420 => 2,
				ChromaSubsamplingMode.YCbCr411 => 1,
				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
			};
			var resampler = new BicubicResampler();
			
			// Shrink the chrominance down and then expand it back up
			cbFullImage.Mutate(ctx => ctx.Resize(image.Width / chrominanceScaleFactorX, image.Height / chrominanceScaleFactorY, resampler));
			crFullImage.Mutate(ctx => ctx.Resize(image.Width / chrominanceScaleFactorX, image.Height / chrominanceScaleFactorY, resampler));
			cbFullImage.Mutate(ctx => ctx.Resize(image.Width, image.Height, resampler));
			crFullImage.Mutate(ctx => ctx.Resize(image.Width, image.Height, resampler));
			
			// Combine the luminance and chrominance images by overwriting the luminance image
			// What we have in cbFullImage and crFullImage is the Cb and Cr channels, but they're
			// being stored in RGB format. We need to convert them back to YCbCr format in the loop.
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					var luminancePixel = luminanceFullImage[x, y];
					// We can just grab any of the RGB components of the luminance pixel to use as the Y channel.
					var luminance = luminancePixel.R;
					
					// We need to convert the Cb and Cr channels back to YCbCr format. But we can't just use
					// ToCb and ToCr because those functions expect RGB values. We need to use the RGB values
					// and convert them back into Cb and Cr.
					var cbPixel = cbFullImage[x, y];
					var crPixel = crFullImage[x, y];
					
					// So, basically, we need to reverse the process we used to convert RGB to YCbCr.
					// The formula for RGB to Cb is Cb = 128 - (0.168736 * R) - (0.331264 * G) + (0.5 * B)
					// And we store that in RGB as R = 0, G = 255 - Cb, B = Cb, so Cb is just cbPixel.B.
					var cb = cbPixel.B;
					
					// Cr = 128 + (0.5 * R) - (0.418688 * G) - (0.081312 * B)
					// And we store that as R = Cr, G = 255 - Cr, B = 255 - Cr, so Cr is just crPixel.R.
					var cr = crPixel.R;

					// Now we need to convert the full YCbCr back into RGB.
					var r = ClampToByte(luminance + (1.402 * (cr - 128)));
					var g = ClampToByte(luminance - (0.344136 * (cb - 128)) - (0.714136 * (cr - 128)));
					var b = ClampToByte(luminance + (1.772 * (cb - 128)));
					luminanceFullImage[x, y] = new Rgba32(r, g, b, 255);
				}
			}
			
			crFullImage.Dispose();
			cbFullImage.Dispose();
			return luminanceFullImage;
		}

		public static void MutateReduceBitDepthTopColors(Image<Rgba32> image, int bitsPerPixel)
		{
			if (bitsPerPixel is not (16 or 8 or 4 or 2 or 1))
			{
				throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel, "Bits per pixel must be 1, 2, 4, 8, or 16.");
			}
			
			var topColors = GetTopNColorsWithoutAlpha(image, 1 << bitsPerPixel);
			
			if (topColors.Count < 1 << bitsPerPixel)
			{
				// If we already have fewer colors than we need, we don't need to do anything.
				return;
			}
			
			var topColorOctree = new ColorSpaceOctreeNode(0);

			foreach (var topColor in topColors)
			{
				topColorOctree.AddColor(topColor);
			}
			
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					var pixel = image[x, y];
					var pixelInt = (pixel.R << 16) | (pixel.G << 8) | pixel.B;
					if (!topColors.Contains(pixelInt))
					{
						// If the color is already in the top colors, no need to change it.
						// If it's not, we need to find the closest color in the top colors.
						var closestColor = topColorOctree.GetClosestColor(pixelInt);

						if (closestColor == null) { throw new InvalidOperationException("No closest color was found."); }
						var closestColorPixel = IntToPixel(closestColor.Value);
						image[x, y] = closestColorPixel;
					}
				}
			}
		}
		
		private static byte ClampToByte(double value) => (byte)Math.Max(0, Math.Min(255, value));

		private static HashSet<int> GetTopNColorsWithoutAlpha(Image<Rgba32> image, int maximumColors)
		{
			var colorCounts = new Dictionary<int, int>();
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					var pixel = image[x, y];
					var pixelInt = PixelToInt(pixel);
					if (!colorCounts.TryAdd(pixelInt, 1))
					{
						colorCounts[pixelInt]++;
					}
				}
			}

			var topColors = new HashSet<int>(colorCounts
				.OrderByDescending(kvp => kvp.Value)
				.Take(maximumColors)
				.Select(kvp => kvp.Key));

			return topColors;
		}

		private static int PixelToInt(Rgba32 pixel) => (pixel.R << 16) | (pixel.G << 8) | pixel.B;
		private static Rgba32 IntToPixel(int value) => new Rgba32((byte)(value >> 16), (byte)(value >> 8), (byte)value, 255);
	}
}
