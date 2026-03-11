namespace Motif.Models;

public sealed class FermataMetadata
{
    public string Type { get; set; } = string.Empty;

    public string Offset { get; set; } = string.Empty;

    public decimal? Length { get; set; }
}
