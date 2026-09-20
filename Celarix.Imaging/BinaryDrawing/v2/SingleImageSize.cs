using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v2
{
    internal sealed class SingleImageSize
    {
        private readonly SingleImageBlock[] _blocks;

        public SingleImageSizingResult Result { get; }
        public IReadOnlyList<SingleImageBlock> Blocks => _blocks;

        public Size TotalSize
        {
            get
            {
                var width = _blocks.Max(b => b.Rectangle.Right);
                var height = _blocks.Max(b => b.Rectangle.Bottom);
                return new Size(width, height);
            }
        }

        public SingleImageSize(SingleImageSizingResult result, IEnumerable<SingleImageBlock> blocks)
        {
            Result = result;
            _blocks = [.. blocks];
        }
    }

    internal sealed class SingleImageBlock
    {
        public SingleImageSizingType Type { get; }
        public Rectangle Rectangle { get; }

        public SingleImageBlock(SingleImageSizingType type, Rectangle rectangle)
        {
            Type = type;
            Rectangle = rectangle;
        }
    }
}
