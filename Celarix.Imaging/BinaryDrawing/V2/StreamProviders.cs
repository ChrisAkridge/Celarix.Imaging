using System;
using System.IO;

namespace Celarix.Imaging.BinaryDrawing.V2
{
    public sealed class ImmediateStreamProvider : IStreamProvider
    {
        private readonly Stream stream;

        public bool DisposeOnAdvance { get; }
        public long Length { get; }
        public string Name { get; }

        public ImmediateStreamProvider(Stream stream, bool disposeOnAdvance = false, string name = null)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            DisposeOnAdvance = disposeOnAdvance;
            Length = stream.Length;
            Name = name;
        }

        public Stream GetStream() => stream;
    }

    public sealed class LazyFileStreamProvider : IStreamProvider
    {
        private readonly string filePath;
        private Stream createdStream;
        private long? cachedLength;

        public bool DisposeOnAdvance => true;
        
        public long Length
        {
            get
            {
                if (createdStream != null) return createdStream.Length;
                if (cachedLength.HasValue) return cachedLength.Value;
                
                if (!File.Exists(filePath)) throw new FileNotFoundException("File not found.", filePath);
                cachedLength = new FileInfo(filePath).Length;
                return cachedLength.Value;
            }
        }
        
        public string Name { get; }

        public LazyFileStreamProvider(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Name = filePath;
        }

        public Stream GetStream()
        {
            createdStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return createdStream;
        }
    }
}
