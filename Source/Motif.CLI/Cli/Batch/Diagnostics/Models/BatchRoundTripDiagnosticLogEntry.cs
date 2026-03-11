namespace Motif.CLI;

internal sealed class BatchRoundTripDiagnosticLogEntry
{
    public required string File { get; init; }

    public required string RelativePath { get; init; }

    public required string Code { get; init; }

    public required string Category { get; init; }

    public required string Severity { get; init; }

    public required string Message { get; init; }

    public string? Path { get; init; }

    public string? SourceValue { get; init; }

    public string? OutputValue { get; init; }
}
