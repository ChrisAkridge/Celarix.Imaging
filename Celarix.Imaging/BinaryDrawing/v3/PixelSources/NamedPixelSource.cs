using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3.PixelSources
{
    internal sealed class NamedPixelSource
    {
        public string FilePath { get; }
        public IPixelSource PixelSource { get; }

        public NamedPixelSource(string filePath, IPixelSource pixelSource)
        {
            FilePath = filePath;
            PixelSource = pixelSource;
        }
    }
}
