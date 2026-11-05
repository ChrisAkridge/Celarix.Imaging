using Celarix.Imaging.ByteViewObservatory.Core.Sources;

namespace Celarix.Imaging.ByteViewObservatory.Test.Sources;

public sealed class SyntheticByteSourceTests
{
    [Fact]
    public async Task FinalChunkIsZeroPaddedButReportsOnlyValidBytes()
    {
        const int chunkSize = 16;
        var source = new SyntheticByteSource(
            length: 21,
            (offset, buffer) =>
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)(offset + (ulong)i);
                }
            });

        byte[] destination = Enumerable.Repeat((byte)0xFF, chunkSize).ToArray();
        SourceReadResult result = await source.ReadChunkAsync(16, destination);

        Assert.True(result.Succeeded);
        Assert.Equal(5, result.BytesWritten);
        Assert.Equal(new byte[] { 16, 17, 18, 19, 20 }, destination[..5]);
        Assert.All(destination[5..], value => Assert.Equal(0, value));
    }

    [Fact]
    public async Task GeneratorFailureIsReturnedAndDestinationRemainsPadding()
    {
        var source = new SyntheticByteSource(
            length: 32,
            (_, _) => throw new InvalidOperationException("synthetic failure"));

        byte[] destination = Enumerable.Repeat((byte)0xFF, 16).ToArray();
        SourceReadResult result = await source.ReadChunkAsync(0, destination);

        Assert.False(result.Succeeded);
        Assert.Equal(SourceReadErrorKind.GeneratorFailed, result.Error!.Kind);
        Assert.Equal(0, result.BytesWritten);
        Assert.All(destination, value => Assert.Equal(0, value));
    }

    [Fact]
    public void SyntheticSourceSupportsTheMaximumLogicalLength()
    {
        var source = new SyntheticByteSource(ulong.MaxValue, (_, _) => { });

        Assert.Equal(ulong.MaxValue, source.Length);
        Assert.Equal(SourceChangeState.Unchanged, source.CheckForChanges().State);
    }
}
