using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Celarix.Imaging.IO
{
	internal sealed class NamedStream
	{
		public string Name { get; }
		public Stream Stream { get; }
		public long Offset { get; }

        public NamedStream(string name, Stream stream, long offset)
        {
            Name = name;
            Stream = stream;
            Offset = offset;
        }
	}
}
