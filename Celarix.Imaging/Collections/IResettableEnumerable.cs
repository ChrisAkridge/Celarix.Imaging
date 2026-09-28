using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.Collections
{
    public interface IResettableEnumerable<out T> : IEnumerable<T>
    {
        void Reset();
    }
}
