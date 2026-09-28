using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3.PixelSources
{
    internal abstract class FilePixelSource : IPixelSource, IDisposable
    {
        protected readonly Lazy<Stream> _source;
        protected readonly Rgba32[] _palette;
        private readonly Rgba32[] pendingPixels;
        private int pendingOffset;
        private int pendingCount;

        protected PaddingByteReader _byteSource;

        public long TotalPixels { get; }
        public long Position { get; private set; }
        public bool Finished => Position >= TotalPixels;

        public FilePixelSource(Lazy<Stream> source,
            Rgba32[] palette,
            long totalPixels,
            int pixelGroupSize,
            int paddingZeroByteCount)
        {
            _source = source;
            _palette = palette;
            TotalPixels = totalPixels;
            pendingPixels = new Rgba32[pixelGroupSize];
            _byteSource = new PaddingByteReader(paddingZeroByteCount);
        }

        public int Read(Span<Rgba32> destination)
        {
            var written = 0;

            while (written < destination.Length && !Finished)
            {
                if (pendingOffset >= pendingCount)
                {
                    pendingOffset = 0;
                    pendingCount = ReadNextPixelGroup(pendingPixels);

                    if (pendingCount == 0)
                    {
                        break;
                    }
                }

                var available = pendingCount - pendingOffset;
                var wanted = destination.Length - written;
                var toCopy = Math.Min(available, wanted);

                pendingPixels.AsSpan(pendingOffset, toCopy).CopyTo(destination.Slice(written, toCopy));

                pendingOffset += toCopy;
                written += toCopy;
                Position += toCopy;
            }

            return written;
        }


        protected abstract int ReadNextPixelGroup(Span<Rgba32> group);

        public void Dispose()
        {
            if (_source.IsValueCreated)
            {
                _source.Value.Dispose();
            }
        }
    }
}
