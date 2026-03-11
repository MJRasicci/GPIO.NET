namespace Motif.CLI;

internal sealed class BatchPathSummary
{
    public required string Code { get; init; }

    public required string Path { get; init; }

    public int Count { get; init; }

    public int FileCount { get; init; }

    public string[] SampleFiles { get; init; } = Array.Empty<string>();
}
