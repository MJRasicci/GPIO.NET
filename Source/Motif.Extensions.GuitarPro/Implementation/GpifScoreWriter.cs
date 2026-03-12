namespace Motif.Extensions.GuitarPro.Implementation;

using Motif;
using Motif.Extensions.GuitarPro.Abstractions;
using Motif.Models;

internal sealed class GpifScoreWriter : IScoreWriter
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
    {
        ArgumentNullException.ThrowIfNull(score);
        ArgumentNullException.ThrowIfNull(destination);

        var result = await unmapper.UnmapAsync(score, cancellationToken).ConfigureAwait(false);
        await serializer.SerializeAsync(result.RawDocument, destination, cancellationToken).ConfigureAwait(false);
        await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask WriteAsync(Score score, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(score);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var destination = File.Create(filePath);
        await WriteAsync(score, destination, cancellationToken).ConfigureAwait(false);
    }
}
