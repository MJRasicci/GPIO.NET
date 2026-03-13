namespace Motif.Models;

/// <summary>
/// Broad semantic instrument families used by authoring-aware export logic.
/// </summary>
public enum InstrumentFamilyKind
{
    /// <summary>
    /// No family is specified.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Generic pitched fallback with no more specific family assigned.
    /// </summary>
    GenericPitched = 1,

    /// <summary>
    /// Guitar-family instruments.
    /// </summary>
    Guitar = 2,

    /// <summary>
    /// Bass-family instruments.
    /// </summary>
    Bass = 3,

    /// <summary>
    /// Piano-family instruments.
    /// </summary>
    Piano = 4,

    /// <summary>
    /// Keyboard-family instruments other than piano.
    /// </summary>
    Keyboard = 5,

    /// <summary>
    /// Percussion or drum-kit instruments.
    /// </summary>
    Percussion = 6,

    /// <summary>
    /// Ukulele-family instruments.
    /// </summary>
    Ukulele = 7,

    /// <summary>
    /// Mandolin-family instruments.
    /// </summary>
    Mandolin = 8,

    /// <summary>
    /// Banjo-family instruments.
    /// </summary>
    Banjo = 9,

    /// <summary>
    /// Bowed string instruments.
    /// </summary>
    BowedStrings = 10
}
