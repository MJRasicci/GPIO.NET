namespace Motif.Models;

public sealed class TimelineBar : ExtensibleModel
{
    public int Index { get; set; }

    public string TimeSignature { get; set; } = string.Empty;

    public bool DoubleBar { get; set; }

    public bool FreeTime { get; set; }

    public string TripletFeel { get; set; } = string.Empty;

    public bool RepeatStart { get; set; }

    public bool RepeatStartAttributePresent { get; set; }

    public bool RepeatEnd { get; set; }

    public bool RepeatEndAttributePresent { get; set; }

    public int RepeatCount { get; set; }

    public bool RepeatCountAttributePresent { get; set; }

    public string AlternateEndings { get; set; } = string.Empty;

    public string SectionLetter { get; set; } = string.Empty;

    public string SectionText { get; set; } = string.Empty;

    public bool HasExplicitEmptySection { get; set; }

    public string Jump { get; set; } = string.Empty;

    public string Target { get; set; } = string.Empty;

    public int? KeyAccidentalCount { get; set; }

    public string KeyMode { get; set; } = string.Empty;

    public string KeyTransposeAs { get; set; } = string.Empty;

    public IReadOnlyDictionary<string, string> DirectionProperties { get; set; } = new Dictionary<string, string>();

    public IReadOnlyList<FermataMetadata> Fermatas { get; set; } = Array.Empty<FermataMetadata>();
}
