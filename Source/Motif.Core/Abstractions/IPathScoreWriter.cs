namespace Motif;

using Motif.Models;

/// <summary>
/// Adds path-based score writes for formats whose file semantics differ from raw stream writes.
/// </summary>
public interface IPathScoreWriter
{
    /// <summary>
    /// Writes the score to the provided file path.
    /// </summary>
    /// <param name="score">The score to serialize.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="cancellationToken">Cancels the write operation.</param>
    /// <returns>A task-like handle that completes when serialization finishes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="score"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, or whitespace.</exception>
    ValueTask WriteAsync(Score score, string filePath, CancellationToken cancellationToken = default);
}
