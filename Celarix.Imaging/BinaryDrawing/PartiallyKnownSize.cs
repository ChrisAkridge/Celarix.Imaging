using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing
{
	public readonly struct PartiallyKnownSize(int? width, int? height)
	{
		public int? Width { get; } = width;
		public int? Height { get; } = height;
		
		// Write a Deconstruct method that allows the tuple to be deconstructed into two nullable integers.
		public void Deconstruct(out int? width, out int? height)
		{
			width = Width;
			height = Height;
		}
	}
}
