using Celarix.Imaging.ByteViewObservatory.Core.Sources;

namespace Celarix.Imaging.ByteViewObservatory.Test.Sources;

public sealed class PhysicalFileByteSourceTests
{
    [Fact]
    public async Task PhysicalSourceReadsTheFinalChunkAndPadsTheDestination()
    {
        string path = Path.GetTempFileName();
        try
        {
            await File.WriteAllBytesAsync(path, Enumerable.Range(0, 21).Select(i => (byte)i).ToArray());
            var source = new PhysicalFileByteSource(path);
            byte[] destination = Enumerable.Repeat((byte)0xFF, 16).ToArray();

            SourceReadResult result = await source.ReadChunkAsync(16, destination);

            Assert.True(result.Succeeded);
            Assert.Equal(5, result.BytesWritten);
            Assert.Equal(new byte[] { 16, 17, 18, 19, 20 }, destination[..5]);
            Assert.All(destination[5..], value => Assert.Equal(0, value));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void PhysicalSourceReportsFileMetadataChanges()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(path, [1, 2, 3]);
            var source = new PhysicalFileByteSource(path);

            File.WriteAllBytes(path, [1, 2, 3, 4]);

            SourceChangeResult change = source.CheckForChanges();

            Assert.True(change.HasChanged);
            Assert.Equal(SourceChangeState.Changed, change.State);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
