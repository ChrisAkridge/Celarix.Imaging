using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3.PixelSources
{
    internal sealed class PixelSource1Bpp : FilePixelSource
    {
        private byte[] _buffer = new byte[1];

        public PixelSource1Bpp(Stream source, Rgba32[] palette)
            : base(source, palette, source.Length * 8, 8, 0)
        {
            if (palette.Length != 2)
            {
                throw new ArgumentException($"A 1bpp image must have a palette of exactly 2 colors, but {palette.Length} were provided.", nameof(palette));
            }
        }

        protected override int ReadNextPixelGroup(Span<Rgba32> group)
        {
            int read = _byteSource.Read(_buffer, _source);

            if (read <= 0) { return 0; }

            var data = _buffer[0];
            group[0] = _palette[data >> 7];
            group[1] = _palette[(data >> 6) & 0b1];
            group[2] = _palette[(data >> 5) & 0b1];
            group[3] = _palette[(data >> 4) & 0b1];
            group[4] = _palette[(data >> 3) & 0b1];
            group[5] = _palette[(data >> 2) & 0b1];
            group[6] = _palette[(data >> 1) & 0b1];
            group[7] = _palette[data & 0b1];
            return 8;
        }
    }
}
