using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Celarix.Imaging.Misc
{
	public static class ChromaPlaygroundHelpers
	{
		private static ColorSpaceConverter colorSpaceConverter = new ColorSpaceConverter();
		
		private static byte ToY(byte r, byte g, byte b)
		{
			var rgb = new Rgb(r / 255f, g / 255f, b / 255f);
			var yCbCr = ColorSpaceConverter.ToYCbCr(rgb);

			return ClampToByte(yCbCr.Y);
		}

		private static byte ToCb(byte r, byte g, byte b)
		{
			var rgb = new Rgb(r / 255f, g / 255f, b / 255f);
			var yCbCr = ColorSpaceConverter.ToYCbCr(rgb);

			return ClampToByte(yCbCr.Cb);
		}
		
		private static byte ToCr(byte r, byte g, byte b)
		{
			var rgb = new Rgb(r / 255f, g / 255f, b / 255f);
			var yCbCr = ColorSpaceConverter.ToYCbCr(rgb);

			return ClampToByte(yCbCr.Cr);
		}

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
					var rgb = new Rgb(pixel.R / 255f, pixel.G / 255f, pixel.B / 255f);
					var yCbCr = ColorSpaceConverter.ToYCbCr(rgb);
					var chrominanceOnly = new YCbCr(127.5f, yCbCr.Cb, yCbCr.Cr);

					return new Rgba32(colorSpaceConverter.ToRgb(chrominanceOnly).ToVector3());
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
				case ColorChannel.OklabL:
				case ColorChannel.OklabA:
				case ColorChannel.OklabB:
				case ColorChannel.OklabChroma:
				case ColorChannel.OklabHue:
					return GetOklabChannel(pixel, channel);
				case ColorChannel.YiqI:
				case ColorChannel.YiqQ:
					return GetYiqChannel(pixel, channel);
				case ColorChannel.LinearRed:
				case ColorChannel.LinearGreen:
				case ColorChannel.LinearBlue:
					return GetLinearChannel(pixel, channel);
				case ColorChannel.Cyan:
				case ColorChannel.Magenta:
				case ColorChannel.Yellow:
				case ColorChannel.Black:
					return GetCmykChannel(pixel, channel);
				default:
					throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
			}
		}

		// Extremes of a and b for colors inside the sRGB gamut are about -0.23..0.28 and -0.31..0.20, and the
		// largest chroma is about 0.32, so these ranges cover all of it.
		private const double OklabABRange = 0.32;
		private const double OklabMaxChroma = 0.33;

		/// <summary>
		/// Blends mid-gray toward a pole color by how far the axis value is from zero (relative to range),
		/// so that neutral colors show as gray.
		/// </summary>
		private static Rgba32 DivergingColor(double value, double range, Rgba32 negativePole, Rgba32 positivePole)
		{
			var t = Math.Min(1d, Math.Abs(value) / range);
			var pole = value < 0 ? negativePole : positivePole;

			return new Rgba32(ClampToByte(Math.Round(128 + (t * (pole.R - 128)))),
				ClampToByte(Math.Round(128 + (t * (pole.G - 128)))),
				ClampToByte(Math.Round(128 + (t * (pole.B - 128)))), 255);
		}

		private static Rgba32 GetYiqChannel(Rgba32 pixel, ColorChannel channel)
		{
			var r = pixel.R / 255d;
			var g = pixel.G / 255d;
			var b = pixel.B / 255d;

			// NTSC's I axis runs orange to cyan-blue, and Q runs purple to green; their maximum magnitudes are 0.5957 and 0.5226.
			return channel == ColorChannel.YiqI
				? DivergingColor((0.595716 * r) - (0.274453 * g) - (0.321263 * b), 0.5957,
					new Rgba32(0, 160, 255, 255), new Rgba32(255, 128, 0, 255))
				: DivergingColor((0.211456 * r) - (0.522591 * g) + (0.311135 * b), 0.5226,
					new Rgba32(0, 200, 0, 255), new Rgba32(200, 0, 255, 255));
		}

		private static Rgba32 GetLinearChannel(Rgba32 pixel, ColorChannel channel)
		{
			byte Linear(byte value) => ClampToByte(Math.Round(SrgbToLinear(value) * 255d));

			return channel switch
			{
				ColorChannel.LinearRed => new Rgba32(Linear(pixel.R), 0, 0, 255),
				ColorChannel.LinearGreen => new Rgba32(0, Linear(pixel.G), 0, 255),
				_ => new Rgba32(0, 0, Linear(pixel.B), 255)
			};
		}

		/// <summary>
		/// Naive CMYK, with no ink profile: K = 1 - max(R, G, B) and C, M, Y are what remains after removing K.
		/// Each plate is drawn as ink on white paper, so more ink is darker and more saturated in that ink's color.
		/// </summary>
		private static Rgba32 GetCmykChannel(Rgba32 pixel, ColorChannel channel)
		{
			var r = pixel.R / 255d;
			var g = pixel.G / 255d;
			var b = pixel.B / 255d;
			var k = 1d - Math.Max(r, Math.Max(g, b));

			// For pure black, K is 1 and the other plates are 0.
			double Plate(double component) => k >= 1d ? 0d : (1d - component - k) / (1d - k);
			byte Paper(double ink) => ClampToByte(Math.Round((1d - ink) * 255d));

			return channel switch
			{
				ColorChannel.Cyan => new Rgba32(Paper(Plate(r)), 255, 255, 255),
				ColorChannel.Magenta => new Rgba32(255, Paper(Plate(g)), 255, 255),
				ColorChannel.Yellow => new Rgba32(255, 255, Paper(Plate(b)), 255),
				_ => new Rgba32(Paper(k), Paper(k), Paper(k), 255)
			};
		}

		private static Rgba32 GetOklabChannel(Rgba32 pixel, ColorChannel channel)
		{
			var (l, a, b) = RgbToOklab(pixel.R, pixel.G, pixel.B);

			static Rgba32 Diverging(double value, Rgba32 negativePole, Rgba32 positivePole) =>
				DivergingColor(value, OklabABRange, negativePole, positivePole);

			switch (channel)
			{
				case ColorChannel.OklabL:
				{
					var gray = ClampToByte(Math.Round(l * 255d));
					return new Rgba32(gray, gray, gray, 255);
				}
				case ColorChannel.OklabA:
					// Green - red axis
					return Diverging(a, new Rgba32(0, 255, 0, 255), new Rgba32(255, 0, 0, 255));
				case ColorChannel.OklabB:
					// Blue - yellow axis
					return Diverging(b, new Rgba32(0, 0, 255, 255), new Rgba32(255, 255, 0, 255));
				case ColorChannel.OklabChroma:
				{
					var gray = ClampToByte(Math.Round(Math.Sqrt((a * a) + (b * b)) / OklabMaxChroma * 255d));
					return new Rgba32(gray, gray, gray, 255);
				}
				case ColorChannel.OklabHue:
				{
					// Map the pixel to the hue wheel at a fixed lightness and chroma, which stays mostly in gamut.
					var hue = Math.Atan2(b, a);
					return OklabToRgba32((0.75, 0.12 * Math.Cos(hue), 0.12 * Math.Sin(hue)));
				}
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
		
		public static Rgba32 GetRGBBitPlane(Rgba32 pixel, int bitPlaneIndex)
		{
			var r = (byte)((pixel.R >> bitPlaneIndex) & 1);
			var g = (byte)((pixel.G >> bitPlaneIndex) & 1);
			var b = (byte)((pixel.B >> bitPlaneIndex) & 1);
			var newR = (byte)(r * 255);
			var newG = (byte)(g * 255);
			var newB = (byte)(b * 255);
			
			return new Rgba32(newR, newG, newB, 255);
		}

		public static Image<Rgba32> ChromaSubsample(Image<Rgba32> image, ChromaSubsamplingMode mode) =>
			Subsample(image, mode, subsampleLuma: false);

		/// <summary>
		/// The reverse of chroma subsampling: color stays at full resolution while luminance is stored in blocks
		/// of the size the mode describes. Nothing does this on purpose.
		/// </summary>
		public static Image<Rgba32> LumaSubsample(Image<Rgba32> image, ChromaSubsamplingMode mode) =>
			Subsample(image, mode, subsampleLuma: true);

		private static Image<Rgba32> Subsample(Image<Rgba32> image, ChromaSubsamplingMode mode, bool subsampleLuma)
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
				ChromaSubsamplingMode.YCbCr811 => 8,
				ChromaSubsamplingMode.YCbCr1611 => 16,
				ChromaSubsamplingMode.YCbCr25611 => 256,
				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
			};
			
			var chrominanceScaleFactorY = mode switch
			{
				ChromaSubsamplingMode.YCbCr422 => 1,
				ChromaSubsamplingMode.YCbCr420 => 2,
				ChromaSubsamplingMode.YCbCr411 => 1,
				ChromaSubsamplingMode.YCbCr811 => 8,
				ChromaSubsamplingMode.YCbCr1611 => 16,
				ChromaSubsamplingMode.YCbCr25611 => 256,
				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
			};
			var resampler = new BicubicResampler();
			
			// Shrink the subsampled channels down and then expand them back up. (A size of 0 would mean
			// "work it out from the other dimension" to ImageSharp, so tiny images are kept at least 1 pixel.)
			var reducedWidth = Math.Max(1, image.Width / chrominanceScaleFactorX);
			var reducedHeight = Math.Max(1, image.Height / chrominanceScaleFactorY);
			var subsampledChannels = subsampleLuma
				? new[] { luminanceFullImage }
				: new[] { cbFullImage, crFullImage };

			foreach (var channelImage in subsampledChannels)
			{
				channelImage.Mutate(ctx => ctx.Resize(reducedWidth, reducedHeight, resampler));
				channelImage.Mutate(ctx => ctx.Resize(image.Width, image.Height, resampler));
			}
			
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

		/// <summary>
		/// Reduces the image to a fixed RGB layout by discarding low bits: 16bpp is 5:6:5, 8bpp is 3:3:2, 4bpp is 1:2:1.
		/// </summary>
		public static void MutateReduceBitDepthByShifting(Image<Rgba32> image, int bitsPerPixel, bool dither = false)
		{
			if (bitsPerPixel is not (16 or 8 or 4))
			{
				throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel, "Bits per pixel must be 4, 8, or 16.");
			}

			ApplyQuantizer(image, pixel =>
			{
				float red;
				float green;
				float blue;

				switch (bitsPerPixel)
				{
					case 16:
						red = (pixel.R >> 3) / 31f;
						green = (pixel.G >> 2) / 63f;
						blue = (pixel.B >> 3) / 31f;
						break;
					case 8:
						red = (pixel.R >> 5) / 7f;
						green = (pixel.G >> 5) / 7f;
						blue = (pixel.B >> 6) / 3f;
						break;
					default:
						red = (pixel.R >> 7) != 0 ? 1f : 0f;
						green = (pixel.G >> 6) / 3f;
						blue = (pixel.B >> 7) != 0 ? 1f : 0f;
						break;
				}

				return new Rgba32((byte)(red * 255f), (byte)(green * 255f), (byte)(blue * 255f));
			}, dither);
		}

		/// <summary>
		/// Converts the image to grayscale (via Y) and quantizes it to 2^bitsPerPixel evenly spaced levels.
		/// </summary>
		public static void MutateReduceBitDepthGrayscale(Image<Rgba32> image, int bitsPerPixel, bool dither = false)
		{
			if (bitsPerPixel is not (8 or 4 or 2 or 1))
			{
				throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel, "Grayscale bit depths must be 8, 4, 2, or 1.");
			}

			var maxLevel = (1 << bitsPerPixel) - 1;

			// Go to full grayscale first so that dithering only has to diffuse a single channel's error.
			MutateImage(image, pixel =>
			{
				var luma = ToY(pixel.R, pixel.G, pixel.B);
				return new Rgba32(luma, luma, luma, 255);
			});

			ApplyQuantizer(image, pixel =>
			{
				var level = pixel.R >> (8 - bitsPerPixel);
				var value = (byte)(level * 255 / maxLevel);

				return new Rgba32(value, value, value, 255);
			}, dither);
		}

		/// <summary>
		/// Reduces the image to a palette of 2^bitsPerPixel colors chosen by k-means clustering in Oklab, so that
		/// the palette minimizes perceptual error across all pixels rather than favoring the most frequent colors.
		/// </summary>
		public static void MutateReduceBitDepthKMeans(Image<Rgba32> image, int bitsPerPixel, bool dither = false)
		{
			if (bitsPerPixel is not (8 or 4 or 2 or 1))
			{
				throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel, "Bits per pixel must be 1, 2, 4, or 8.");
			}

			var k = 1 << bitsPerPixel;

			// Histogram at 5 bits per channel so the clustering works on at most 32,768 weighted points.
			const int binCount = 1 << 15;
			var counts = new long[binCount];
			var sums = new long[binCount * 3];

			static int BinOf(Rgba32 p) => ((p.R >> 3) << 10) | ((p.G >> 3) << 5) | (p.B >> 3);

			MutateImage(image, pixel =>
			{
				var bin = BinOf(pixel);
				counts[bin]++;
				sums[bin * 3] += pixel.R;
				sums[bin * 3 + 1] += pixel.G;
				sums[bin * 3 + 2] += pixel.B;
				return pixel;
			});

			var pointBins = new List<int>();
			for (int bin = 0; bin < binCount; bin++)
			{
				if (counts[bin] > 0) { pointBins.Add(bin); }
			}

			var pointCount = pointBins.Count;
			var points = new (double L, double A, double B)[pointCount];
			var weights = new double[pointCount];
			for (int i = 0; i < pointCount; i++)
			{
				var bin = pointBins[i];
				var n = counts[bin];
				points[i] = RgbToOklab(sums[bin * 3] / (double)n, sums[bin * 3 + 1] / (double)n, sums[bin * 3 + 2] / (double)n);
				weights[i] = n;
			}

			var centroids = InitializeCentroidsKMeansPlusPlus(points, weights, Math.Min(k, pointCount));
			var assignment = new int[pointCount];

			for (int iteration = 0; iteration < 12; iteration++)
			{
				var changed = AssignToNearest(points, centroids, assignment);

				var sumL = new double[centroids.Length];
				var sumA = new double[centroids.Length];
				var sumB = new double[centroids.Length];
				var weightSum = new double[centroids.Length];
				for (int i = 0; i < pointCount; i++)
				{
					var c = assignment[i];
					sumL[c] += points[i].L * weights[i];
					sumA[c] += points[i].A * weights[i];
					sumB[c] += points[i].B * weights[i];
					weightSum[c] += weights[i];
				}

				for (int c = 0; c < centroids.Length; c++)
				{
					// An empty cluster keeps its previous centroid.
					if (weightSum[c] > 0)
					{
						centroids[c] = (sumL[c] / weightSum[c], sumA[c] / weightSum[c], sumB[c] / weightSum[c]);
					}
				}

				if (!changed && iteration > 0) { break; }
			}

			AssignToNearest(points, centroids, assignment);

			var palette = new Rgba32[centroids.Length];
			for (int c = 0; c < palette.Length; c++)
			{
				palette[c] = OklabToRgba32(centroids[c]);
			}

			if (dither)
			{
				// Dithered pixels carry diffused error, so they need an exact nearest-color search
				// rather than the per-bin lookup used below.
				var paletteLab = new (double L, double A, double B)[palette.Length];
				for (int c = 0; c < palette.Length; c++)
				{
					paletteLab[c] = RgbToOklab(palette[c].R, palette[c].G, palette[c].B);
				}

				DitherFloydSteinberg(image, pixel =>
				{
					var lab = RgbToOklab(pixel.R, pixel.G, pixel.B);
					var best = 0;
					var bestDistance = double.MaxValue;
					for (int c = 0; c < paletteLab.Length; c++)
					{
						var distance = DistanceSquared(lab, paletteLab[c]);
						if (distance < bestDistance)
						{
							bestDistance = distance;
							best = c;
						}
					}
					return palette[best];
				});
				return;
			}

			var paletteByBin = new Rgba32[binCount];
			for (int i = 0; i < pointCount; i++)
			{
				paletteByBin[pointBins[i]] = palette[assignment[i]];
			}

			MutateImage(image, pixel => paletteByBin[BinOf(pixel)]);
		}

		/// <summary>
		/// Applies a color-to-color quantizer to every pixel, optionally diffusing each pixel's quantization
		/// error onto its neighbors (Floyd-Steinberg) so that flat regions become patterns instead of bands.
		/// </summary>
		private static void ApplyQuantizer(Image<Rgba32> image, Func<Rgba32, Rgba32> quantize, bool dither)
		{
			if (dither)
			{
				DitherFloydSteinberg(image, quantize);
			}
			else
			{
				MutateImage(image, quantize);
			}
		}

		private static void DitherFloydSteinberg(Image<Rgba32> image, Func<Rgba32, Rgba32> quantize)
		{
			// Each buffer holds accumulated (R, G, B) error for one row, padded by one pixel on each side
			// so that diffusing off either edge needs no bounds checks.
			var width = image.Width;
			var currentRowError = new float[(width + 2) * 3];
			var nextRowError = new float[(width + 2) * 3];

			for (int y = 0; y < image.Height; y++)
			{
				// Serpentine scanning avoids the diagonal streaks a single scan direction produces.
				var leftToRight = y % 2 == 0;
				var step = leftToRight ? 1 : -1;
				Array.Clear(nextRowError);

				for (int i = 0; i < width; i++)
				{
					var x = leftToRight ? i : width - 1 - i;
					var offset = (x + 1) * 3;
					var original = image[x, y];

					var red = Math.Clamp(original.R + currentRowError[offset], 0f, 255f);
					var green = Math.Clamp(original.G + currentRowError[offset + 1], 0f, 255f);
					var blue = Math.Clamp(original.B + currentRowError[offset + 2], 0f, 255f);

					var chosen = quantize(new Rgba32((byte)MathF.Round(red), (byte)MathF.Round(green), (byte)MathF.Round(blue), original.A));
					image[x, y] = chosen;

					var errors = new[] { red - chosen.R, green - chosen.G, blue - chosen.B };
					for (int channel = 0; channel < 3; channel++)
					{
						var error = errors[channel];
						currentRowError[offset + (step * 3) + channel] += error * 7f / 16f;
						nextRowError[offset - (step * 3) + channel] += error * 3f / 16f;
						nextRowError[offset + channel] += error * 5f / 16f;
						nextRowError[offset + (step * 3) + channel] += error * 1f / 16f;
					}
				}

				(currentRowError, nextRowError) = (nextRowError, currentRowError);
			}
		}

		private static (double L, double A, double B)[] InitializeCentroidsKMeansPlusPlus(
			(double L, double A, double B)[] points, double[] weights, int k)
		{
			// k-means++: each new centroid is drawn with probability proportional to weight * squared distance
			// from the nearest centroid chosen so far, which spreads the starting palette across the color range.
			var random = new Random(0);
			var centroids = new (double L, double A, double B)[k];
			var nearestDistance = new double[points.Length];
			Array.Fill(nearestDistance, double.MaxValue);

			var first = 0;
			for (int i = 1; i < points.Length; i++)
			{
				if (weights[i] > weights[first]) { first = i; }
			}
			centroids[0] = points[first];

			for (int c = 1; c < k; c++)
			{
				var total = 0d;
				for (int i = 0; i < points.Length; i++)
				{
					nearestDistance[i] = Math.Min(nearestDistance[i], DistanceSquared(points[i], centroids[c - 1]));
					total += nearestDistance[i] * weights[i];
				}

				var target = random.NextDouble() * total;
				var chosen = points.Length - 1;
				for (int i = 0; i < points.Length; i++)
				{
					target -= nearestDistance[i] * weights[i];
					if (target <= 0) { chosen = i; break; }
				}
				centroids[c] = points[chosen];
			}

			return centroids;
		}

		private static bool AssignToNearest((double L, double A, double B)[] points,
			(double L, double A, double B)[] centroids, int[] assignment)
		{
			var changed = false;
			for (int i = 0; i < points.Length; i++)
			{
				var best = 0;
				var bestDistance = double.MaxValue;
				for (int c = 0; c < centroids.Length; c++)
				{
					var distance = DistanceSquared(points[i], centroids[c]);
					if (distance < bestDistance)
					{
						bestDistance = distance;
						best = c;
					}
				}

				if (assignment[i] != best)
				{
					assignment[i] = best;
					changed = true;
				}
			}
			return changed;
		}

		private static double DistanceSquared((double L, double A, double B) a, (double L, double A, double B) b)
		{
			var dl = a.L - b.L;
			var da = a.A - b.A;
			var db = a.B - b.B;
			return (dl * dl) + (da * da) + (db * db);
		}

		private static double SrgbToLinear(double channel)
		{
			var c = channel / 255d;
			return c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
		}

		private static double LinearToSrgb(double linear)
		{
			var c = linear <= 0.0031308 ? linear * 12.92 : (1.055 * Math.Pow(linear, 1 / 2.4)) - 0.055;
			return c * 255d;
		}

		// Oklab, from https://bottosson.github.io/posts/oklab/
		private static (double L, double A, double B) RgbToOklab(double r, double g, double b)
		{
			var lr = SrgbToLinear(r);
			var lg = SrgbToLinear(g);
			var lb = SrgbToLinear(b);

			var l = Math.Cbrt((0.4122214708 * lr) + (0.5363325363 * lg) + (0.0514459929 * lb));
			var m = Math.Cbrt((0.2119034982 * lr) + (0.6806995451 * lg) + (0.1073969566 * lb));
			var s = Math.Cbrt((0.0883024619 * lr) + (0.2817188376 * lg) + (0.6299787005 * lb));

			return ((0.2104542553 * l) + (0.7936177850 * m) - (0.0040720468 * s),
				(1.9779984951 * l) - (2.4285922050 * m) + (0.4505937099 * s),
				(0.0259040371 * l) + (0.7827717662 * m) - (0.8086757660 * s));
		}

		private static Rgba32 OklabToRgba32((double L, double A, double B) lab)
		{
			var l = lab.L + (0.3963377774 * lab.A) + (0.2158037573 * lab.B);
			var m = lab.L - (0.1055613458 * lab.A) - (0.0638541728 * lab.B);
			var s = lab.L - (0.0894841775 * lab.A) - (1.2914855480 * lab.B);
			l = l * l * l;
			m = m * m * m;
			s = s * s * s;

			var r = (4.0767416621 * l) - (3.3077115913 * m) + (0.2309699292 * s);
			var g = (-1.2684380046 * l) + (2.6097574011 * m) - (0.3413193965 * s);
			var b = (-0.0041960863 * l) - (0.7034186147 * m) + (1.7076147010 * s);

			return new Rgba32(ClampToByte(Math.Round(LinearToSrgb(r))),
				ClampToByte(Math.Round(LinearToSrgb(g))),
				ClampToByte(Math.Round(LinearToSrgb(b))), 255);
		}

		public static void MutateReduceBitDepthTopColors(Image<Rgba32> image, int bitsPerPixel, bool dither = false)
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
			
			Rgba32 Quantize(Rgba32 pixel)
			{
				var pixelInt = PixelToInt(pixel);

				// If the color is already in the top colors, no need to change it.
				// If it's not, we need to find the closest color in the top colors.
				if (topColors.Contains(pixelInt)) { return pixel; }

				var closestColor = topColorOctree.GetClosestColor(pixelInt);
				if (closestColor == null) { throw new InvalidOperationException("No closest color was found."); }
				return IntToPixel(closestColor.Value);
			}

			ApplyQuantizer(image, Quantize, dither);
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
