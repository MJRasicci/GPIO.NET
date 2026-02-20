namespace GPIO.NET.Models.Patching;

public sealed class GpPatchDocument
{
    public IReadOnlyList<AppendNotesPatch> AppendNotes { get; init; } = Array.Empty<AppendNotesPatch>();
}

public sealed class AppendNotesPatch
{
    public int TrackId { get; init; }

    public int MasterBarIndex { get; init; }

    public int VoiceIndex { get; init; } = 0;

    public string RhythmNoteValue { get; init; } = "Quarter";

    public int AugmentationDots { get; init; }

    public int? TupletNumerator { get; init; }

    public int? TupletDenominator { get; init; }

    public IReadOnlyList<int> MidiPitches { get; init; } = Array.Empty<int>();
}
