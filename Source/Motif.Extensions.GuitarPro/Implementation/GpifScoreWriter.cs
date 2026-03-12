namespace Motif.Extensions.GuitarPro.Implementation;

using Motif;
using Motif.Extensions.GuitarPro.Abstractions;
using Motif.Extensions.GuitarPro.Models.Write;
using Motif.Models;

internal sealed class GpifScoreWriter : IGpifWriter
{
    private readonly IScoreUnmapper unmapper;
    private readonly IGpifSerializer serializer;

    public GpifScoreWriter()
        : this(new DefaultScoreUnmapper(), new XmlGpifSerializer())
    {
    }

    internal GpifScoreWriter(IScoreUnmapper unmapper, IGpifSerializer serializer)
    {
        this.unmapper = unmapper;
        this.serializer = serializer;
    }

    public async ValueTask WriteAsync(Score score, Stream destination, CancellationToken cancellationToken = default)
        => _ = await WriteWithDiagnosticsAsync(score, destination, cancellationToken).ConfigureAwait(false);

    public async ValueTask<WriteDiagnostics> WriteWithDiagnosticsAsync(Score score, Stream destination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(score);
        ArgumentNullException.ThrowIfNull(destination);

        var result = await unmapper.UnmapAsync(score, cancellationToken).ConfigureAwait(false);
        await serializer.SerializeAsync(result.RawDocument, destination, cancellationToken).ConfigureAwait(false);
        await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
        return result.Diagnostics;
    }

    public async ValueTask WriteAsync(Score score, string filePath, CancellationToken cancellationToken = default)
        => _ = await WriteWithDiagnosticsAsync(score, filePath, cancellationToken).ConfigureAwait(false);

    public async ValueTask<WriteDiagnostics> WriteWithDiagnosticsAsync(Score score, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(score);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var destination = File.Create(filePath);
        return await WriteWithDiagnosticsAsync(score, destination, cancellationToken).ConfigureAwait(false);
    }
}
