using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;

namespace Celarix.Imaging.Packing
{
	internal sealed class Node
    {
        private long serializationNodeId;
        
        public bool Used { get; set; }
        public Node Down { get; set; }
        public Node Right { get; set; }
        public Size Size { get; set; }
        public Point Location { get; set; }

        public Node() { }
        
        public Node(Point location, Size size)
        {
            Size = size;
            Location = location;
        }
    }
}
