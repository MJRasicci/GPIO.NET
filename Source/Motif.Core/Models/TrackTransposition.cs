namespace Motif.Models;

/// <summary>
/// Track-level transposition expressed as chromatic and octave offsets.
/// </summary>
public sealed class TrackTransposition
{
    /// <summary>
    /// Gets or sets a value indicating whether the transposition was explicitly authored.
    /// </summary>
    public bool IsSpecified { get; set; }

    /// <summary>
    /// Gets or sets the chromatic transposition component.
    /// </summary>
    public int Chromatic { get; set; }

    /// <summary>
    /// Gets or sets the octave transposition component.
    /// </summary>
    public int Octave { get; set; }
}
