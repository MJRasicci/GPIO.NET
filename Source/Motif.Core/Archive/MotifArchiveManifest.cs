namespace Motif;

internal sealed class MotifArchiveManifest
{
    public string FormatVersion { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public List<MotifArchiveSource> Sources { get; set; } = [];

    public List<string> Extensions { get; set; } = [];
}
