namespace Motif.Extensions.GuitarPro.Abstractions;

using Motif;
using Motif.Models;

/// <summary>
/// High-level entry point for reading a Guitar Pro file and returning a mapped domain score.
/// </summary>
public interface IGuitarProReader : IScoreReader, IPathScoreReader
{
    new ValueTask<Score> ReadAsync(Stream source, CancellationToken cancellationToken = default);

    new ValueTask<Score> ReadAsync(string filePath, CancellationToken cancellationToken = default);
}
