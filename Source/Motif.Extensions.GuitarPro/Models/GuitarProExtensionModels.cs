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
