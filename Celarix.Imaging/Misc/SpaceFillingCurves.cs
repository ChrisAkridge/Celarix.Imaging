using System;

namespace Celarix.Imaging.Misc
{
	/// <summary>
	/// Space-filling curves: paths that visit every cell of a grid exactly once, in an order where cells that are
	/// close along the path tend to be close in space, and vice versa. That locality is what makes them useful for
	/// laying data out in 2D without it turning into stripes.
	/// </summary>
	public static class SpaceFillingCurves
	{
		/// <summary>
		/// Converts a distance along the 2D Hilbert curve into a cell position. The curve fills a square whose side is
		/// a power of two, starts at (0, 0), and ends at (sideLength - 1, 0).
		/// </summary>
		/// <remarks>
		/// The curve is built recursively: a square is four quadrants, each holding a Hilbert curve that has been
		/// rotated or flipped so that the end of one meets the start of the next. This loop undoes that recursion
		/// from the smallest quadrants outward, two bits of the distance at a time. Each pair of bits says which
		/// quadrant the distance falls in, and the earlier (smaller-scale) coordinates are transformed to match the
		/// quadrant's orientation.
		/// </remarks>
		public static (int X, int Y) HilbertDistanceToPoint(int sideLength, long distance)
		{
			if (sideLength < 1 || (sideLength & (sideLength - 1)) != 0)
			{
				throw new ArgumentException("The side length must be a power of two.", nameof(sideLength));
			}

			var x = 0;
			var y = 0;
			var remaining = distance;

			for (int scale = 1; scale < sideLength; scale *= 2)
			{
				// The low two bits pick the quadrant (in Hilbert order, not row/column order).
				var quadrantX = (int)(1 & (remaining / 2));
				var quadrantY = (int)(1 & (remaining ^ quadrantX));

				// Rotate the sub-square built so far to fit the quadrant it is landing in.
				if (quadrantY == 0)
				{
					if (quadrantX == 1)
					{
						x = scale - 1 - x;
						y = scale - 1 - y;
					}

					(x, y) = (y, x);
				}

				x += scale * quadrantX;
				y += scale * quadrantY;
				remaining /= 4;
			}

			return (x, y);
		}

		/// <summary>
		/// Converts a point in a 3D grid, with bitsPerAxis bits per coordinate, into its distance along the 3D Hilbert
		/// curve. Points that are close on the curve are close in space, so sorting points by this value groups
		/// nearby points together in a way that a plain sort on one axis (which gives stripes) cannot.
		/// </summary>
		/// <remarks>
		/// This is John Skilling's algorithm ("Programming the Hilbert curve", 2004). It works for any number of
		/// dimensions by rearranging the coordinates' bits in place until the bits, read across all coordinates
		/// from most to least significant, spell out the Hilbert distance. The first loop is the "undo the
		/// rotations" step, the second converts to a Gray code (which is what makes consecutive distances differ by
		/// exactly one step), and the last loop interleaves the bits into a single number.
		/// </remarks>
		public static ulong HilbertPointToDistance3D(uint x, uint y, uint z, int bitsPerAxis)
		{
			if (bitsPerAxis is < 1 or > 21)
			{
				throw new ArgumentOutOfRangeException(nameof(bitsPerAxis), bitsPerAxis, "Bits per axis must be between 1 and 21 so that the distance fits in 63 bits.");
			}

			Span<uint> coordinates = stackalloc uint[] { x, y, z };
			var highestBit = 1u << (bitsPerAxis - 1);

			// Undo the excess work: for each bit from the top down, flip or swap the lower bits
			// depending on whether that bit is set, mirroring how the curve is rotated per quadrant.
			for (var bit = highestBit; bit > 1; bit >>= 1)
			{
				var lowerBits = bit - 1;
				for (int axis = 0; axis < 3; axis++)
				{
					if ((coordinates[axis] & bit) != 0)
					{
						coordinates[0] ^= lowerBits;
					}
					else
					{
						var swap = (coordinates[0] ^ coordinates[axis]) & lowerBits;
						coordinates[0] ^= swap;
						coordinates[axis] ^= swap;
					}
				}
			}

			// Gray encode.
			coordinates[1] ^= coordinates[0];
			coordinates[2] ^= coordinates[1];

			var flipMask = 0u;
			for (var bit = highestBit; bit > 1; bit >>= 1)
			{
				if ((coordinates[2] & bit) != 0)
				{
					flipMask ^= bit - 1;
				}
			}

			for (int axis = 0; axis < 3; axis++)
			{
				coordinates[axis] ^= flipMask;
			}

			// Interleave: the distance's top bit is x's top bit, then y's, then z's, then x's next bit, and so on.
			var distance = 0UL;
			for (int bit = bitsPerAxis - 1; bit >= 0; bit--)
			{
				for (int axis = 0; axis < 3; axis++)
				{
					distance = (distance << 1) | ((coordinates[axis] >> bit) & 1);
				}
			}

			return distance;
		}

		/// <summary>
		/// Visits every cell of a width x height grid, in order, along a "generalized Hilbert" (gilbert) curve.
		/// Unlike the ordinary Hilbert curve, it works for any rectangle, not just power-of-two squares.
		/// </summary>
		/// <remarks>
		/// From Jakub Cerveny's gilbert (https://github.com/jakubcerveny/gilbert). The idea is the same as the
		/// Hilbert curve's: split the rectangle into pieces, fill each with a smaller curve, and orient the pieces
		/// so that each one ends where the next begins. Where Hilbert always splits into four equal quadrants,
		/// this splits a wide rectangle in two, or a normal one into three (a step up, a long run across, a step
		/// down), and nudges the split points to even numbers so that pieces join cleanly. For rectangles with
		/// odd dimensions a single diagonal step may be unavoidable, but every other step moves to a neighbor.
		/// </remarks>
		public static void VisitGilbert(int width, int height, Action<int, int> visit)
		{
			if (width < 1 || height < 1)
			{
				throw new ArgumentOutOfRangeException(width < 1 ? nameof(width) : nameof(height), "Both dimensions must be at least 1.");
			}

			// (ax, ay) is the vector along the rectangle's long side and (bx, by) the vector along its short side;
			// their signs say which way the curve runs, which is how pieces get rotated and flipped.
			if (width >= height)
			{
				GilbertPiece(0, 0, width, 0, 0, height, visit);
			}
			else
			{
				GilbertPiece(0, 0, 0, height, width, 0, visit);
			}
		}

		private static void GilbertPiece(int x, int y, int ax, int ay, int bx, int by, Action<int, int> visit)
		{
			var majorLength = Math.Abs(ax + ay);
			var minorLength = Math.Abs(bx + by);

			// Unit vectors for the major and minor directions.
			var majorStepX = Math.Sign(ax);
			var majorStepY = Math.Sign(ay);
			var minorStepX = Math.Sign(bx);
			var minorStepY = Math.Sign(by);

			if (minorLength == 1)
			{
				// One cell tall: a straight run.
				for (int i = 0; i < majorLength; i++)
				{
					visit(x, y);
					x += majorStepX;
					y += majorStepY;
				}

				return;
			}

			if (majorLength == 1)
			{
				// One cell wide: a straight run along the other direction.
				for (int i = 0; i < minorLength; i++)
				{
					visit(x, y);
					x += minorStepX;
					y += minorStepY;
				}

				return;
			}

			// Half-vectors. The reference implementation uses floor division, which is what >> 1 does even for
			// negative numbers (C#'s / would round toward zero instead, changing which cells get split).
			var halfAx = ax >> 1;
			var halfAy = ay >> 1;
			var halfBx = bx >> 1;
			var halfBy = by >> 1;
			var halfMajorLength = Math.Abs(halfAx + halfAy);
			var halfMinorLength = Math.Abs(halfBx + halfBy);

			if (2 * majorLength > 3 * minorLength)
			{
				// Long and thin: split in two along the long side.
				if (halfMajorLength % 2 != 0 && majorLength > 2)
				{
					// Prefer even sizes, so the two halves connect cleanly.
					halfAx += majorStepX;
					halfAy += majorStepY;
				}

				GilbertPiece(x, y, halfAx, halfAy, bx, by, visit);
				GilbertPiece(x + halfAx, y + halfAy, ax - halfAx, ay - halfAy, bx, by, visit);
			}
			else
			{
				// Standard case: one piece stepping up, one long piece across, and one stepping back down.
				if (halfMinorLength % 2 != 0 && minorLength > 2)
				{
					halfBx += minorStepX;
					halfBy += minorStepY;
				}

				GilbertPiece(x, y, halfBx, halfBy, halfAx, halfAy, visit);
				GilbertPiece(x + halfBx, y + halfBy, ax, ay, bx - halfBx, by - halfBy, visit);
				GilbertPiece(x + (ax - majorStepX) + (halfBx - minorStepX),
					y + (ay - majorStepY) + (halfBy - minorStepY),
					-halfBx, -halfBy, -(ax - halfAx), -(ay - halfAy), visit);
			}
		}
	}
}
