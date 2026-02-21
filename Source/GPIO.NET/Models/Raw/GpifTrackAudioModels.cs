namespace GPIO.NET.Models.Raw;

public sealed class GpifInstrumentSet
{
    public string Name { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;

    public int? LineCount { get; init; }
}

public sealed class GpifSound
{
    public string Name { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public int? MidiLsb { get; init; }

    public int? MidiMsb { get; init; }

    public int? MidiProgram { get; init; }
}

public sealed class GpifRse
{
    public string ChannelStripVersion { get; init; } = string.Empty;

    public string ChannelStripParameters { get; init; } = string.Empty;
}
