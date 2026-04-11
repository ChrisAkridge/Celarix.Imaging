using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.IO.v2
{
    public abstract class NamedStream<T>
    {
        public abstract string CurrentFileName { get; }
        public abstract int FileNumber { get; }
        public abstract int TotalFiles { get; }
        public abstract long Position { get; }
        public abstract long TotalLength { get; }

        public abstract NamedStreamReadResult Read(T[] buffer, out int elementsRead);
    }
}
