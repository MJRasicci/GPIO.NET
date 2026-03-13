namespace Motif.Models;

/// <summary>
/// A single staff within a <see cref="Track"/>.
/// </summary>
public sealed class Staff : ExtensibleModel
{
    /// <summary>
    /// Gets or sets the zero-based staff position within the owning track.
    /// </summary>
    public int StaffIndex { get; set; }

    /// <summary>
    /// Gets or sets the tuning used by the staff.
    /// </summary>
    public StaffTuning Tuning { get; set; } = new();

    /// <summary>
    /// Gets or sets the capo fret applied to the staff, when any.
    /// </summary>
    public int? CapoFret { get; set; }

    /// <summary>
    /// Gets or sets the ordered measures assigned to the staff.
    /// </summary>
    public IReadOnlyList<StaffMeasure> Measures { get; set; } = Array.Empty<StaffMeasure>();
}
