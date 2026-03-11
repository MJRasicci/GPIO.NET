namespace Motif.Models;

public sealed class Bend
{
    public bool Enabled { get; set; }

    public BendTypeKind Type { get; set; } = BendTypeKind.Unknown;

    public decimal? OriginOffset { get; set; }

    public decimal? OriginValue { get; set; }

    public decimal? MiddleOffset1 { get; set; }

    public decimal? MiddleOffset2 { get; set; }

    public decimal? MiddleValue { get; set; }

    public decimal? DestinationOffset { get; set; }

    public decimal? DestinationValue { get; set; }
}
