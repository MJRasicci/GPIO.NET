namespace Motif.Models;

public sealed class Staff : ExtensibleModel
{
    public int StaffIndex { get; set; }

    public IReadOnlyList<StaffMeasure> Measures { get; set; } = Array.Empty<StaffMeasure>();
}
