namespace Motif.Models;

/// <summary>
/// Broad musical role used when instrument identity alone is not enough to guide export defaults.
/// </summary>
public enum TrackRoleKind
{
    /// <summary>
    /// No role is specified.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The track is pitched.
    /// </summary>
    Pitched = 1,

    /// <summary>
    /// The track is percussion-based.
    /// </summary>
    Percussion = 2
}
