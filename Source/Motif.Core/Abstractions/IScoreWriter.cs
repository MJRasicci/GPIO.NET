namespace Motif;

using Motif.Models;

/// <summary>
/// Writes a Motif domain score to encoded format-specific output.
/// </summary>
public interface IScoreWriter
{
    /// <summary>
    /// Writes the score to the provided destination stream.
    /// Implementations must not dispose the caller-owned stream.
    /// </summary>
    /// <param name="score">The score to serialize.</param>
    /// <param name="destination">The caller-owned stream that receives the encoded score data.</param>
    /// <param name="cancellationToken">Cancels the write operation.</param>
    /// <returns>A task-like handle that completes when serialization finishes.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="score"/> or <paramref name="destination"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// Implementations may require the stream to be writable and seekable depending on the target format.
    /// </remarks>
    ValueTask WriteAsync(Score score, Stream destination, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes the score to the provided destination file path.
    /// </summary>
    /// <param name="score">The score to serialize.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="cancellationToken">Cancels the write operation.</param>
    /// <returns>A task-like handle that completes when serialization finishes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="score"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, or whitespace.</exception>
    ValueTask WriteAsync(Score score, string filePath, CancellationToken cancellationToken = default);
}
