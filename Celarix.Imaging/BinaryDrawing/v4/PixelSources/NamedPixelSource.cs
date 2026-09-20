using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v4.PixelSources
{
    public sealed class NamedPixelSource
    {
        public string FilePath { get; }
        public IPixelSource Source { get; }

        public NamedPixelSource(string filePath, IPixelSource source)
        {
            FilePath = filePath;
            Source = source;
        }
    }
}
