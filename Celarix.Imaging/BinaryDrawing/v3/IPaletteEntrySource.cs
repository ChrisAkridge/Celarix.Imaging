using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.BinaryDrawing.v3
{
    internal interface IPaletteEntrySource
    {
        long TotalEntries { get; }
        long EntriesRead { get; }
        PaletteEntryReadResult Read(Span<int> destination);
    }
}
