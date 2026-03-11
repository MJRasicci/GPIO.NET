namespace Motif.Models;

public sealed class Voice : ExtensibleModel
{
    public int VoiceIndex { get; set; }

    public IReadOnlyList<Beat> Beats { get; set; } = Array.Empty<Beat>();
}
