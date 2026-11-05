namespace Celarix.Imaging.ByteViewObservatory.Core.Sources;

/// <summary>
/// Metadata captured when a source is opened.
/// </summary>
public readonly record struct SourceSnapshot(
    ulong Length,
    DateTime? LastWriteTimeUtc,
    string? Identifier);

public enum SourceChangeState
{
    Unchanged,
    Changed,
    Unavailable
}

public readonly record struct SourceChangeResult(
    SourceChangeState State,
    SourceSnapshot? CurrentSnapshot,
    string? Message)
{
    public bool HasChanged => State == SourceChangeState.Changed;
}
