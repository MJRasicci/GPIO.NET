namespace Motif.Models;

/// <summary>
/// More specific semantic instrument identities layered on top of <see cref="InstrumentFamilyKind"/>.
/// </summary>
public enum InstrumentKind
{
    /// <summary>
    /// No specific instrument is specified.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Generic pitched fallback.
    /// </summary>
    GenericPitched = 1,

    /// <summary>
    /// Steel-string acoustic guitar.
    /// </summary>
    SteelStringGuitar = 2,

    /// <summary>
    /// Nylon-string classical guitar.
    /// </summary>
    NylonStringGuitar = 3,

    /// <summary>
    /// Electric guitar.
    /// </summary>
    ElectricGuitar = 4,

    /// <summary>
    /// Acoustic bass.
    /// </summary>
    AcousticBass = 5,

    /// <summary>
    /// Electric bass.
    /// </summary>
    ElectricBass = 6,

    /// <summary>
    /// Fretless bass.
    /// </summary>
    FretlessBass = 7,

    /// <summary>
    /// Synth bass.
    /// </summary>
    SynthBass = 8,

    /// <summary>
    /// Acoustic piano.
    /// </summary>
    AcousticPiano = 9,

    /// <summary>
    /// Electric piano.
    /// </summary>
    ElectricPiano = 10,

    /// <summary>
    /// Generic keyboard.
    /// </summary>
    Keyboard = 11,

    /// <summary>
    /// Drum kit.
    /// </summary>
    DrumKit = 12,

    /// <summary>
    /// Ukulele.
    /// </summary>
    Ukulele = 13,

    /// <summary>
    /// Mandolin.
    /// </summary>
    Mandolin = 14,

    /// <summary>
    /// Generic banjo.
    /// </summary>
    Banjo = 15,

    /// <summary>
    /// Five-string banjo.
    /// </summary>
    FiveStringBanjo = 16,

    /// <summary>
    /// Violin.
    /// </summary>
    Violin = 17,

    /// <summary>
    /// Viola.
    /// </summary>
    Viola = 18,

    /// <summary>
    /// Cello.
    /// </summary>
    Cello = 19,

    /// <summary>
    /// Contrabass.
    /// </summary>
    Contrabass = 20
}
