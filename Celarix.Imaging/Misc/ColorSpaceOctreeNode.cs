using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.Misc
{
	/// <summary>
	/// Represents a division of a color space. Contains either a set of colors or references to 8 child nodes.
	/// </summary>
	internal sealed class ColorSpaceOctreeNode
	{
		private const int MaxColorsInNode = 256;

		// Exactly one of these is non-null.
		private List<int> colorsInNode;
		private ColorSpaceOctreeNode[] children;
		
		private readonly int depth;

		/// <summary>
		/// Represents a division of a color space. Contains either a set of colors or references to 8 child nodes.
		/// </summary>
		public ColorSpaceOctreeNode(int depth) => this.depth = depth;

		public void AddColor(int color)
		{
			if (colorsInNode != null)
			{
				if (colorsInNode.Count < MaxColorsInNode)
				{
					colorsInNode.Add(color);
				}
				else
				{
					Split();
					int index = GetChildIndex(color);
					children[index].AddColor(color);
				}
			}
			else if (children != null)
			{
				int index = GetChildIndex(color);
				children[index].AddColor(color);
			}
			else
			{
				colorsInNode = [color];
			}
		}

		public int? GetClosestColor(int searchColor)
		{
			if (colorsInNode != null)
			{
				return colorsInNode.MinBy(c => ColorDistance(c, searchColor));
			}

			if (children == null)
			{
				// We were split but no colors were added to us.
				return null;
			}

			var index = GetChildIndex(searchColor);
			var closestColor = children[index].GetClosestColor(searchColor);

			if (closestColor != null) { return closestColor; }

			// If we subdivide the octree enough, it's inevitable that we'll end up with a node that has no colors.
			// But we did need to split the octree at this level, so at least 1 of our eight children has to have colors.
			var closestChildrenDistance = double.PositiveInfinity;
			int? closestChildrenColor = null;
			for (int i = 0; i < 8; i++)
			{
				if (i == index) { continue; }

				var closestChildColor = children[i].GetClosestColor(searchColor);
				if (closestChildColor == null) { continue; }
				var distance = ColorDistance(closestChildColor.Value, searchColor);
				if (distance <= closestChildrenDistance)
				{
					closestChildrenDistance = distance;
					closestChildrenColor = closestChildColor;
				}
			}

			// May return null if we didn't find any colors in the children, either.
			// Let the recursive caller handle it.
			return closestChildrenColor;
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString() =>
			colorsInNode != null
				? $"Node with {colorsInNode.Count} colors"
				: $"Node with 8 children, {children.Count(c => c.colorsInNode != null)} with colors";

		private void Split()
		{
			// We split this node into 8 children, all at one more depth than we're at.
			// The 8 children are drawn from {top, bottom}, {left, right}, {front, back}.
			// The highest bit of the child index is for top and bottom, the middle bit is for left and right,
			// and the lowest bit is for front and back.
			children = new ColorSpaceOctreeNode[8];
			for (int i = 0; i < 8; i++)
			{
				children[i] = new ColorSpaceOctreeNode(depth + 1);
			}
			
			// Now we have to distribute all the colors in this node to the children.
			foreach (int color in colorsInNode)
			{
				int index = GetChildIndex(color);
				children[index].AddColor(color);
			}

			colorsInNode = null;
		}
		
		private int GetChildIndex(int color)
		{
			// To figure out which child a color belongs to, we look at the bits of the color.
			// We consider the bottom 24 bits of the integer to be the color, which lets us use
			// any color space we want so long as it fits in 24 bits.
			
			// We determine the bits to look at by the depth.
			// Depth 0 looks at bits 24, 23, and 22,
			// Depth 1 looks at bits 21, 20, and 19, and so forth.
			const int mask = 0b111;
			int shift = 21 - (3 * depth);
			return (color & (mask << shift)) >> shift;
		}

		private static double ColorDistance(int a, int b)
		{
			// Use a three-dimensional distance formula to calculate the distance between two colors.
			int ar = a >> 16;
			int ag = (a >> 8) & 0xFF;
			int ab = a & 0xFF;
			int br = b >> 16;
			int bg = (b >> 8) & 0xFF;
			int bb = b & 0xFF;
			
			int rSquared = (ar - br) * (ar - br);
			int gSquared = (ag - bg) * (ag - bg);
			int bSquared = (ab - bb) * (ab - bb);

			return Math.Sqrt(rSquared + gSquared + bSquared);
		}
	}
}
