using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;

namespace Celarix.Imaging.Packing
{
    internal sealed class PackingJob
    {
        public PackingOptions Options { get; set; }
        public Dictionary<string, Size> ImagesAndSizes { get; set; }
        public List<Block> Blocks { get; set; }
    }
}
