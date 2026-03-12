namespace Motif;

using Motif.Models;

/// <summary>
/// Contributes supplementary archive entries for a specific extension namespace inside a native `.motif` file.
/// </summary>
public interface IArchiveContributor
{
    /// <summary>
    /// Gets the stable contributor key used in archive entry paths and the manifest extension list.
    /// </summary>
    string ContributorKey { get; }

    /// <summary>
    /// Produces archive entries to persist for the current score state.
    /// </summary>
    /// <param name="score">The score being written.</param>
    /// <returns>The supplementary entries to include in the `.motif` archive.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="score"/> is <see langword="null"/>.</exception>
    IReadOnlyList<ArchiveEntry> GetArchiveEntries(Score score);

    /// <summary>
    /// Restores extension-specific state from the contributor's entries after the score has been deserialized.
    /// </summary>
    /// <param name="score">The score being rehydrated.</param>
    /// <param name="entries">The archive entries assigned to this contributor.</param>
    /// <exception cref="ArgumentNullException"><paramref name="score"/> or <paramref name="entries"/> is <see langword="null"/>.</exception>
    void RestoreFromArchive(Score score, IReadOnlyList<ArchiveEntry> entries);
}
