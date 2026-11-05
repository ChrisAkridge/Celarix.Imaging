namespace Celarix.Imaging.ByteViewObservatory.Core.Geometry;

public readonly record struct BlockPixelGeometry(int Width, int Height)
{
    public int PixelCount => Width * Height;
}

/// <summary>
/// Decodes the 24 bits in one ternary-family three-byte block into raster-ordered
/// pixel values. The first pixel consumes the block's first three, six, twelve,
/// or twenty-four bits.
/// </summary>
public static class TernaryBlockPacking
{
    public static BlockPixelGeometry GetPixelGeometry(int bitsPerPixel) => bitsPerPixel switch
    {
        3 => new BlockPixelGeometry(4, 2),
        6 => new BlockPixelGeometry(2, 2),
        12 => new BlockPixelGeometry(2, 1),
        24 => new BlockPixelGeometry(1, 1),
        _ => throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel,
            "Ternary blocks support 3, 6, 12, or 24 bits per pixel.")
    };

    public static void DecodeBlock(
        ReadOnlySpan<byte> block,
        int bitsPerPixel,
        Span<uint> destination)
    {
        if (block.Length < 3)
        {
            throw new ArgumentException("A ternary block must contain at least three bytes.", nameof(block));
        }

        BlockPixelGeometry geometry = GetPixelGeometry(bitsPerPixel);
        if (destination.Length < geometry.PixelCount)
        {
            throw new ArgumentException(
                $"The destination must contain at least {geometry.PixelCount} pixel values.",
                nameof(destination));
        }

        uint packed = (uint)(block[0] << 16 | block[1] << 8 | block[2]);
        uint mask = (1u << bitsPerPixel) - 1;
        for (int pixelIndex = 0; pixelIndex < geometry.PixelCount; pixelIndex++)
        {
            int shift = 24 - ((pixelIndex + 1) * bitsPerPixel);
            destination[pixelIndex] = (packed >> shift) & mask;
        }
    }
}
