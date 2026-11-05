namespace Celarix.Imaging.ByteViewObservatory.Core.Sources;

public delegate void FillSourceChunk(ulong alignedOffset, byte[] buffer);

/// <summary>
/// Exposes a deterministic, lazily generated virtual byte source.
/// </summary>
public sealed class SyntheticByteSource : IByteSource
{
    private readonly FillSourceChunk fillChunk;

    public SyntheticByteSource(ulong length, FillSourceChunk fillChunk, string? identifier = null)
    {
        ArgumentNullException.ThrowIfNull(fillChunk);

        Length = length;
        this.fillChunk = fillChunk;
        Snapshot = new SourceSnapshot(length, null, identifier);
    }

    public ulong Length { get; }

    public SourceSnapshot Snapshot { get; }

    public ValueTask<SourceReadResult> ReadChunkAsync(
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
            return ValueTask.FromResult(SourceReadResult.Success(0));
        }

        var generatedChunk = new byte[destination.Length];
        try
        {
            fillChunk(alignedOffset, generatedChunk);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return ValueTask.FromResult(SourceReadResult.Failure(
                new SourceReadError(SourceReadErrorKind.GeneratorFailed, exception.Message)));
        }

        generatedChunk.AsSpan(0, bytesInChunk).CopyTo(destination.Span);
        return ValueTask.FromResult(SourceReadResult.Success(bytesInChunk));
    }

    public SourceChangeResult CheckForChanges() => new(
        SourceChangeState.Unchanged,
        Snapshot,
        null);
}
