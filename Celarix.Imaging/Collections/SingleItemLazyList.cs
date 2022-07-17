using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Celarix.Imaging.Collections
{
    /// <summary>
    /// Contains a list of factory objects that create a given value on demand.
    /// </summary>
    public sealed class SingleItemLazyList<T> where T : IDisposable
    {
        private int currentItemIndex = -1;
        private T currentItem;
        private readonly List<Func<T>> itemFactories;

        public SingleItemLazyList() =>
            itemFactories = new List<Func<T>>();

        public SingleItemLazyList(IEnumerable<Func<T>> itemFactories) =>
            this.itemFactories = itemFactories.ToList();

        public T GetItem(int index)
        {
            if (index < 0 || index > itemFactories.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (currentItemIndex == index)
            {
                return currentItem;
            }
            
            currentItem?.Dispose();
            currentItem = itemFactories[index]();
            currentItemIndex = index;

            return currentItem;
        }

        public void SetItemFactory(int index, Func<T> itemFactory)
        {
            if (index < 0 || index > itemFactories.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            itemFactories[index] = itemFactory;
        }

        public void Add(Func<T> itemFactory)
        {
            itemFactories.Add(itemFactory);
        }
    }
}
