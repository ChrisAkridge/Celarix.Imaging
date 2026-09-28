using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.Collections
{
    public sealed class ReadOnlyResettableEnumerable<T> : IResettableEnumerable<T>
    {
        private readonly IReadOnlyList<T> _items;
        private int _index;

        public ReadOnlyResettableEnumerable(IReadOnlyList<T> items)
        {
            _items = items;
            _index = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (_index < _items.Count)
            {
                yield return _items[_index++];
            }
        }

        public void Reset()
        {
            _index = 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
