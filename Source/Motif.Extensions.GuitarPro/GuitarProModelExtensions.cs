namespace Motif.Extensions.GuitarPro;

using Motif.Extensions.GuitarPro.Models;
using Motif.Models;

public static class GuitarProModelExtensions
{
    public static GpScoreExtension? GetGuitarPro(this GuitarProScore score)
    {
        ArgumentNullException.ThrowIfNull(score);

        return score.GetExtension<GpScoreExtension>();
    }

    public static GpScoreExtension GetRequiredGuitarPro(this GuitarProScore score)
    {
        ArgumentNullException.ThrowIfNull(score);

        return score.GetRequiredExtension<GpScoreExtension>();
    }

    public static GpScoreExtension GetOrCreateGuitarPro(this GuitarProScore score)
    {
        ArgumentNullException.ThrowIfNull(score);

        var extension = score.GetGuitarPro();
        if (extension is not null)
        {
            return extension;
        }

        extension = new GpScoreExtension
        {
            Metadata = new ScoreMetadata(),
            MasterTrack = new MasterTrackMetadata()
        };
        score.SetExtension(extension);
        return extension;
    }

    public static GpTrackExtension? GetGuitarPro(this TrackModel track)
    {
        ArgumentNullException.ThrowIfNull(track);

        return track.GetExtension<GpTrackExtension>();
    }

    public static GpTrackExtension GetRequiredGuitarPro(this TrackModel track)
    {
        ArgumentNullException.ThrowIfNull(track);

        return track.GetRequiredExtension<GpTrackExtension>();
    }

    public static GpTrackExtension GetOrCreateGuitarPro(this TrackModel track)
    {
        ArgumentNullException.ThrowIfNull(track);

        var extension = track.GetGuitarPro();
        if (extension is not null)
        {
            return extension;
        }

        extension = new GpTrackExtension
        {
            Metadata = new TrackMetadata()
        };
        track.SetExtension(extension);
        return extension;
    }

    public static void ReattachGuitarProExtensionsFrom(this GuitarProScore target, GuitarProScore source)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(source);

        var scoreExtension = source.GetGuitarPro();
        if (scoreExtension is not null)
        {
            target.SetExtension(new GpScoreExtension
            {
                Metadata = scoreExtension.Metadata,
                MasterTrack = scoreExtension.MasterTrack
            });
        }

        var sourceTracksById = new Dictionary<int, TrackModel>();
        foreach (var sourceTrack in source.Tracks)
        {
            sourceTracksById[sourceTrack.Id] = sourceTrack;
        }

        foreach (var targetTrack in target.Tracks)
        {
            if (!sourceTracksById.TryGetValue(targetTrack.Id, out var sourceTrack))
            {
                continue;
            }

            var trackExtension = sourceTrack.GetGuitarPro();
            if (trackExtension is null)
            {
                continue;
            }

            targetTrack.SetExtension(new GpTrackExtension
            {
                Metadata = trackExtension.Metadata
            });
        }
    }
}
