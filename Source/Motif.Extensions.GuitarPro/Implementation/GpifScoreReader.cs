namespace Motif.Extensions.GuitarPro.Implementation;

using Motif;
using Motif.Extensions.GuitarPro.Abstractions;
using Motif.Models;

internal sealed class GpifScoreReader : IScoreReader, IPathScoreReader
{
    private readonly IGpifDeserializer deserializer;
    private readonly IScoreMapper mapper;

    public GpifScoreReader()
        : this(new XmlGpifDeserializer(), new DefaultScoreMapper())
    {
    }

    internal GpifScoreReader(IGpifDeserializer deserializer, IScoreMapper mapper)
    {
        this.deserializer = deserializer;
        this.mapper = mapper;
    }

    public async ValueTask<Score> ReadAsync(Stream source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        var raw = await deserializer.DeserializeAsync(source, cancellationToken).ConfigureAwait(false);
        return await mapper.MapAsync(raw, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Score> ReadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        await using var source = File.OpenRead(filePath);
        return await ReadAsync(source, cancellationToken).ConfigureAwait(false);
    }
}
