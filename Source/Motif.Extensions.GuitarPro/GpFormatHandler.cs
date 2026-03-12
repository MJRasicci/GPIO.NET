namespace Motif.Extensions.GuitarPro;

using Motif;

/// <summary>
/// Registers Guitar Pro archive support for the unified <see cref="MotifScore"/> API.
/// </summary>
public sealed class GpFormatHandler : IFormatHandler
{
    /// <inheritdoc />
    public IReadOnlyList<string> SupportedExtensions { get; } = [".gp"];

    /// <inheritdoc />
    public string FormatName => "Guitar Pro";

    /// <inheritdoc />
    public IScoreReader CreateReader() => new GuitarProReader();

    /// <inheritdoc />
    public IScoreWriter CreateWriter() => new GuitarProWriter();
}
