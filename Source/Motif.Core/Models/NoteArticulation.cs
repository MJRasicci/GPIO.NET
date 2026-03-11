namespace Motif.Models;

public sealed class NoteArticulation
{
    public string LeftFingering { get; set; } = string.Empty;

    public string RightFingering { get; set; } = string.Empty;

    public string Ornament { get; set; } = string.Empty;

    public bool LetRing { get; set; }

    public string Vibrato { get; set; } = string.Empty;

    public bool TieOrigin { get; set; }

    public bool TieDestination { get; set; }

    public int? Trill { get; set; }

    public TrillSpeedKind TrillSpeed { get; set; } = TrillSpeedKind.None;

    public int? Accent { get; set; }

    public bool AntiAccent { get; set; }

    public bool PalmMuted { get; set; }

    public bool Muted { get; set; }

    public bool Tapped { get; set; }

    public bool LeftHandTapped { get; set; }

    public bool HopoOrigin { get; set; }

    public bool HopoDestination { get; set; }

    public HopoTypeKind HopoType { get; set; } = HopoTypeKind.None;

    public int? HopoOriginNoteId { get; set; }

    public int? HopoDestinationNoteId { get; set; }

    public IReadOnlyList<SlideType> Slides { get; set; } = Array.Empty<SlideType>();

    public Harmonic? Harmonic { get; set; }

    public Bend? Bend { get; set; }
}
