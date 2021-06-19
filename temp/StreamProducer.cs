using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.ByteView
{
	public sealed class StreamProducer : IEnumerable<Stream>
    {
        private readonly IList<string> filePaths;

        public StreamProducer(IList<string> filePaths) => this.filePaths = filePaths;

        public IEnumerator<Stream> GetEnumerator()
        {
            foreach (string filePath in filePaths)
            {
                Stream stream;
                try { stream = File.OpenRead(filePath); }
                catch (Exception) { continue; }

                yield return stream;
            }
        }

		IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
	}
}
