using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v4
{
    public sealed class NamedByteSourceProvider
    {
        private readonly (string FilePath, Func<Stream> StreamFactory)[] _files;
        private int _currentIndex = 0;
        private Stream? _currentStream;

        private int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex == value)
                {
                    return;
                }

                _currentIndex = value;
                ChangeStream();
            }
        }

        public string CurrentFilePath => _files[CurrentIndex].FilePath;

        public NamedByteSourceProvider(params string[] filePaths)
        {
            _files = [.. filePaths.Select(fp => (FilePath: fp, StreamFactory: new Func<Stream>(() => File.OpenRead(fp))))];
        }

        public NamedStreamReadResult Read(Span<byte> destination, out int bytesWritten)
        {
            if (_currentStream == null)
            {
                // This is actually pretty elegant! On first read, we immediately signal that you
                // have to read the file path to properly draw title bars if you want them.
                bytesWritten = 0;
                NextFile();
                return NamedStreamReadResult.EndOfFile;
            }

            // Read into the destination and check how many bytes were written.
            bytesWritten = _currentStream.Read(destination);

            // If we read 0 bytes, that means we've reached the end of the stream. We should move to the next file.
            if (bytesWritten == 0)
            {
                if (_currentIndex == _files.Length - 1)
                {
                    // We've reached the end of all files.
                    return NamedStreamReadResult.EndOfStream;
                }

                NextFile();
                return NamedStreamReadResult.EndOfFile;
            }
            return NamedStreamReadResult.NotEndOfFile;
        }

        public void Reset()
        {
            CurrentIndex = 0;
        }

        private void ChangeStream()
        {
            _currentStream?.Dispose();
            _currentStream = _files[CurrentIndex].StreamFactory();
        }

        private void NextFile()
        {
            if (CurrentIndex < _files.Length - 1)
            {
                CurrentIndex += 1;
            }
        }
    }
}
