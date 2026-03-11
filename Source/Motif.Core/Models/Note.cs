namespace Motif.Models;

public sealed class Note : ExtensibleModel
{
    public int Id { get; set; }

    public int? Velocity { get; set; }

    public int? MidiPitch { get; set; }

    public PitchValue? ConcertPitch { get; set; }

    public PitchValue? TransposedPitch { get; set; }

    public bool ShowStringNumber { get; set; }

    public int? StringNumber { get; set; }

    public decimal Duration { get; set; }

    public bool TieExtendedFromPrevious { get; set; }

    public NoteArticulation Articulation { get; set; } = new();
}
