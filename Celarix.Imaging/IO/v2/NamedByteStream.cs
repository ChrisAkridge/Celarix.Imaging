using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.IO.v2
{
    public sealed class NamedByteStream : NamedStream<byte>
    {
        private readonly List<string> _filePaths;
        private readonly Dictionary<string, long> _fileLengths;
        private int _currentFileIndex;
        private Stream? _currentStream;
        private long _bytesRead;

        public override string CurrentFileName => _filePaths[_currentFileIndex];

        public override int FileNumber => _currentFileIndex;

        public override int TotalFiles => _filePaths.Count;

        public override long Position => _bytesRead;

        public override long TotalLength { get; }

        public NamedByteStream(IEnumerable<string> filePaths)
        {
            _filePaths = [.. filePaths];
            _fileLengths = new Dictionary<string, long>();
            _currentStream = null;

            var totalLength = 0L;
            foreach (var path in filePaths)
            {
                var info = new FileInfo(path);
                totalLength += info.Length;
                _fileLengths[path] = info.Length;
            }
            TotalLength = totalLength;
            NextFile();
        }

        public override NamedStreamReadResult Read(byte[] buffer, out int elementsRead)
        {
            Array.Clear(buffer, 0, buffer.Length);

            if (_currentStream == null)
            {
                elementsRead = 0;
                return NamedStreamReadResult.EndOfStream;
            }

            int bytesRead = _currentStream.Read(buffer, 0, buffer.Length);
            _bytesRead += bytesRead;
            elementsRead = bytesRead;

            if (bytesRead == 0)
            {
                _currentFileIndex++;
                return NextFile() ? NamedStreamReadResult.EndOfFile : NamedStreamReadResult.EndOfStream;
            }

            return NamedStreamReadResult.NotEndOfFile;
        }

        public long GetFileLength(int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= _filePaths.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(fileIndex), "File index is out of range.");
            }

            var path = _filePaths[fileIndex];
            return _fileLengths[path];
        }

        private bool NextFile()
        {
            _currentStream?.Dispose();
            _currentStream = null;
    
            if (_currentFileIndex < _filePaths.Count)
            {
                var path = _filePaths[_currentFileIndex];
                _currentStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return true;
            }

            return false;
        }
    }
}
