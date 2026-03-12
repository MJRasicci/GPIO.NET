namespace Motif;

/// <summary>
/// Describes a score format integration that can create reader and writer instances
/// for one or more file extensions.
/// </summary>
public interface IFormatHandler
{
    /// <summary>
    /// Gets the file extensions supported by this handler.
    /// </summary>
    /// <remarks>
    /// Extensions should include the leading period (for example, <c>.gp</c> or <c>.musicxml</c>).
    /// </remarks>
    IReadOnlyList<string> SupportedExtensions { get; }

    /// <summary>
    /// Gets a human-readable format name for diagnostics and help text.
    /// </summary>
    string FormatName { get; }

    /// <summary>
    /// Creates a reader capable of decoding scores in this handler's format.
    /// </summary>
    /// <returns>A format-specific <see cref="IScoreReader"/> instance.</returns>
    IScoreReader CreateReader();

    /// <summary>
    /// Creates a writer capable of encoding scores in this handler's format.
    /// </summary>
    /// <returns>A format-specific <see cref="IScoreWriter"/> instance.</returns>
    IScoreWriter CreateWriter();
}
