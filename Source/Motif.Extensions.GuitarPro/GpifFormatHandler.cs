namespace Motif.Extensions.GuitarPro;

using Motif;
using Motif.Extensions.GuitarPro.Implementation;

/// <summary>
/// Registers raw GPIF XML support for the unified <see cref="MotifScore"/> API.
/// </summary>
public sealed class GpifFormatHandler : IFormatHandler
{
    /// <inheritdoc />
    public IReadOnlyList<string> SupportedExtensions { get; } = [".gpif"];

    /// <inheritdoc />
    public string FormatName => "Guitar Pro GPIF";

    /// <inheritdoc />
    public IScoreReader CreateReader() => new GpifScoreReader();

    /// <inheritdoc />
    public IScoreWriter CreateWriter() => new GpifScoreWriter();
}
