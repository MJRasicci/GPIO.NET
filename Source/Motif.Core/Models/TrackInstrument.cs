namespace Motif.Models;

/// <summary>
/// Semantic instrument identity for a <see cref="Track"/>.
/// </summary>
public sealed class TrackInstrument
{
    /// <summary>
    /// Gets or sets the broad instrument family.
    /// </summary>
    public InstrumentFamilyKind Family { get; set; }

    /// <summary>
    /// Gets or sets the more specific instrument identity.
    /// </summary>
    public InstrumentKind Kind { get; set; }

    /// <summary>
    /// Gets or sets the broad pitched/percussion role of the track.
    /// </summary>
    public TrackRoleKind Role { get; set; }
}
