using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.Extensions
{
	internal static class EnumerableExtensions
	{
        public static IEnumerable<bool> ToBitEnumerable(this IEnumerable<byte> sequence)
        {
            foreach (var item in sequence)
            {
                yield return (item & 0x80) != 0;
                yield return (item & 0x40) != 0;
                yield return (item & 0x20) != 0;
                yield return (item & 0x10) != 0;
                yield return (item & 0x08) != 0;
                yield return (item & 0x04) != 0;
                yield return (item & 0x02) != 0;
                yield return (item & 0x01) != 0;
            }
        }

        public static IEnumerable<T> PadToAlignment<T>(this IEnumerable<T> sequence,
            long alignment,
            T defaultValue = default)
        {
            var seenItems = 0L;

            foreach (T item in sequence)
            {
                seenItems += 1;
                yield return item;
            }

            while (seenItems % alignment != 0)
            {
                seenItems += 1;
                yield return defaultValue;
            }
        }

        public static IEnumerable<IList<T>> ChunkIntoLists<T>(this IEnumerable<T> sequence, int chunkSize)
        {
            var list = new List<T>(chunkSize);

            foreach (var item in sequence)
            {
                list.Add(item);

                if (list.Count != chunkSize) { continue; }

                yield return list;

                list.Clear();
            }
        }

        public static int BitsToInt(this IList<bool> bits)
        {
            var result = 0;

            for (var i = 0; i < bits.Count; i++)
            {
                int bitPlace = bits.Count - 1 - i;
                if (bits[i]) { result |= (1 << bitPlace); }
            }

            return result;
        }
    }
}
