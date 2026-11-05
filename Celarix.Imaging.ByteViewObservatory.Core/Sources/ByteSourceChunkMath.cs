namespace Celarix.Imaging.ByteViewObservatory.Core.Sources;

public static class ByteSourceChunkMath
{
    public static ulong GetChunkCount(ulong length, int chunkSize)
    {
        ValidateChunkSize(chunkSize);

        return length == 0
            ? 0
            : ((length - 1) / (ulong)chunkSize) + 1;
    }

    public static ulong GetChunkOffset(ulong chunkIndex, int chunkSize)
    {
        ValidateChunkSize(chunkSize);

        ulong size = (ulong)chunkSize;
        if (chunkIndex > ulong.MaxValue / size)
        {
            throw new OverflowException("The chunk offset cannot be represented by ulong.");
        }

        return chunkIndex * size;
    }

    public static int GetBytesInChunk(ulong length, ulong alignedOffset, int chunkSize)
    {
        ValidateAlignedOffset(alignedOffset, chunkSize);

        if (alignedOffset >= length)
        {
            return 0;
        }

        ulong remaining = length - alignedOffset;
        return (int)Math.Min(remaining, (ulong)chunkSize);
    }

    public static void ValidateAlignedOffset(ulong alignedOffset, int chunkSize)
    {
        ValidateChunkSize(chunkSize);

        if (alignedOffset % (ulong)chunkSize != 0)
        {
            throw new ArgumentException(
                $"The offset must be aligned to the {chunkSize}-byte destination size.",
                nameof(alignedOffset));
        }
    }

    private static void ValidateChunkSize(int chunkSize) => ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSize);
}
