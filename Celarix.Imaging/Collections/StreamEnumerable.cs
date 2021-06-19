using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Celarix.Imaging.BinaryDrawing;

namespace Celarix.Imaging.Collections
{
	internal sealed class StreamEnumerable : IEnumerable<byte>
    {
        private readonly Stream stream;

        public StreamEnumerable(Stream stream) => this.stream = stream;

        public IEnumerator<byte> GetEnumerator()
        {
            const int BufferSize = 1048576;
            int bytesRead;
            var buffer = new byte[BufferSize];

            do
            {
                bytesRead = stream.Read(buffer, 0, BufferSize);

                for (var i = 0; i < bytesRead; i++) { yield return buffer[i]; }
            } while (bytesRead == BufferSize);
        }

        public IEnumerable<int> EnumeratePixels(int bitDepth, int bufferSize = 1048576)
        {
            int bytesRead;
            var buffer = new byte[bufferSize];

            do
            {
                bytesRead = stream.Read(buffer, 0, bufferSize);

                var pixels = PixelBufferer.BufferToPixels(buffer, bytesRead, bitDepth);
                for (var i = 0; i < pixels.Length; i++) { yield return pixels[i]; }
            } while (bytesRead == bufferSize);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
