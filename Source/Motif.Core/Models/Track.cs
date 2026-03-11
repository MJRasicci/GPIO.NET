namespace Motif.Models;

public sealed class Track : ExtensibleModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public IReadOnlyList<Staff> Staves { get; set; } = Array.Empty<Staff>();
}
