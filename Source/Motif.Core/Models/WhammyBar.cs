namespace Motif.Models;

public sealed class WhammyBar
{
    public bool Enabled { get; set; }

    public bool Extended { get; set; }

    public decimal? OriginValue { get; set; }

    public decimal? MiddleValue { get; set; }

    public decimal? DestinationValue { get; set; }

    public decimal? OriginOffset { get; set; }

    public decimal? MiddleOffset1 { get; set; }

    public decimal? MiddleOffset2 { get; set; }

    public decimal? DestinationOffset { get; set; }
}
