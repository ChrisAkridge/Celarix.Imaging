using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Celarix.Imaging.IO
{
    internal sealed class NamedStream
    {
        public string Name { get; set; }
        public int StreamIndex { get; set; }
        public long Offset { get; set; }
        public long Length { get; set; }
    }
}
