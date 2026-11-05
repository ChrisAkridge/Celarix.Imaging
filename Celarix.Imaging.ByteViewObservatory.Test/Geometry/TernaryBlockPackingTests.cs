using Celarix.Imaging.ByteViewObservatory.Core.Geometry;

namespace Celarix.Imaging.ByteViewObservatory.Test.Geometry;

public sealed class TernaryBlockPackingTests
{
    [Theory]
    [InlineData(3, 4, 2)]
    [InlineData(6, 2, 2)]
    [InlineData(12, 2, 1)]
    [InlineData(24, 1, 1)]
    public void BlockGeometryMatchesTheTernaryLayouts(int bitsPerPixel, int width, int height)
    {
        Assert.Equal(new BlockPixelGeometry(width, height),
            TernaryBlockPacking.GetPixelGeometry(bitsPerPixel));
    }

    [Fact]
    public void ThreeBppValuesFollowTheBlockBitstreamInRasterOrder()
    {
        uint[] values = new uint[8];

        TernaryBlockPacking.DecodeBlock([0x05, 0x39, 0x77], 3, values);

        Assert.Equal(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 }, values);
    }

    [Fact]
    public void TwentyFourBppUsesTheWholeThreeByteBlock()
    {
        uint[] value = new uint[1];

        TernaryBlockPacking.DecodeBlock([0x12, 0x34, 0x56], 24, value);

        Assert.Equal(0x123456u, value[0]);
    }
}
