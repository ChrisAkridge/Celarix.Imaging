namespace Celarix.Imaging.ByteViewObservatory.Core.Bits;

/// <summary>
/// An absolute position within a byte source. Bit index 0 is the byte's MSB;
/// bit index 7 is its LSB.
/// </summary>
public readonly record struct BitAddress
{
    public BitAddress(ulong byteOffset, int bitIndex)
    {
        if (bitIndex is < 0 or > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(bitIndex), bitIndex,
                "A bit index must be between 0 and 7.");
        }

        ByteOffset = byteOffset;
        BitIndex = (byte)bitIndex;
    }

    public ulong ByteOffset { get; }

    public byte BitIndex { get; }

    public bool TryAdvance(int bitCount, out BitAddress result)
    {
        if (bitCount < 0)
        {
            result = default;
            return false;
        }

        ulong totalBitIndex = (ulong)BitIndex + (ulong)bitCount;
        ulong byteAdvance = totalBitIndex / 8;
        if (ByteOffset > ulong.MaxValue - byteAdvance)
        {
            result = default;
            return false;
        }

        result = new BitAddress(
            ByteOffset + byteAdvance,
            (int)(totalBitIndex % 8));
        return true;
    }

    public BitAddress Advance(int bitCount)
    {
        if (!TryAdvance(bitCount, out BitAddress result))
        {
            throw new OverflowException("The bit address cannot be advanced by the requested amount.");
        }

        return result;
    }
}
