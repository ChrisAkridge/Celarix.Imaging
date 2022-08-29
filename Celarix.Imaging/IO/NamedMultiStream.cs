using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Celarix.Imaging.Collections;

namespace Celarix.Imaging.IO
{
	public class NamedMultiStream : Stream
    {
        private readonly List<NamedStream> namedStreams;
        private readonly SingleItemLazyList<Stream> lazyStreamList;
        private int currentStreamIndex;
        private long position;

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length { get; }

        public List<string> NameBuffer { get; set; }

        public override long Position
        {
            get => position;
            set
            {
                var newNamedStream = namedStreams.First(s => s.Offset + s.Length >= value);
                currentStreamIndex = namedStreams.IndexOf(newNamedStream);

                var newStream = lazyStreamList.GetItem(newNamedStream.StreamIndex);
                newStream.Seek(value - newStream.Position, SeekOrigin.Begin);
                position = value;
            }
        }
        
        internal NamedMultiStream() { }

        public NamedMultiStream(IEnumerable<string> filePaths)
        {
            namedStreams = new List<NamedStream>();
            lazyStreamList = new SingleItemLazyList<Stream>();
            var i = 0;
            var offset = 0L;

            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);

                namedStreams.Add(new NamedStream
                {
                    Name = filePath,
                    Length = fileInfo.Length,
                    Offset = offset,
                    StreamIndex = i
                });
                
                lazyStreamList.Add(() => File.OpenRead(filePath));

                offset += fileInfo.Length;
                Length += fileInfo.Length;
                i++;
            }
        }

        public override void Flush() => throw new NotImplementedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            var totalBytesRead = 0;

            while (count > 0 && currentStreamIndex < namedStreams.Count)
            {
                var currentNamedStream = namedStreams[currentStreamIndex];
                if (NameBuffer != null && NameBuffer.Count == 0)
                {
                    NameBuffer.Add(currentNamedStream.Name);
                }

                var currentStream = lazyStreamList.GetItem(currentNamedStream.StreamIndex);
                var bytesRead = currentStream.Read(buffer, offset, count);

                if (bytesRead == 0)
                {
                    if (currentStreamIndex + 1 < namedStreams.Count)
                    {
                        currentStreamIndex += 1;
                        NameBuffer?.Add(namedStreams[currentStreamIndex].Name);
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
                    long totalLength = namedStreams.Sum(s => s.Length);
                    Position = totalLength - offset;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }

            return Position;
        }
        public override void SetLength(long value) => throw new NotImplementedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
    }
}
