namespace Motif.Models;

public sealed class PitchValue
{
    public string Step { get; set; } = string.Empty;

    public string Accidental { get; set; } = string.Empty;

    public int? Octave { get; set; }
}
