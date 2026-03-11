namespace Motif.Models;

public sealed class Beat : ExtensibleModel
{
    public int Id { get; set; }

    public string GraceType { get; set; } = string.Empty;

    public string Dynamic { get; set; } = string.Empty;

    public string Golpe { get; set; } = string.Empty;

    public bool Slashed { get; set; }

    public string Hairpin { get; set; } = string.Empty;

    public string Ottavia { get; set; } = string.Empty;

    public bool? LegatoOrigin { get; set; }

    public bool? LegatoDestination { get; set; }

    public string PickStrokeDirection { get; set; } = string.Empty;

    public bool Slapped { get; set; }

    public bool Popped { get; set; }

    public bool PalmMuted { get; set; }

    public bool Brush { get; set; }

    public bool BrushIsUp { get; set; }

    public bool Arpeggio { get; set; }

    public int? BrushDurationTicks { get; set; }

    public bool Rasgueado { get; set; }

    public string RasgueadoPattern { get; set; } = string.Empty;

    public bool DeadSlapped { get; set; }

    public bool Tremolo { get; set; }

    public string TremoloValue { get; set; } = string.Empty;

    public string FreeText { get; set; } = string.Empty;

    public WhammyBar? WhammyBar { get; set; }

    public decimal Offset { get; set; }

    public decimal Duration { get; set; }

    public IReadOnlyList<Note> Notes { get; set; } = Array.Empty<Note>();

    public IReadOnlyList<int> MidiPitches { get; set; } = Array.Empty<int>();
}
