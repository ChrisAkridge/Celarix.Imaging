namespace Celarix.Imaging.ByteViewObservatory.Core.Bits;

/// <summary>
/// Reads bit sequences in file order. The first consumed bit is the numeric MSB.
/// </summary>
public static class BitPacking
{
    public static bool GetBit(
        ReadOnlySpan<byte> source,
        ulong sourceOffset,
        int validBytes,
        BitAddress address)
    {
        ValidateValidByteCount(source, validBytes);

        if (address.ByteOffset < sourceOffset)
        {
            return false;
        }

        ulong relativeByteOffset = address.ByteOffset - sourceOffset;
        if (relativeByteOffset >= (ulong)validBytes ||
            relativeByteOffset >= (ulong)source.Length)
        {
            return false;
        }

        int byteIndex = (int)relativeByteOffset;
        byte mask = (byte)(0x80 >> address.BitIndex);
        return (source[byteIndex] & mask) != 0;
    }

    public static ulong ReadUInt64(
        ReadOnlySpan<byte> source,
        ulong sourceOffset,
        int validBytes,
        BitAddress start,
        int bitCount,
        ByteOrder byteOrder = ByteOrder.BigEndian)
    {
        if (bitCount is < 0 or > 64)
        {
            throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount,
                "A UInt64 read must contain between 0 and 64 bits.");
        }

        return (ulong)ReadUnsigned(
            source,
            sourceOffset,
            validBytes,
            start,
            bitCount,
            byteOrder);
    }

    public static UInt128 ReadUInt128(
        ReadOnlySpan<byte> source,
        ulong sourceOffset,
        int validBytes,
        BitAddress start,
        int bitCount,
        ByteOrder byteOrder = ByteOrder.BigEndian)
    {
        if (bitCount is < 0 or > 128)
        {
            throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount,
                "A UInt128 read must contain between 0 and 128 bits.");
        }

        return ReadUnsigned(source, sourceOffset, validBytes, start, bitCount, byteOrder);
    }

    /// <summary>
    /// Copies an arbitrary bit sequence into a byte buffer in MSB-first order.
    /// The destination must be exactly large enough to hold the requested bits,
    /// rounded up to a whole byte. Unavailable source bits remain zero.
    /// </summary>
    public static void CopyBits(
        ReadOnlySpan<byte> source,
        ulong sourceOffset,
        int validBytes,
        BitAddress start,
        int bitCount,
        Span<byte> destination,
        ByteOrder byteOrder = ByteOrder.BigEndian)
    {
        ValidateValidByteCount(source, validBytes);

        if (bitCount < 0 || bitCount > int.MaxValue - 7)
        {
            throw new ArgumentOutOfRangeException(nameof(bitCount));
        }

        int destinationLength = (bitCount + 7) / 8;
        if (destination.Length != destinationLength)
        {
            throw new ArgumentException(
                $"The destination must contain exactly {destinationLength} bytes.",
                nameof(destination));
        }

        if (byteOrder == ByteOrder.LittleEndian && bitCount % 8 != 0)
        {
            throw new ArgumentException(
                "Little-endian interpretation requires a whole number of bytes.",
                nameof(bitCount));
        }

        destination.Clear();
        for (int bitIndex = 0; bitIndex < bitCount; bitIndex++)
        {
            if (!start.TryAdvance(bitIndex, out BitAddress address) ||
                !GetBit(source, sourceOffset, validBytes, address))
            {
                continue;
            }

            int destinationByteIndex = bitIndex / 8;
            int destinationBitIndex = bitIndex % 8;
            destination[destinationByteIndex] |= (byte)(0x80 >> destinationBitIndex);
        }

        if (byteOrder == ByteOrder.LittleEndian)
        {
            destination.Reverse();
        }
    }

    private static UInt128 ReadUnsigned(
        ReadOnlySpan<byte> source,
        ulong sourceOffset,
        int validBytes,
        BitAddress start,
        int bitCount,
        ByteOrder byteOrder)
    {
        Span<byte> bytes = stackalloc byte[(bitCount + 7) / 8];
        CopyBits(source, sourceOffset, validBytes, start, bitCount, bytes, byteOrder);

        UInt128 result = 0;
        foreach (byte value in bytes)
        {
            result = (result << 8) | value;
        }

        int paddingBits = bytes.Length * 8 - bitCount;
        return paddingBits == 0 ? result : result >> paddingBits;
    }

    private static void ValidateValidByteCount(ReadOnlySpan<byte> source, int validBytes)
    {
        if (validBytes < 0 || validBytes > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(validBytes));
        }
    }
}
