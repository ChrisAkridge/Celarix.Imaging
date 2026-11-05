using Celarix.Imaging.ByteViewObservatory.Core.Bits;

namespace Celarix.Imaging.ByteViewObservatory.Test.Bits;

public sealed class BitPackingTests
{
    [Fact]
    public void BitIndexZeroIsTheMostSignificantBit()
    {
        byte[] source = [0b1000_0000];

        Assert.True(BitPacking.GetBit(source, 0, 1, new BitAddress(0, 0)));
        Assert.False(BitPacking.GetBit(source, 0, 1, new BitAddress(0, 7)));
    }

    [Fact]
    public void FirstConsumedBitBecomesNumericMostSignificantBit()
    {
        byte[] source = [0b1011_0010];

        ulong value = BitPacking.ReadUInt64(
            source,
            sourceOffset: 0,
            validBytes: 1,
            start: new BitAddress(0, 1),
            bitCount: 5);

        Assert.Equal(0b01100UL, value);
    }

    [Fact]
    public void ReadsContinueMsbFirstAcrossByteBoundaries()
    {
        byte[] source = [0b1111_0000, 0b1010_0000];

        ulong value = BitPacking.ReadUInt64(
            source,
            sourceOffset: 0,
            validBytes: 2,
            start: new BitAddress(0, 4),
            bitCount: 8);

        Assert.Equal(0b0000_1010UL, value);
    }

    [Fact]
    public void LittleEndianReversesGroupedBytesOnly()
    {
        byte[] source = [0x12, 0x34];

        ulong value = BitPacking.ReadUInt64(
            source,
            sourceOffset: 0,
            validBytes: 2,
            start: new BitAddress(0, 0),
            bitCount: 16,
            byteOrder: ByteOrder.LittleEndian);

        Assert.Equal(0x3412UL, value);
    }

    [Fact]
    public void UnavailableBitsAreZeroPadded()
    {
        byte[] source = [0x80];

        ulong value = BitPacking.ReadUInt64(
            source,
            sourceOffset: 0,
            validBytes: 1,
            start: new BitAddress(0, 0),
            bitCount: 16);

        Assert.Equal(0x8000UL, value);
    }

    [Fact]
    public void LittleEndianRequiresWholeBytes()
    {
        Assert.Throws<ArgumentException>(() => BitPacking.ReadUInt64(
            [0xFF],
            sourceOffset: 0,
            validBytes: 1,
            start: new BitAddress(0, 0),
            bitCount: 4,
            byteOrder: ByteOrder.LittleEndian));
    }

    [Fact]
    public void BitAddressCanDetectOverflowWithoutWrapping()
    {
        var address = new BitAddress(ulong.MaxValue, 7);

        Assert.False(address.TryAdvance(1, out _));
        Assert.Throws<OverflowException>(() => address.Advance(1));
    }
}
