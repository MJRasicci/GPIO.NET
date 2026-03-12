namespace Motif;

using Motif.Models;

internal static class MotifArchiveStateReattachment
{
    public static void ReattachFrom(Score target, Score source)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(source);

        var targetState = target.GetExtension<MotifArchivePreservedStateExtension>();
        var sourceState = source.GetExtension<MotifArchivePreservedStateExtension>();

        var mergedEntries = MergeEntries(targetState?.PreservedEntries, sourceState?.PreservedEntries);
        var mergedExtensions = MergeExtensions(targetState?.ManifestExtensions, sourceState?.ManifestExtensions);
        var mergedSources = MergeSources(targetState?.ManifestSources, sourceState?.ManifestSources);

        if (mergedEntries.Count == 0 && mergedExtensions.Count == 0 && mergedSources.Count == 0)
        {
            target.RemoveExtension<MotifArchivePreservedStateExtension>();
            return;
        }

        target.SetExtension(new MotifArchivePreservedStateExtension
        {
            PreservedEntries = mergedEntries,
            ManifestExtensions = mergedExtensions,
            ManifestSources = mergedSources
        });
    }

    private static IReadOnlyList<ArchiveEntry> MergeEntries(
        IReadOnlyList<ArchiveEntry>? targetEntries,
        IReadOnlyList<ArchiveEntry>? sourceEntries)
    {
        var entries = new Dictionary<string, ArchiveEntry>(StringComparer.OrdinalIgnoreCase);
        AddEntries(entries, targetEntries);
        AddEntries(entries, sourceEntries);
        return entries.Values
            .OrderBy(entry => entry.EntryPath, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static void AddEntries(
        IDictionary<string, ArchiveEntry> entries,
        IReadOnlyList<ArchiveEntry>? archiveEntries)
    {
        if (archiveEntries is null)
        {
            return;
        }

        foreach (var entry in archiveEntries)
        {
            var normalizedPath = MotifArchivePaths.NormalizeEntryPath(entry.EntryPath);
            entries[normalizedPath] = new ArchiveEntry(normalizedPath, entry.Data);
        }
    }

    private static IReadOnlyList<string> MergeExtensions(
        IReadOnlyList<string>? targetExtensions,
        IReadOnlyList<string>? sourceExtensions)
    {
        var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddExtensions(extensions, targetExtensions);
        AddExtensions(extensions, sourceExtensions);
        return extensions
            .OrderBy(extension => extension, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static void AddExtensions(
        ISet<string> extensions,
        IReadOnlyList<string>? manifestExtensions)
    {
        if (manifestExtensions is null)
        {
            return;
        }

        foreach (var extension in manifestExtensions)
        {
            extensions.Add(extension);
        }
    }

    private static IReadOnlyList<MotifArchiveSource> MergeSources(
        IReadOnlyList<MotifArchiveSource>? targetSources,
        IReadOnlyList<MotifArchiveSource>? sourceSources)
    {
        var mergedSources = new List<MotifArchiveSource>();
        var seenKeys = new HashSet<string>(StringComparer.Ordinal);

        AddSources(mergedSources, seenKeys, targetSources);
        AddSources(mergedSources, seenKeys, sourceSources);

        return mergedSources;
    }

    private static void AddSources(
        ICollection<MotifArchiveSource> mergedSources,
        ISet<string> seenKeys,
        IReadOnlyList<MotifArchiveSource>? sources)
    {
        if (sources is null)
        {
            return;
        }

        foreach (var source in sources)
        {
            var key = $"{source.Format}\0{source.FileName}\0{source.ImportedAt}";
            if (!seenKeys.Add(key))
            {
                continue;
            }

            mergedSources.Add(new MotifArchiveSource
            {
                Format = source.Format,
                FileName = source.FileName,
                ImportedAt = source.ImportedAt
            });
        }
    }
}
