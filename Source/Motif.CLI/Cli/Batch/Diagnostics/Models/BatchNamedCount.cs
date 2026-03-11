namespace Motif.CLI;

internal sealed class BatchNamedCount
{
    public required string Name { get; init; }

    public int Count { get; init; }

    public int FileCount { get; init; }

    public string[] SampleFiles { get; init; } = Array.Empty<string>();
}
