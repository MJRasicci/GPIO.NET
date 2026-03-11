namespace Motif.CLI;

internal sealed class BatchRoundTripFileResult
{
    public required string File { get; init; }

    public required string RelativePath { get; init; }

    public bool GpifBytesIdentical { get; init; }

    public int DiagnosticCount { get; init; }

    public int WarningCount { get; init; }

    public int InfoCount { get; init; }

    public BatchNamedCount[] DiagnosticCodeCounts { get; init; } = Array.Empty<BatchNamedCount>();

    public BatchNamedCount[] DiagnosticSectionCounts { get; init; } = Array.Empty<BatchNamedCount>();
}
