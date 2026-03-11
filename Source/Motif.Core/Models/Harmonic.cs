namespace Motif.Models;

public sealed class Harmonic
{
    public int? Type { get; set; }

    public string TypeName { get; set; } = string.Empty;

    public HarmonicTypeKind Kind { get; set; } = HarmonicTypeKind.Unknown;

    public decimal? Fret { get; set; }

    public bool Enabled { get; set; }
}
