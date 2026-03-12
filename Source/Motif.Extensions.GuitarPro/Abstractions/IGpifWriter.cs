namespace Motif.Extensions.GuitarPro.Abstractions;

using Motif;
using Motif.Extensions.GuitarPro.Models.Write;
using Motif.Models;

/// <summary>
/// High-level entry point for writing raw GPIF XML from a mapped Motif score.
/// </summary>
public interface IGpifWriter : IScoreWriter
{
    /// <summary>
    /// Writes raw GPIF XML to the provided file path.
    /// </summary>
    /// <param name="score">The score to serialize.</param>
    /// <param name="filePath">The destination GPIF file path.</param>
    /// <param name="cancellationToken">Cancels the write operation.</param>
    new ValueTask WriteAsync(Score score, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes raw GPIF XML to the provided file path and returns write diagnostics.
    /// </summary>
    /// <param name="score">The score to serialize.</param>
    /// <param name="filePath">The destination GPIF file path.</param>
    /// <param name="cancellationToken">Cancels the write operation.</param>
    /// <returns>The diagnostics captured while regenerating GPIF from the mapped score.</returns>
    ValueTask<WriteDiagnostics> WriteWithDiagnosticsAsync(Score score, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes raw GPIF XML to the provided destination stream and returns write diagnostics.
    /// </summary>
    /// <param name="score">The score to serialize.</param>
    /// <param name="destination">The caller-owned stream that receives GPIF XML.</param>
    /// <param name="cancellationToken">Cancels the write operation.</param>
    /// <returns>The diagnostics captured while regenerating GPIF from the mapped score.</returns>
    ValueTask<WriteDiagnostics> WriteWithDiagnosticsAsync(Score score, Stream destination, CancellationToken cancellationToken = default);
}
