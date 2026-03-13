namespace Motif.Models;

/// <summary>
/// Staff-level tuning definition.
/// </summary>
public sealed class StaffTuning
{
    /// <summary>
    /// Gets or sets the tuning pitches ordered from low to high.
    /// </summary>
    public IReadOnlyList<int> Pitches { get; set; } = Array.Empty<int>();

    /// <summary>
    /// Gets or sets an optional display label for the tuning preset.
    /// </summary>
    public string Label { get; set; } = string.Empty;
}
