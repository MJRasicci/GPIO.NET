namespace Motif;

using System.IO.Compression;
using Motif.Models;
using System.Text.Json;

internal sealed class MotifArchiveReader : IScoreReader
{
    public async ValueTask<Score> ReadAsync(Stream source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        MemoryStream? bufferedStream = null;
        var archiveStream = source;
        if (!source.CanSeek)
        {
            bufferedStream = new MemoryStream();
            await source.CopyToAsync(bufferedStream, cancellationToken).ConfigureAwait(false);
            bufferedStream.Position = 0;
            archiveStream = bufferedStream;
        }

        try
        {
            using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Read, leaveOpen: true);
            var manifestEntry = archive.GetEntry(MotifArchiveFormat.ManifestEntryName)
                ?? throw new InvalidDataException($"The .motif archive is missing '{MotifArchiveFormat.ManifestEntryName}'.");
            var scoreEntry = archive.GetEntry(MotifArchiveFormat.ScoreEntryName)
                ?? throw new InvalidDataException($"The .motif archive is missing '{MotifArchiveFormat.ScoreEntryName}'.");
            var supplementalEntries = await ReadSupplementalEntriesAsync(archive, cancellationToken).ConfigureAwait(false);

            var manifest = await ReadManifestAsync(manifestEntry, cancellationToken).ConfigureAwait(false);
            MotifArchiveFormat.ValidateManifest(manifest);

            var score = await ReadScoreAsync(scoreEntry, cancellationToken).ConfigureAwait(false);
            ArchiveContributorRegistry.RestoreArchiveEntries(score, manifest, supplementalEntries);
            ScoreNavigation.EnsurePlaybackSequence(score);
            return score;
        }
        finally
        {
            if (bufferedStream is not null)
            {
                await bufferedStream.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    public async ValueTask<Score> ReadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        await using var source = File.OpenRead(filePath);
        return await ReadAsync(source, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<MotifArchiveManifest> ReadManifestAsync(
        ZipArchiveEntry manifestEntry,
        CancellationToken cancellationToken)
    {
        var stream = await manifestEntry.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var _ = stream;
        var manifest = await JsonSerializer.DeserializeAsync(
                stream,
                MotifArchiveJsonContext.Default.MotifArchiveManifest,
                cancellationToken)
            .ConfigureAwait(false);

        return manifest ?? throw new InvalidDataException("Unable to deserialize the .motif archive manifest.");
    }

    private static async ValueTask<Score> ReadScoreAsync(ZipArchiveEntry scoreEntry, CancellationToken cancellationToken)
    {
        var stream = await scoreEntry.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var _ = stream;
        var score = await JsonSerializer.DeserializeAsync(stream, MotifJsonContext.Default.Score, cancellationToken)
            .ConfigureAwait(false);

        return score ?? throw new InvalidDataException("Unable to deserialize 'score.json' from the .motif archive.");
    }

    private static async ValueTask<IReadOnlyList<ArchiveEntry>> ReadSupplementalEntriesAsync(
        ZipArchive archive,
        CancellationToken cancellationToken)
    {
        var entries = new List<ArchiveEntry>();
        foreach (var entry in archive.Entries)
        {
            if (MotifArchivePaths.IsCoreEntry(entry.FullName))
            {
                continue;
            }

            var stream = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var _ = stream;
            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer, cancellationToken).ConfigureAwait(false);
            entries.Add(new ArchiveEntry(entry.FullName, buffer.ToArray()));
        }

        return entries;
    }
}
