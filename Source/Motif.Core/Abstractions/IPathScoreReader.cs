namespace Motif;

using Motif.Models;

/// <summary>
/// Adds path-based score reads for formats whose file semantics differ from raw stream reads.
/// </summary>
public interface IPathScoreReader
{
    /// <summary>
    /// Reads a score from the provided file path.
    /// </summary>
    /// <param name="filePath">The path to the encoded score file.</param>
    /// <param name="cancellationToken">Cancels the read operation.</param>
    /// <returns>A mapped <see cref="Score"/> instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, or whitespace.</exception>
    ValueTask<Score> ReadAsync(string filePath, CancellationToken cancellationToken = default);
}
