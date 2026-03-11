namespace Motif.CLI;

internal sealed class BatchFileHeadline
{
    public required string RelativePath { get; init; }

    public int DiagnosticCount { get; init; }
}
