using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Celarix.Imaging.IO
{
	public sealed class NamedMultiStream : Stream
    {
        private readonly List<NamedStream> streams;
        private readonly long length;
        private int currentStreamIndex;
        private long position;

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => length;
        public List<string> NameBuffer { get; set; }

        public override long Position
        {
            get => position;
            set
            {
                var newStream = streams.First(s => s.Offset + s.Stream.Length >= value);
                currentStreamIndex = streams.IndexOf(newStream);
                newStream.Stream.Seek(value - newStream.Stream.Position, SeekOrigin.Begin);
                position = value;
            }
        }

        public NamedMultiStream(IEnumerable<KeyValuePair<string, Stream>> streams)
        {
            this.streams = new List<NamedStream>();
            var offset = 0L;

            foreach (var (name, stream) in streams)
            {
                this.streams.Add(new NamedStream(name, stream, offset));
                offset += stream.Length;
                length += stream.Length;
            }
        }

        public override void Flush() => throw new NotImplementedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            var totalBytesRead = 0;

            while (count > 0 && currentStreamIndex < streams.Count)
            {
                var currentStream = streams[currentStreamIndex];
                if (NameBuffer != null && NameBuffer.Count == 0)
                {
                    NameBuffer.Add(currentStream.Name);
                }

                var bytesRead = currentStream.Stream.Read(buffer, offset, count);

                if (bytesRead == 0)
                {
                    if (currentStreamIndex + 1 < streams.Count)
                    {
                        currentStreamIndex += 1;
                        NameBuffer?.Add(streams[currentStreamIndex].Name);
                    }
                    else { return totalBytesRead; }
                }

                totalBytesRead += bytesRead;
                offset += bytesRead;
                position += bytesRead;
                count -= bytesRead;
            }

            return totalBytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    long totalLength = streams.Sum(s => s.Stream.Length);
                    Position = totalLength - offset;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }

            return Position;
        }
        public override void SetLength(long value) => throw new NotImplementedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public void DisposeAll()
        {
            foreach (var stream in streams) { stream.Stream.Dispose(); }
        }
    }
}
