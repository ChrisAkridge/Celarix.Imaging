using System;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Celarix.Imaging.Misc
{
	/// <summary>
	/// Draws the set of distinct colors in an image as an image of its own, one pixel per color, arranged so that
	/// similar-looking colors end up next to each other. Colors are compared by their position on a 3D Hilbert
	/// curve through Oklab space (see <see cref="SpaceFillingCurves"/>), which is a rough proxy for how different
	/// they look. Pixels with no color are left transparent.
	/// </summary>
	public static class UniqueColorMap
	{
		private const int HilbertBitsPerAxis = 10;
		private const int SideLength = 4096;
		private const int ColorCount = 1 << 24;

		// Oklab's lightness runs 0..1 for sRGB colors; a and b stay within about +/-0.33.
		private const double OklabABExtent = 0.35;

		private static readonly double[] SrgbToLinearTable = BuildSrgbToLinearTable();
		private static readonly object RankTableLock = new();
		private static int[] rankByColor;

		/// <summary>
		/// The compact layout: only the colors present are drawn, sorted by their Oklab Hilbert distance and laid
		/// along a generalized Hilbert curve on a roughly square rectangle. Every pixel is used (bar the last few),
		/// but where a color lands depends on which other colors the image has, so two images' maps are not
		/// directly comparable.
		/// </summary>
		public static Image<Rgba32> RenderCompact(Image<Rgba32> source)
		{
			var colors = GetDistinctColors(source);

			// Sorting by (distance, color) gives every color a unique, stable place in line.
			var sortKeys = new ulong[colors.Length];
			for (int i = 0; i < colors.Length; i++)
			{
				sortKeys[i] = (OklabHilbertDistance(colors[i]) << 24) | (uint)colors[i];
			}
			Array.Sort(sortKeys);

			var width = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(sortKeys.Length)));
			var height = Math.Max(1, (int)Math.Ceiling(sortKeys.Length / (double)width));
			var map = new Image<Rgba32>(width, height);

			// The first sortKeys.Length cells of the curve receive the colors in order; any leftover cells stay transparent.
			var nextColor = 0;
			SpaceFillingCurves.VisitGilbert(width, height, (x, y) =>
			{
				if (nextColor < sortKeys.Length)
				{
					map[x, y] = ColorFromInt((int)(sortKeys[nextColor] & 0xFFFFFF));
					nextColor++;
				}
			});

			return map;
		}

		/// <summary>
		/// The fixed layout: a 4096x4096 image with one pixel for each of the 2^24 possible colors. Where each color goes
		/// never depends on the image, so maps of different images can be compared directly, at the cost of most of
		/// the map being empty. Colors are ranked by Oklab Hilbert distance across all 2^24 of them, and rank
		/// picks the position along an ordinary Hilbert curve starting at (0, 0).
		/// </summary>
		public static Image<Rgba32> RenderFixed(Image<Rgba32> source)
		{
			var ranks = GetRankTable();
			var map = new Image<Rgba32>(SideLength, SideLength);

			foreach (var color in GetDistinctColors(source))
			{
				var (x, y) = SpaceFillingCurves.HilbertDistanceToPoint(SideLength, ranks[color]);
				map[x, y] = ColorFromInt(color);
			}

			return map;
		}

		private static int[] GetDistinctColors(Image<Rgba32> source)
		{
			// One flag per possible color (16 MB) is the cheapest way to deduplicate, and reading it back in order sorts the colors too.
			var seen = new bool[ColorCount];
			var distinct = 0;

			for (int y = 0; y < source.Height; y++)
			{
				for (int x = 0; x < source.Width; x++)
				{
					var color = ColorToInt(source[x, y]);
					if (!seen[color])
					{
						seen[color] = true;
						distinct++;
					}
				}
			}

			var colors = new int[distinct];
			var index = 0;
			for (int color = 0; color < ColorCount; color++)
			{
				if (seen[color]) { colors[index++] = color; }
			}

			return colors;
		}

		/// <summary>
		/// Builds (once, then keeps) a table giving each of the 2^24 colors its rank when all colors are sorted by
		/// Oklab Hilbert distance. Costs about a second or two, and 64 MB while it is held.
		/// </summary>
		private static int[] GetRankTable()
		{
			lock (RankTableLock)
			{
				if (rankByColor != null) { return rankByColor; }

				var sortKeys = new ulong[ColorCount];
				Parallel.For(0, 256, red =>
				{
					for (int green = 0; green < 256; green++)
					{
						for (int blue = 0; blue < 256; blue++)
						{
							var color = (red << 16) | (green << 8) | blue;
							sortKeys[color] = (OklabHilbertDistance(color) << 24) | (uint)color;
						}
					}
				});

				Array.Sort(sortKeys);

				var ranks = new int[ColorCount];
				for (int rank = 0; rank < sortKeys.Length; rank++)
				{
					ranks[(int)(sortKeys[rank] & 0xFFFFFF)] = rank;
				}

				rankByColor = ranks;
				return ranks;
			}
		}

		/// <summary>
		/// Where a color falls along a 3D Hilbert curve through Oklab space. Oklab is used because distances in it
		/// roughly match perceived color difference, so colors near each other on the curve look alike.
		/// </summary>
		private static ulong OklabHilbertDistance(int color)
		{
			var linearRed = SrgbToLinearTable[(color >> 16) & 0xFF];
			var linearGreen = SrgbToLinearTable[(color >> 8) & 0xFF];
			var linearBlue = SrgbToLinearTable[color & 0xFF];

			// Oklab, from https://bottosson.github.io/posts/oklab/ (the same conversion as in ChromaPlaygroundHelpers).
			var l = Math.Cbrt((0.4122214708 * linearRed) + (0.5363325363 * linearGreen) + (0.0514459929 * linearBlue));
			var m = Math.Cbrt((0.2119034982 * linearRed) + (0.6806995451 * linearGreen) + (0.1073969566 * linearBlue));
			var s = Math.Cbrt((0.0883024619 * linearRed) + (0.2817188376 * linearGreen) + (0.6299787005 * linearBlue));

			var lightness = (0.2104542553 * l) + (0.7936177850 * m) - (0.0040720468 * s);
			var a = (1.9779984951 * l) - (2.4285922050 * m) + (0.4505937099 * s);
			var b = (0.0259040371 * l) + (0.7827717662 * m) - (0.8086757660 * s);

			// Scale each axis onto the curve's integer grid.
			const int maxCoordinate = (1 << HilbertBitsPerAxis) - 1;
			static uint Quantize(double value, double minimum, double maximum) =>
				(uint)Math.Clamp(Math.Round((value - minimum) / (maximum - minimum) * maxCoordinate), 0, maxCoordinate);

			return SpaceFillingCurves.HilbertPointToDistance3D(
				Quantize(lightness, 0d, 1d),
				Quantize(a, -OklabABExtent, OklabABExtent),
				Quantize(b, -OklabABExtent, OklabABExtent),
				HilbertBitsPerAxis);
		}

		private static double[] BuildSrgbToLinearTable()
		{
			var table = new double[256];
			for (int value = 0; value < table.Length; value++)
			{
				var c = value / 255d;
				table[value] = c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
			}
			return table;
		}

		private static int ColorToInt(Rgba32 pixel) => (pixel.R << 16) | (pixel.G << 8) | pixel.B;

		private static Rgba32 ColorFromInt(int color) => new((byte)(color >> 16), (byte)(color >> 8), (byte)color, 255);
	}
}
