using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3.PixelSources
{
    internal sealed class PaddingByteReader
    {
        private int _paddingZeroByteCount;

        public PaddingByteReader(int paddingZeroByteCount)
        {
            _paddingZeroByteCount = paddingZeroByteCount;
        }

        public int Read(Span<byte> destination, Stream stream)
        {
            var written = stream.Read(destination);
            if (written < destination.Length)
            {
                var remaining = destination.Length - written;
                var index = written;
                while (remaining > 0 && _paddingZeroByteCount > 0)
                {
                    destination[index] = 0x00;
                    index += 1;
                    remaining -= 1;
                    _paddingZeroByteCount -= 1;
                    written += 1;
                }
            }

            return written;
        }
    }
}
