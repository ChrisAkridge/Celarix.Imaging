using Celarix.Imaging.ByteViewObservatory.Core.Sources;

namespace Celarix.Imaging.ByteViewObservatory.Test.Sources;

public sealed class ByteSourceChunkMathTests
{
    [Fact]
    public void ChunkSizesMatchTheTwoByteVOFamilies()
    {
        Assert.Equal(1_048_576, ByteSourceChunkSizes.ForFamily(ByteSourceFamily.BinaryPowerOfTwo));
        Assert.Equal(786_432, ByteSourceChunkSizes.ForFamily(ByteSourceFamily.TernaryMultiple));
    }

    [Theory]
    [InlineData(0UL, 1_048_576, 0UL)]
    [InlineData(1UL, 1_048_576, 1UL)]
    [InlineData(1_048_576UL, 1_048_576, 1UL)]
    [InlineData(1_048_577UL, 1_048_576, 2UL)]
    public void ChunkCountRoundsUpWithoutReadingTheSource(
        ulong length,
        int chunkSize,
        ulong expectedCount)
    {
        Assert.Equal(expectedCount, ByteSourceChunkMath.GetChunkCount(length, chunkSize));
    }

    [Fact]
    public void MaximumUlongLengthDoesNotOverflowChunkCount()
    {
        ulong chunkCount = ByteSourceChunkMath.GetChunkCount(
            ulong.MaxValue,
            ByteSourceChunkSizes.BinaryPowerOfTwo);

        Assert.Equal(
            ((ulong.MaxValue - 1) / ByteSourceChunkSizes.BinaryPowerOfTwo) + 1,
            chunkCount);
    }

    [Fact]
    public void FinalChunkReportsOnlyTheLogicalBytes()
    {
        Assert.Equal(
            17,
            ByteSourceChunkMath.GetBytesInChunk(
                length: 1_048_593,
                alignedOffset: 1_048_576,
                chunkSize: 1_048_576));

        Assert.Equal(
            0,
            ByteSourceChunkMath.GetBytesInChunk(
                length: 1_048_593,
                alignedOffset: 2_097_152,
                chunkSize: 1_048_576));
    }

    [Fact]
    public void UnalignedOffsetsAreRejected()
    {
        Assert.Throws<ArgumentException>(() =>
            ByteSourceChunkMath.ValidateAlignedOffset(1, 1_048_576));
    }
}
