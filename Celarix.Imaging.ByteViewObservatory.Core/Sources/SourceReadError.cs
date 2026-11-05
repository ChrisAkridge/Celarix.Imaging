namespace Celarix.Imaging.ByteViewObservatory.Core.Sources;

public enum SourceReadErrorKind
{
    SourceUnavailable,
    ReadFailed,
    GeneratorFailed
}

/// <summary>
/// Describes an expected source failure without using an exception as control flow.
/// </summary>
public sealed record SourceReadError(SourceReadErrorKind Kind, string Message);
