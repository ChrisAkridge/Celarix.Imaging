using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;

namespace Celarix.Imaging.Packing
{
	internal sealed class Block
	{
        public Size Size { get; set; }
        public Node Fit { get; set; }
        public string ImageFilePath { get; set; }
	}
}
