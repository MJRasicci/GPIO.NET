namespace Motif.Extensions.GuitarPro.Models;

using Motif.Models;

public sealed class GpScoreExtension : IModelExtension
{
    public required ScoreMetadata Metadata { get; set; }

    public required MasterTrackMetadata MasterTrack { get; set; }
}

public sealed class GpTrackExtension : IModelExtension
{
    public required TrackMetadata Metadata { get; set; }
}

public sealed class GpMeasureExtension : IModelExtension
{
    public required GpMeasureMetadata Metadata { get; set; }
}

public sealed class GpMeasureStaffExtension : IModelExtension
{
    public required GpMeasureStaffMetadata Metadata { get; set; }
}

public sealed class GpVoiceExtension : IModelExtension
{
    public required GpVoiceMetadata Metadata { get; set; }
}

public sealed class GpBeatExtension : IModelExtension
{
    public required GpBeatMetadata Metadata { get; set; }
}

public sealed class GpNoteExtension : IModelExtension
{
    public required GpNoteMetadata Metadata { get; set; }
}

public sealed class GpMeasureMetadata
{
    public string MasterBarXml { get; set; } = string.Empty;

    public string BarXml { get; set; } = string.Empty;

    public int SourceBarId { get; set; } = -1;

    public string DirectionsXml { get; set; } = string.Empty;

    public string MasterBarXPropertiesXml { get; set; } = string.Empty;

    public string BarXPropertiesXml { get; set; } = string.Empty;
}

public sealed class GpMeasureStaffMetadata
{
    public string BarXml { get; set; } = string.Empty;

    public int SourceBarId { get; set; } = -1;

    public string BarXPropertiesXml { get; set; } = string.Empty;
}

public sealed class GpVoiceMetadata
{
    public string Xml { get; set; } = string.Empty;

    public int SourceVoiceId { get; set; }

    public IReadOnlyDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

    public IReadOnlyList<string> DirectionTags { get; set; } = Array.Empty<string>();
}

public sealed class GpBeatMetadata
{
    public string Xml { get; set; } = string.Empty;

    public int SourceRhythmId { get; set; } = -1;

    public GpRhythmShapeMetadata? SourceRhythm { get; set; }

    public string XPropertiesXml { get; set; } = string.Empty;
}

public sealed class GpRhythmShapeMetadata
{
    public string Xml { get; set; } = string.Empty;

    public string NoteValue { get; set; } = string.Empty;

    public int AugmentationDots { get; set; }

    public bool AugmentationDotUsesCountAttribute { get; set; }

    public int[] AugmentationDotCounts { get; set; } = Array.Empty<int>();

    public TupletRatioModel? PrimaryTuplet { get; set; }

    public TupletRatioModel? SecondaryTuplet { get; set; }
}

public sealed class GpNoteMetadata
{
    public string Xml { get; set; } = string.Empty;

    public int? SourceMidiPitch { get; set; }

    public int? SourceTransposedMidiPitch { get; set; }

    public int? SourceFret { get; set; }

    public int? SourceStringNumber { get; set; }

    public string XPropertiesXml { get; set; } = string.Empty;
}
