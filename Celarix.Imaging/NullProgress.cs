using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging
{
    internal sealed class NullProgress<T> : IProgress<T>
    {
        public void Report(T value)
        {
            // Do nothing
        }
    }
}
