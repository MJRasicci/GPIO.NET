namespace Motif.Models;

public sealed class StaffMeasure : ExtensibleModel
{
    public int Index { get; set; }

    public int StaffIndex { get; set; }

    public string Clef { get; set; } = string.Empty;

    public string SimileMark { get; set; } = string.Empty;

    public IReadOnlyList<Voice> Voices { get; set; } = Array.Empty<Voice>();

    public IReadOnlyList<Beat> Beats { get; set; } = Array.Empty<Beat>();
}
