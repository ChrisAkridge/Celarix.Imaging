using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3.PixelSources
{
    internal interface IPixelSource
    {
        long TotalPixels { get; }
        long Position { get; }
        bool Finished { get; }

        int Read(Span<Rgba32> destination);
    }
}
