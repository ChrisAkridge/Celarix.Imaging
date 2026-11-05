using System.IO;

namespace Celarix.Imaging.ByteViewObservatory.Core.Sources;

/// <summary>
/// Provides aligned chunks from one physical file without materializing the file.
/// </summary>
public sealed class PhysicalFileByteSource : IByteSource
{
    private readonly string filePath;

    public PhysicalFileByteSource(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException("The source file was not found.", filePath);
        }

        if (fileInfo.Length < 0)
        {
            throw new IOException("The source file reported a negative length.");
        }

        this.filePath = fileInfo.FullName;
        Length = (ulong)fileInfo.Length;
        Snapshot = CreateSnapshot(fileInfo);
    }

    public ulong Length { get; }

    public SourceSnapshot Snapshot { get; }

    public async ValueTask<SourceReadResult> ReadChunkAsync(
        ulong alignedOffset,
        Memory<byte> destination,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (destination.Length == 0)
        {
            throw new ArgumentException("The destination must not be empty.", nameof(destination));
        }

        ByteSourceChunkMath.ValidateAlignedOffset(alignedOffset, destination.Length);
        destination.Span.Clear();

        int bytesInChunk = ByteSourceChunkMath.GetBytesInChunk(
            Length,
            alignedOffset,
            destination.Length);

        if (bytesInChunk == 0)
        {
            return SourceReadResult.Success(0);
        }

        try
        {
            await using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete,
                bufferSize: 64 * 1024,
                options: FileOptions.Asynchronous | FileOptions.RandomAccess);

            stream.Position = checked((long)alignedOffset);

            int bytesWritten = 0;
            while (bytesWritten < bytesInChunk)
            {
                int bytesRead = await stream.ReadAsync(
                    destination[bytesWritten..bytesInChunk],
                    cancellationToken);

                if (bytesRead == 0)
                {
                    break;
                }

                bytesWritten += bytesRead;
            }

            return SourceReadResult.Success(bytesWritten);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return SourceReadResult.Failure(
                new SourceReadError(SourceReadErrorKind.ReadFailed, exception.Message));
        }
    }

    public SourceChangeResult CheckForChanges()
    {
        try
        {
            var currentInfo = new FileInfo(filePath);
            if (!currentInfo.Exists)
            {
                return new SourceChangeResult(
                    SourceChangeState.Unavailable,
                    null,
                    "The source file is no longer available.");
            }

            var currentSnapshot = CreateSnapshot(currentInfo);
            bool changed = currentSnapshot.Length != Snapshot.Length ||
                currentSnapshot.LastWriteTimeUtc != Snapshot.LastWriteTimeUtc;

            return new SourceChangeResult(
                changed ? SourceChangeState.Changed : SourceChangeState.Unchanged,
                currentSnapshot,
                changed ? "The source file metadata has changed." : null);
        }
        catch (Exception exception)
        {
            return new SourceChangeResult(
                SourceChangeState.Unavailable,
                null,
                exception.Message);
        }
    }

    private static SourceSnapshot CreateSnapshot(FileInfo fileInfo) => new(
        (ulong)fileInfo.Length,
        fileInfo.LastWriteTimeUtc,
        fileInfo.FullName);
}
