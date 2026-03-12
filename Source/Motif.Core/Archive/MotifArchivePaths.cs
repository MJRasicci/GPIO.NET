namespace Motif;

internal static class MotifArchivePaths
{
    private const string ExtensionsPrefix = "extensions/";
    private const string ResourcesPrefix = "resources/";

    public static string NormalizeEntryPath(string entryPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryPath);

        var normalized = entryPath.Trim().Replace('\\', '/').TrimStart('/');
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Archive entry path cannot be empty.", nameof(entryPath));
        }

        return normalized;
    }

    public static bool IsCoreEntry(string entryPath)
    {
        var normalized = NormalizeEntryPath(entryPath);
        return string.Equals(normalized, MotifArchiveFormat.ManifestEntryName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, MotifArchiveFormat.ScoreEntryName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsReservedEntry(string entryPath)
        => IsCoreEntry(entryPath);

    public static bool IsValidContributorEntryPath(string contributorKey, string entryPath)
    {
        var normalizedKey = ArchiveContributorRegistry.NormalizeContributorKey(contributorKey);
        var normalizedPath = NormalizeEntryPath(entryPath);

        return string.Equals(normalizedPath, $"{ExtensionsPrefix}{normalizedKey}.json", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.StartsWith($"{ExtensionsPrefix}{normalizedKey}/", StringComparison.OrdinalIgnoreCase)
            || normalizedPath.StartsWith($"{ResourcesPrefix}{normalizedKey}/", StringComparison.OrdinalIgnoreCase);
    }

    public static bool TryGetContributorKey(string entryPath, out string contributorKey)
    {
        var normalizedPath = NormalizeEntryPath(entryPath);

        if (TryGetExtensionContributorKey(normalizedPath, out contributorKey))
        {
            return true;
        }

        if (TryGetResourceContributorKey(normalizedPath, out contributorKey))
        {
            return true;
        }

        contributorKey = string.Empty;
        return false;
    }

    private static bool TryGetExtensionContributorKey(string normalizedPath, out string contributorKey)
    {
        if (!normalizedPath.StartsWith(ExtensionsPrefix, StringComparison.OrdinalIgnoreCase))
        {
            contributorKey = string.Empty;
            return false;
        }

        var suffix = normalizedPath[ExtensionsPrefix.Length..];
        var slashIndex = suffix.IndexOf('/');
        if (slashIndex > 0)
        {
            contributorKey = suffix[..slashIndex];
            return !string.IsNullOrWhiteSpace(contributorKey);
        }

        if (suffix.EndsWith(".json", StringComparison.OrdinalIgnoreCase) && suffix.Length > ".json".Length)
        {
            contributorKey = suffix[..^".json".Length];
            return !string.IsNullOrWhiteSpace(contributorKey);
        }

        contributorKey = string.Empty;
        return false;
    }

    private static bool TryGetResourceContributorKey(string normalizedPath, out string contributorKey)
    {
        if (!normalizedPath.StartsWith(ResourcesPrefix, StringComparison.OrdinalIgnoreCase))
        {
            contributorKey = string.Empty;
            return false;
        }

        var suffix = normalizedPath[ResourcesPrefix.Length..];
        var slashIndex = suffix.IndexOf('/');
        if (slashIndex <= 0)
        {
            contributorKey = string.Empty;
            return false;
        }

        contributorKey = suffix[..slashIndex];
        return !string.IsNullOrWhiteSpace(contributorKey);
    }
}
