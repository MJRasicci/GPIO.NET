namespace Motif;

using Motif.Models;

/// <summary>
/// Reads an encoded score stream into the Motif domain model.
/// </summary>
public interface IScoreReader
{
    /// <summary>
    /// Reads a score from the provided format-specific stream.
    /// Implementations must not dispose the caller-owned stream.
    /// </summary>
    /// <param name="source">The caller-owned stream containing score data in the reader's source format.</param>
    /// <param name="cancellationToken">Cancels the read operation.</param>
    /// <returns>A mapped <see cref="Score"/> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Implementations may require the stream to be readable and seekable depending on the source format.
    /// </remarks>
    ValueTask<Score> ReadAsync(Stream source, CancellationToken cancellationToken = default);
}
