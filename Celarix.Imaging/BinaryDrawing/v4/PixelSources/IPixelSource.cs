using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v4.PixelSources
{
    public interface IPixelSource
    {
        long TotalPixels { get; }
        long Position { get; }
        bool Finished { get; }

        NamedStreamReadResult Read(Span<Rgba32> destination, out int pixelsWritten);
    }
}
