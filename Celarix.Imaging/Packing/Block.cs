using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;

namespace Celarix.Imaging.Packing
{
	internal sealed class Block<TSource>
	{
        public Size Size { get; set; }
        public Node? Fit { get; set; }
        public TSource? Source { get; set; }
	}
}
