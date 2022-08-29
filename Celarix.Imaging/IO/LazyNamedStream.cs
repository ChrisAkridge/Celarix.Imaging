using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Celarix.Imaging.IO
{
    internal sealed class LazyNamedStream : IDisposable
    {
        public string Name { get; set; }
        public Stream Stream { get; set; }
        
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}