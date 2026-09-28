using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.IO.v2
{
    public sealed class NamedStreamFactory
    {
        public string FilePath { get; }
        public Func<Stream> StreamFactory { get; }

        public NamedStreamFactory(string filePath)
        {
            FilePath = filePath;
            StreamFactory = () => new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}
