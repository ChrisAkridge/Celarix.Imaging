using SixLabors.ImageSharp.ColorSpaces.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.Pipeline
{
    public sealed class OperationProperties
    {
        // Binary Drawing Operations
        public ByteToPixelFormat ByteToPixelFormat { get; set; }
        public BinaryDrawingColorSpace BinaryDrawingColorSpace { get; set; }
        public BinaryDrawingPixelLayout BinaryDrawingPixelLayout { get; set; }
        public BinaryDrawingSizingMode BinaryDrawingSizingMode { get; set; }
        public BinaryDrawingTitleMode BinaryDrawingTitleMode { get; set; }

        // Sorting and Unique Colors
        public ColorSpace SortColorSpace { get; set; }
        public ColorSpace UniqueColorsColorSpace { get; set; }

        // View Color Channels
        public SelectedChannels ViewColorChannels { get; set; }
    }
}
