using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.ByteView
{
	public sealed class MultiFileStream
    {
        private BinaryReader currentFileStream;
        private readonly IEnumerator<string> filePathEnumerator;

        public MultiFileStream(IEnumerable<string> filePaths)
        {
            filePathEnumerator = filePaths.GetEnumerator();

            if (!TryMoveNext())
            {
                throw new ArgumentException("No file in the path list could be opened.");
            }
        }

        public IList<string> ReadReturnFileNames(byte[] buffer, int count)
        {
            var filesInBuffer = new List<string> { filePathEnumerator.Current };
            int bufferPosition = 0;

            while (true)
            {
                int bytesRead = currentFileStream.Read(buffer, bufferPosition, count);
                bufferPosition += bytesRead;
                count -= bytesRead;
                if (count == 0) { break; }

                if (!TryMoveNext()) { return filesInBuffer; }
                filesInBuffer.Add(filePathEnumerator.Current);
            }

            return filesInBuffer;
        }

        private bool TryMoveNext()
        {
            while (true)
            {
                try
                {
                    currentFileStream?.Dispose();

                    if (!filePathEnumerator.MoveNext()) { return false; }
                    if (filePathEnumerator.Current == null) { continue; }

                    currentFileStream = new BinaryReader(File.OpenRead(filePathEnumerator.Current));
                    return true;
                }
                catch { }
            }
        }
    }
}
