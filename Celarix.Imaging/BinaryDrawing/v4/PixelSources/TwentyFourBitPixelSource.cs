using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v4.PixelSources
{
    public sealed class TwentyFourBitPixelSource : IPixelSource
    {
        public long TotalPixels => throw new NotImplementedException();
        public long Position => throw new NotImplementedException();
        public bool Finished => throw new NotImplementedException();

        public NamedStreamReadResult Read(Span<Rgba32> destination, out int pixelsWritten)
        {
            throw new NotImplementedException();
        }
    }
}
