using Celarix.Imaging.ByteViewObservatory.Core.Geometry;

namespace Celarix.Imaging.ByteViewObservatory.Test.Geometry;

public sealed class PageGeometryTests
{
    [Theory]
    [InlineData(1, 4096, 2048, 1_048_576, false)]
    [InlineData(2, 2048, 2048, 1_048_576, false)]
    [InlineData(3, 2048, 1024, 786_432, true)]
    [InlineData(4, 2048, 1024, 1_048_576, false)]
    [InlineData(6, 1024, 1024, 786_432, true)]
    [InlineData(8, 1024, 1024, 1_048_576, false)]
    [InlineData(12, 1024, 512, 786_432, true)]
    [InlineData(16, 1024, 512, 1_048_576, false)]
    [InlineData(24, 512, 512, 786_432, true)]
    [InlineData(32, 512, 512, 1_048_576, false)]
    [InlineData(64, 512, 256, 1_048_576, false)]
    [InlineData(128, 256, 256, 1_048_576, false)]
    [InlineData(256, 256, 128, 1_048_576, false)]
    public void EstablishedPageGeometryIsPreserved(
        int bitsPerPixel,
        int width,
        int height,
        int bytesPerPage,
        bool isTernaryMultiple)
    {
        PageGeometry geometry = PageGeometry.ForBitsPerPixel(bitsPerPixel);

        Assert.Equal(width, geometry.Width);
        Assert.Equal(height, geometry.Height);
        Assert.Equal(bytesPerPage, geometry.BytesPerPage);
        Assert.Equal(isTernaryMultiple, geometry.IsTernaryMultiple);
        Assert.Equal((long)width * height, geometry.PixelCount);
    }
}
