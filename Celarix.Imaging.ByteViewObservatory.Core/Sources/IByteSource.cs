namespace Celarix.Imaging.ByteViewObservatory.Core.Sources;

/// <summary>
/// Provides deterministic, read-only aligned chunks of a logical byte source.
/// </summary>
public interface IByteSource
{
    /// <summary>
    /// Gets the logical length of the source. Bytes beyond this length are padding,
    /// not part of the source.
    /// </summary>
    ulong Length { get; }

    SourceSnapshot Snapshot { get; }

    /// <summary>
    /// Fills one aligned chunk. The destination is cleared before the source writes
    /// into it; bytes after <see cref="SourceReadResult.BytesWritten"/> are padding.
    /// The destination length selects the chunk size, and the offset must be aligned
    /// to that length. This method does not assemble reads across chunk boundaries.
    /// </summary>
    ValueTask<SourceReadResult> ReadChunkAsync(
        ulong alignedOffset,
        Memory<byte> destination,
        CancellationToken cancellationToken = default);

    SourceChangeResult CheckForChanges();
}
