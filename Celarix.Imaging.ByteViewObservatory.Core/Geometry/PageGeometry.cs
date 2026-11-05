namespace Celarix.Imaging.ByteViewObservatory.Core.Geometry;

public readonly record struct PageGeometry(
    int BitsPerPixel,
    int Width,
    int Height,
    int BytesPerPage,
    bool IsTernaryMultiple)
{
    public long PixelCount => (long)Width * Height;

    public static PageGeometry ForBitsPerPixel(int bitsPerPixel) => bitsPerPixel switch
    {
        1 => new(1, 4096, 2048, 1_048_576, false),
        2 => new(2, 2048, 2048, 1_048_576, false),
        3 => new(3, 2048, 1024, 786_432, true),
        4 => new(4, 2048, 1024, 1_048_576, false),
        6 => new(6, 1024, 1024, 786_432, true),
        8 => new(8, 1024, 1024, 1_048_576, false),
        12 => new(12, 1024, 512, 786_432, true),
        16 => new(16, 1024, 512, 1_048_576, false),
        24 => new(24, 512, 512, 786_432, true),
        32 => new(32, 512, 512, 1_048_576, false),
        64 => new(64, 512, 256, 1_048_576, false),
        128 => new(128, 256, 256, 1_048_576, false),
        256 => new(256, 256, 128, 1_048_576, false),
        _ => throw new ArgumentOutOfRangeException(nameof(bitsPerPixel), bitsPerPixel,
            "The bit depth does not have an established ByteVO page geometry.")
    };
}
