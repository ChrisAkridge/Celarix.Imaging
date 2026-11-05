namespace Celarix.Imaging.ByteViewObservatory.Core.Sources;

/// <summary>
/// Describes the result of filling one aligned source chunk.
/// </summary>
public readonly record struct SourceReadResult
{
    private SourceReadResult(int bytesWritten, SourceReadError? error)
    {
        BytesWritten = bytesWritten;
        Error = error;
    }

    public int BytesWritten { get; }

    public SourceReadError? Error { get; }

    public bool Succeeded => Error is null;

    public static SourceReadResult Success(int bytesWritten)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bytesWritten);

        return new SourceReadResult(bytesWritten, null);
    }

    public static SourceReadResult Failure(SourceReadError error, int bytesWritten = 0)
    {
        ArgumentNullException.ThrowIfNull(error);

        ArgumentOutOfRangeException.ThrowIfNegative(bytesWritten);

        return new SourceReadResult(bytesWritten, error);
    }
}
