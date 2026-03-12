namespace Motif;

using Motif.Models;

/// <summary>
/// Central entry point for opening and saving Motif scores across registered formats.
/// </summary>
public static class MotifScore
{
    /// <summary>
    /// Opens a score from the provided file path by inferring the format from its extension.
    /// </summary>
    /// <param name="filePath">The source file path.</param>
    /// <param name="cancellationToken">Cancels the read operation.</param>
    /// <returns>A mapped <see cref="Score"/> instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, whitespace, or has no extension.</exception>
    /// <exception cref="InvalidOperationException">No handler is registered for the inferred extension.</exception>
    /// <exception cref="NotSupportedException">The file uses a core format that is not implemented yet.</exception>
    public static async ValueTask<Score> OpenAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var formatHint = FormatHandlerRegistry.GetFormatHintFromPath(filePath);
        var reader = ResolveHandlerOrThrow(formatHint).CreateReader();
        return await reader.ReadAsync(filePath, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Opens a score from the provided stream using an explicit format hint.
    /// </summary>
    /// <param name="source">The caller-owned source stream.</param>
    /// <param name="formatHint">A file extension or format token such as <c>.gp</c> or <c>json</c>.</param>
    /// <param name="cancellationToken">Cancels the read operation.</param>
    /// <returns>A mapped <see cref="Score"/> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="formatHint"/> is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException">No handler is registered for <paramref name="formatHint"/>.</exception>
    /// <exception cref="NotSupportedException">The requested core format is not implemented yet.</exception>
    public static ValueTask<Score> OpenAsync(Stream source, string formatHint, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        var reader = ResolveHandlerOrThrow(formatHint).CreateReader();
        return reader.ReadAsync(source, cancellationToken);
    }

    /// <summary>
    /// Saves a score to the provided file path by inferring the format from its extension.
    /// </summary>
    /// <param name="score">The score to serialize.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="cancellationToken">Cancels the write operation.</param>
    /// <returns>A task-like handle that completes when serialization finishes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="score"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty, whitespace, or has no extension.</exception>
    /// <exception cref="InvalidOperationException">No handler is registered for the inferred extension.</exception>
    /// <exception cref="NotSupportedException">The file uses a core format that is not implemented yet.</exception>
    public static async ValueTask SaveAsync(Score score, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(score);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var formatHint = FormatHandlerRegistry.GetFormatHintFromPath(filePath);
        var writer = ResolveHandlerOrThrow(formatHint).CreateWriter();
        await writer.WriteAsync(score, filePath, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Saves a score to the provided stream using an explicit format hint.
    /// </summary>
    /// <param name="score">The score to serialize.</param>
    /// <param name="destination">The caller-owned destination stream.</param>
    /// <param name="formatHint">A file extension or format token such as <c>.gp</c> or <c>json</c>.</param>
    /// <param name="cancellationToken">Cancels the write operation.</param>
    /// <returns>A task-like handle that completes when serialization finishes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="score"/> or <paramref name="destination"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="formatHint"/> is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException">No handler is registered for <paramref name="formatHint"/>.</exception>
    /// <exception cref="NotSupportedException">The requested core format is not implemented yet.</exception>
    public static ValueTask SaveAsync(Score score, Stream destination, string formatHint, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(score);
        ArgumentNullException.ThrowIfNull(destination);

        var writer = ResolveHandlerOrThrow(formatHint).CreateWriter();
        return writer.WriteAsync(score, destination, cancellationToken);
    }

    /// <summary>
    /// Returns all currently available score formats, including Motif's built-in JSON support.
    /// </summary>
    public static IReadOnlyList<IFormatHandler> GetRegisteredFormats()
        => FormatHandlerRegistry.GetRegisteredHandlers();

    /// <summary>
    /// Returns whether Motif can open the provided file path with the currently registered handlers.
    /// </summary>
    /// <param name="filePath">The file path to inspect.</param>
    public static bool CanOpen(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var extension = Path.GetExtension(filePath);
        return !string.IsNullOrWhiteSpace(extension)
               && !string.Equals(FormatHandlerRegistry.NormalizeFormatHint(extension), ".motif", StringComparison.OrdinalIgnoreCase)
               && FormatHandlerRegistry.TryResolve(extension, out _);
    }

    /// <summary>
    /// Registers a format handler explicitly and returns a token that unregisters it on dispose.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    /// <returns>A disposable registration token.</returns>
    public static IDisposable RegisterHandler(IFormatHandler handler)
        => FormatHandlerRegistry.Register(handler);

    private static IFormatHandler ResolveHandlerOrThrow(string formatHint)
    {
        var normalizedHint = FormatHandlerRegistry.NormalizeFormatHint(formatHint);
        if (string.Equals(normalizedHint, ".motif", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("The native '.motif' archive format is planned but not implemented yet.");
        }

        return FormatHandlerRegistry.TryResolve(normalizedHint, out var handler)
            ? handler!
            : throw new InvalidOperationException(
                $"No format handler registered for '{normalizedHint}'. Reference the relevant Motif extension package or call {nameof(MotifScore)}.{nameof(RegisterHandler)}(...).");
    }
}
