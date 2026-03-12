namespace Motif;

/// <summary>
/// Represents a single supplementary file entry inside a native `.motif` archive.
/// </summary>
public sealed class ArchiveEntry
{
    /// <summary>
    /// Initializes a new archive entry.
    /// </summary>
    /// <param name="entryPath">The relative archive path, for example <c>extensions/guitarpro.json</c>.</param>
    /// <param name="data">The entry payload bytes.</param>
    /// <exception cref="ArgumentException"><paramref name="entryPath"/> is null, empty, or whitespace.</exception>
    public ArchiveEntry(string entryPath, ReadOnlyMemory<byte> data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryPath);

        EntryPath = entryPath;
        Data = data.ToArray();
    }

    /// <summary>
    /// Gets the relative path for this entry inside the archive.
    /// </summary>
    public string EntryPath { get; }

    /// <summary>
    /// Gets the entry payload bytes.
    /// </summary>
    public ReadOnlyMemory<byte> Data { get; }
}
