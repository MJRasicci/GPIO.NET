namespace GPIO.NET.Models.Raw;

public sealed class GpifMasterTrack
{
    public int[] TrackIds { get; init; } = Array.Empty<int>();

    public IReadOnlyList<GpifAutomation> Automations { get; init; } = Array.Empty<GpifAutomation>();

    public string RseXml { get; init; } = string.Empty;
}
