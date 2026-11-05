namespace Celarix.Imaging.ByteViewObservatory.Core.Sources;

/// <summary>
/// Defines the initial aligned source chunk sizes used by ByteView Observatory.
/// </summary>
public static class ByteSourceChunkSizes
{
    public const int BinaryPowerOfTwo = 1 * 1024 * 1024;
    public const int TernaryMultiple = 768 * 1024;

    public static int ForFamily(ByteSourceFamily family) => family switch
    {
        ByteSourceFamily.BinaryPowerOfTwo => BinaryPowerOfTwo,
        ByteSourceFamily.TernaryMultiple => TernaryMultiple,
        _ => throw new ArgumentOutOfRangeException(nameof(family), family, "Unknown source family.")
    };
}
