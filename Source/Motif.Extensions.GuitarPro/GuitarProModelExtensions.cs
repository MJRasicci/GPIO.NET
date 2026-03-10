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

    public static GpMeasureExtension? GetGuitarPro(this MeasureModel measure)
    {
        ArgumentNullException.ThrowIfNull(measure);

        return measure.GetExtension<GpMeasureExtension>();
    }

    public static GpMeasureExtension GetRequiredGuitarPro(this MeasureModel measure)
    {
        ArgumentNullException.ThrowIfNull(measure);

        return measure.GetRequiredExtension<GpMeasureExtension>();
    }

    public static GpMeasureExtension GetOrCreateGuitarPro(this MeasureModel measure)
    {
        ArgumentNullException.ThrowIfNull(measure);

        var extension = measure.GetGuitarPro();
        if (extension is not null)
        {
            return extension;
        }

        extension = new GpMeasureExtension
        {
            Metadata = new GpMeasureMetadata()
        };
        measure.SetExtension(extension);
        return extension;
    }

    public static GpMeasureStaffExtension? GetGuitarPro(this MeasureStaffModel staff)
    {
        ArgumentNullException.ThrowIfNull(staff);

        return staff.GetExtension<GpMeasureStaffExtension>();
    }

    public static GpMeasureStaffExtension GetRequiredGuitarPro(this MeasureStaffModel staff)
    {
        ArgumentNullException.ThrowIfNull(staff);

        return staff.GetRequiredExtension<GpMeasureStaffExtension>();
    }

    public static GpMeasureStaffExtension GetOrCreateGuitarPro(this MeasureStaffModel staff)
    {
        ArgumentNullException.ThrowIfNull(staff);

        var extension = staff.GetGuitarPro();
        if (extension is not null)
        {
            return extension;
        }

        extension = new GpMeasureStaffExtension
        {
            Metadata = new GpMeasureStaffMetadata()
        };
        staff.SetExtension(extension);
        return extension;
    }

    public static GpVoiceExtension? GetGuitarPro(this MeasureVoiceModel voice)
    {
        ArgumentNullException.ThrowIfNull(voice);

        return voice.GetExtension<GpVoiceExtension>();
    }

    public static GpVoiceExtension GetRequiredGuitarPro(this MeasureVoiceModel voice)
    {
        ArgumentNullException.ThrowIfNull(voice);

        return voice.GetRequiredExtension<GpVoiceExtension>();
    }

    public static GpVoiceExtension GetOrCreateGuitarPro(this MeasureVoiceModel voice)
    {
        ArgumentNullException.ThrowIfNull(voice);

        var extension = voice.GetGuitarPro();
        if (extension is not null)
        {
            return extension;
        }

        extension = new GpVoiceExtension
        {
            Metadata = new GpVoiceMetadata()
        };
        voice.SetExtension(extension);
        return extension;
    }

    public static GpBeatExtension? GetGuitarPro(this BeatModel beat)
    {
        ArgumentNullException.ThrowIfNull(beat);

        return beat.GetExtension<GpBeatExtension>();
    }

    public static GpBeatExtension GetRequiredGuitarPro(this BeatModel beat)
    {
        ArgumentNullException.ThrowIfNull(beat);

        return beat.GetRequiredExtension<GpBeatExtension>();
    }

    public static GpBeatExtension GetOrCreateGuitarPro(this BeatModel beat)
    {
        ArgumentNullException.ThrowIfNull(beat);

        var extension = beat.GetGuitarPro();
        if (extension is not null)
        {
            return extension;
        }

        extension = new GpBeatExtension
        {
            Metadata = new GpBeatMetadata()
        };
        beat.SetExtension(extension);
        return extension;
    }

    public static GpNoteExtension? GetGuitarPro(this NoteModel note)
    {
        ArgumentNullException.ThrowIfNull(note);

        return note.GetExtension<GpNoteExtension>();
    }

    public static GpNoteExtension GetRequiredGuitarPro(this NoteModel note)
    {
        ArgumentNullException.ThrowIfNull(note);

        return note.GetRequiredExtension<GpNoteExtension>();
    }

    public static GpNoteExtension GetOrCreateGuitarPro(this NoteModel note)
    {
        ArgumentNullException.ThrowIfNull(note);

        var extension = note.GetGuitarPro();
        if (extension is not null)
        {
            return extension;
        }

        extension = new GpNoteExtension
        {
            Metadata = new GpNoteMetadata()
        };
        note.SetExtension(extension);
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

            ReattachMeasureHierarchyExtensions(targetTrack, sourceTrack);
        }
    }

    private static void ReattachMeasureHierarchyExtensions(TrackModel targetTrack, TrackModel sourceTrack)
    {
        for (var measureIndex = 0; measureIndex < targetTrack.Measures.Count && measureIndex < sourceTrack.Measures.Count; measureIndex++)
        {
            var targetMeasure = targetTrack.Measures[measureIndex];
            var sourceMeasure = sourceTrack.Measures[measureIndex];
            var measureExtension = sourceMeasure.GetGuitarPro();
            if (measureExtension is not null)
            {
                targetMeasure.SetExtension(new GpMeasureExtension
                {
                    Metadata = measureExtension.Metadata
                });
            }

            var sourceStaffByIndex = sourceMeasure.AdditionalStaffBars
                .ToDictionary(staff => staff.StaffIndex);
            foreach (var targetStaff in targetMeasure.AdditionalStaffBars)
            {
                if (!sourceStaffByIndex.TryGetValue(targetStaff.StaffIndex, out var sourceStaff))
                {
                    continue;
                }

                var staffExtension = sourceStaff.GetGuitarPro();
                if (staffExtension is null)
                {
                    continue;
                }

                targetStaff.SetExtension(new GpMeasureStaffExtension
                {
                    Metadata = staffExtension.Metadata
                });
            }

            var sourceVoiceByIndex = sourceMeasure.Voices
                .ToDictionary(voice => voice.VoiceIndex);
            foreach (var targetVoice in targetMeasure.Voices)
            {
                if (!sourceVoiceByIndex.TryGetValue(targetVoice.VoiceIndex, out var sourceVoice))
                {
                    continue;
                }

                var voiceExtension = sourceVoice.GetGuitarPro();
                if (voiceExtension is null)
                {
                    continue;
                }

                targetVoice.SetExtension(new GpVoiceExtension
                {
                    Metadata = voiceExtension.Metadata
                });

                ReattachBeatExtensions(targetVoice.Beats, sourceVoice.Beats);
            }

            ReattachBeatExtensions(targetMeasure.Beats, sourceMeasure.Beats);

            foreach (var targetStaff in targetMeasure.AdditionalStaffBars)
            {
                if (!sourceStaffByIndex.TryGetValue(targetStaff.StaffIndex, out var sourceStaff))
                {
                    continue;
                }

                ReattachBeatExtensions(targetStaff.Beats, sourceStaff.Beats);

                var sourceStaffVoicesByIndex = sourceStaff.Voices
                    .ToDictionary(voice => voice.VoiceIndex);
                foreach (var targetStaffVoice in targetStaff.Voices)
                {
                    if (!sourceStaffVoicesByIndex.TryGetValue(targetStaffVoice.VoiceIndex, out var sourceStaffVoice))
                    {
                        continue;
                    }

                    ReattachBeatExtensions(targetStaffVoice.Beats, sourceStaffVoice.Beats);
                }
            }
        }
    }

    private static void ReattachBeatExtensions(IReadOnlyList<BeatModel> targetBeats, IReadOnlyList<BeatModel> sourceBeats)
    {
        var sourceBeatsById = BuildItemsByIdQueue(sourceBeats, static beat => beat.Id);
        foreach (var targetBeat in targetBeats)
        {
            if (!TryDequeueMatchingItem(sourceBeatsById, targetBeat.Id, out var sourceBeat))
            {
                continue;
            }

            var beatExtension = sourceBeat.GetGuitarPro();
            if (beatExtension is not null)
            {
                targetBeat.SetExtension(new GpBeatExtension
                {
                    Metadata = beatExtension.Metadata
                });
            }

            var sourceNotesById = BuildItemsByIdQueue(sourceBeat.Notes, static note => note.Id);
            foreach (var targetNote in targetBeat.Notes)
            {
                if (!TryDequeueMatchingItem(sourceNotesById, targetNote.Id, out var sourceNote))
                {
                    continue;
                }

                var noteExtension = sourceNote.GetGuitarPro();
                if (noteExtension is null)
                {
                    continue;
                }

                targetNote.SetExtension(new GpNoteExtension
                {
                    Metadata = noteExtension.Metadata
                });
            }
        }
    }

    private static Dictionary<int, Queue<TItem>> BuildItemsByIdQueue<TItem>(
        IReadOnlyList<TItem> items,
        Func<TItem, int> idSelector)
    {
        var itemsById = new Dictionary<int, Queue<TItem>>();
        foreach (var item in items)
        {
            var id = idSelector(item);
            if (!itemsById.TryGetValue(id, out var queue))
            {
                queue = new Queue<TItem>();
                itemsById[id] = queue;
            }

            queue.Enqueue(item);
        }

        return itemsById;
    }

    private static bool TryDequeueMatchingItem<TItem>(
        Dictionary<int, Queue<TItem>> itemsById,
        int id,
        out TItem item)
    {
        if (itemsById.TryGetValue(id, out var queue) && queue.Count > 0)
        {
            item = queue.Dequeue();
            return true;
        }

        item = default!;
        return false;
    }
}
