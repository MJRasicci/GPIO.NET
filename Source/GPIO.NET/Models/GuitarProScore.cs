namespace GPIO.NET.Models;

public sealed class GuitarProScore
{
    public string Title { get; init; } = string.Empty;

    public string Artist { get; init; } = string.Empty;

    public string Album { get; init; } = string.Empty;

    public IReadOnlyList<TrackModel> Tracks { get; init; } = Array.Empty<TrackModel>();
}

public sealed class TrackModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public IReadOnlyList<MeasureModel> Measures { get; init; } = Array.Empty<MeasureModel>();
}

public sealed class MeasureModel
{
    public int Index { get; init; }

    public string TimeSignature { get; init; } = string.Empty;

    public IReadOnlyList<BeatModel> Beats { get; init; } = Array.Empty<BeatModel>();
}

public sealed class BeatModel
{
    public decimal Offset { get; init; }

    public decimal Duration { get; init; }

    public IReadOnlyList<int> MidiPitches { get; init; } = Array.Empty<int>();
}
