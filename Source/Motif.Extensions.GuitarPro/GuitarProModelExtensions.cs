namespace Motif.Extensions.GuitarPro;

using Motif.Extensions.GuitarPro.Models;
using Motif.Models;

public static class GuitarProModelExtensions
{
    public static GpScoreExtension? GetGuitarPro(this Score score)
    {
        ArgumentNullException.ThrowIfNull(score);

        return score.GetExtension<GpScoreExtension>();
    }

    public static GpScoreExtension GetRequiredGuitarPro(this Score score)
    {
        ArgumentNullException.ThrowIfNull(score);

        return score.GetRequiredExtension<GpScoreExtension>();
    }

    public static GpScoreExtension GetOrCreateGuitarPro(this Score score)
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

    public static void InvalidateGuitarProExtensions(this Score score)
    {
        ArgumentNullException.ThrowIfNull(score);

        score.RemoveExtension<GpScoreExtension>();

        foreach (var track in score.Tracks)
        {
            track.RemoveExtension<GpTrackExtension>();
            InvalidateMeasureHierarchyExtensions(track);
        }
    }

    public static GpExtensionReattachmentResult ReattachGuitarProExtensionsFrom(this Score target, Score source)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(source);

        if (!ReferenceEquals(target, source))
        {
            target.InvalidateGuitarProExtensions();
        }

        var result = new GpExtensionReattachmentResult();
        var scoreExtension = source.GetGuitarPro();
        if (scoreExtension is not null)
        {
            target.SetExtension(new GpScoreExtension
            {
                Metadata = scoreExtension.Metadata,
                MasterTrack = scoreExtension.MasterTrack
            });

            result.ScoreAttached = true;
        }
        else
        {
            result.ScoreUnmatched = true;
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
                result.TracksUnmatched++;
                CountUnmatchedTrackSubtree(targetTrack, result);
                continue;
            }

            var trackExtension = sourceTrack.GetGuitarPro();
            if (trackExtension is null)
            {
                result.TracksUnmatched++;
            }
            else
            {
                targetTrack.SetExtension(new GpTrackExtension
                {
                    Metadata = trackExtension.Metadata
                });

                result.TracksAttached++;
            }

            ReattachMeasureHierarchyExtensions(targetTrack, sourceTrack, result);
        }

        return result;
    }

    private static void ReattachMeasureHierarchyExtensions(TrackModel targetTrack, TrackModel sourceTrack, GpExtensionReattachmentResult result)
    {
        var sourceMeasuresByIndex = sourceTrack.Measures
            .GroupBy(measure => measure.Index)
            .ToDictionary(group => group.Key, group => group.First());

        foreach (var targetMeasure in targetTrack.Measures)
        {
            if (!sourceMeasuresByIndex.TryGetValue(targetMeasure.Index, out var sourceMeasure))
            {
                CountUnmatchedMeasureSubtree(targetMeasure, result);
                continue;
            }

            var measureExtension = sourceMeasure.GetGuitarPro();
            if (measureExtension is not null)
            {
                targetMeasure.SetExtension(new GpMeasureExtension
                {
                    Metadata = measureExtension.Metadata
                });

                result.MeasuresAttached++;
            }
            else
            {
                result.MeasuresUnmatched++;
            }

            var sourceStaffByIndex = sourceMeasure.AdditionalStaffBars
                .ToDictionary(staff => staff.StaffIndex);
            foreach (var targetStaff in targetMeasure.AdditionalStaffBars)
            {
                if (!sourceStaffByIndex.TryGetValue(targetStaff.StaffIndex, out var sourceStaff))
                {
                    CountUnmatchedStaffSubtree(targetStaff, result);
                    continue;
                }

                var staffExtension = sourceStaff.GetGuitarPro();
                if (staffExtension is null)
                {
                    result.StaffsUnmatched++;
                }
                else
                {
                    targetStaff.SetExtension(new GpMeasureStaffExtension
                    {
                        Metadata = staffExtension.Metadata
                    });

                    result.StaffsAttached++;
                }

                ReattachBeatExtensions(targetStaff.Beats, sourceStaff.Beats, result);

                var sourceStaffVoicesByIndex = sourceStaff.Voices
                    .ToDictionary(voice => voice.VoiceIndex);
                foreach (var targetStaffVoice in targetStaff.Voices)
                {
                    if (!sourceStaffVoicesByIndex.TryGetValue(targetStaffVoice.VoiceIndex, out var sourceStaffVoice))
                    {
                        CountUnmatchedVoiceSubtree(targetStaffVoice, result);
                        continue;
                    }

                    var staffVoiceExtension = sourceStaffVoice.GetGuitarPro();
                    if (staffVoiceExtension is null)
                    {
                        result.VoicesUnmatched++;
                    }
                    else
                    {
                        targetStaffVoice.SetExtension(new GpVoiceExtension
                        {
                            Metadata = staffVoiceExtension.Metadata
                        });

                        result.VoicesAttached++;
                    }

                    ReattachBeatExtensions(targetStaffVoice.Beats, sourceStaffVoice.Beats, result);
                }
            }

            var sourceVoiceByIndex = sourceMeasure.Voices
                .ToDictionary(voice => voice.VoiceIndex);
            foreach (var targetVoice in targetMeasure.Voices)
            {
                if (!sourceVoiceByIndex.TryGetValue(targetVoice.VoiceIndex, out var sourceVoice))
                {
                    CountUnmatchedVoiceSubtree(targetVoice, result);
                    continue;
                }

                var voiceExtension = sourceVoice.GetGuitarPro();
                if (voiceExtension is null)
                {
                    result.VoicesUnmatched++;
                }
                else
                {
                    targetVoice.SetExtension(new GpVoiceExtension
                    {
                        Metadata = voiceExtension.Metadata
                    });

                    result.VoicesAttached++;
                }

                ReattachBeatExtensions(targetVoice.Beats, sourceVoice.Beats, result);
            }

            ReattachBeatExtensions(targetMeasure.Beats, sourceMeasure.Beats, result);
        }
    }

    private static void ReattachBeatExtensions(
        IReadOnlyList<BeatModel> targetBeats,
        IReadOnlyList<BeatModel> sourceBeats,
        GpExtensionReattachmentResult result)
    {
        var sourceBeatsById = BuildItemsByIdQueue(sourceBeats, static beat => beat.Id);
        foreach (var targetBeat in targetBeats)
        {
            if (!TryDequeueMatchingItem(sourceBeatsById, targetBeat.Id, out var sourceBeat))
            {
                CountUnmatchedBeatSubtree(targetBeat, result);
                continue;
            }

            var beatExtension = sourceBeat.GetGuitarPro();
            if (beatExtension is not null)
            {
                targetBeat.SetExtension(new GpBeatExtension
                {
                    Metadata = beatExtension.Metadata
                });

                result.BeatsAttached++;
            }
            else
            {
                result.BeatsUnmatched++;
            }

            var sourceNotesById = BuildItemsByIdQueue(sourceBeat.Notes, static note => note.Id);
            foreach (var targetNote in targetBeat.Notes)
            {
                if (!TryDequeueMatchingItem(sourceNotesById, targetNote.Id, out var sourceNote))
                {
                    result.NotesUnmatched++;
                    continue;
                }

                var noteExtension = sourceNote.GetGuitarPro();
                if (noteExtension is null)
                {
                    result.NotesUnmatched++;
                    continue;
                }

                targetNote.SetExtension(new GpNoteExtension
                {
                    Metadata = noteExtension.Metadata
                });

                result.NotesAttached++;
            }
        }
    }

    private static void InvalidateMeasureHierarchyExtensions(TrackModel track)
    {
        foreach (var measure in track.Measures)
        {
            measure.RemoveExtension<GpMeasureExtension>();

            foreach (var staff in measure.AdditionalStaffBars)
            {
                staff.RemoveExtension<GpMeasureStaffExtension>();

                foreach (var voice in staff.Voices)
                {
                    voice.RemoveExtension<GpVoiceExtension>();
                    InvalidateBeatExtensions(voice.Beats);
                }

                InvalidateBeatExtensions(staff.Beats);
            }

            foreach (var voice in measure.Voices)
            {
                voice.RemoveExtension<GpVoiceExtension>();
                InvalidateBeatExtensions(voice.Beats);
            }

            InvalidateBeatExtensions(measure.Beats);
        }
    }

    private static void InvalidateBeatExtensions(IReadOnlyList<BeatModel> beats)
    {
        foreach (var beat in beats)
        {
            beat.RemoveExtension<GpBeatExtension>();

            foreach (var note in beat.Notes)
            {
                note.RemoveExtension<GpNoteExtension>();
            }
        }
    }

    private static void CountUnmatchedTrackSubtree(TrackModel track, GpExtensionReattachmentResult result)
    {
        foreach (var measure in track.Measures)
        {
            CountUnmatchedMeasureSubtree(measure, result);
        }
    }

    private static void CountUnmatchedMeasureSubtree(MeasureModel measure, GpExtensionReattachmentResult result)
    {
        result.MeasuresUnmatched++;

        foreach (var targetStaff in measure.AdditionalStaffBars)
        {
            CountUnmatchedStaffSubtree(targetStaff, result);
        }

        foreach (var targetVoice in measure.Voices)
        {
            CountUnmatchedVoiceSubtree(targetVoice, result);
        }

        CountUnmatchedBeatList(measure.Beats, result);
    }

    private static void CountUnmatchedStaffSubtree(MeasureStaffModel staff, GpExtensionReattachmentResult result)
    {
        result.StaffsUnmatched++;

        foreach (var targetVoice in staff.Voices)
        {
            CountUnmatchedVoiceSubtree(targetVoice, result);
        }

        CountUnmatchedBeatList(staff.Beats, result);
    }

    private static void CountUnmatchedVoiceSubtree(MeasureVoiceModel voice, GpExtensionReattachmentResult result)
    {
        result.VoicesUnmatched++;
        CountUnmatchedBeatList(voice.Beats, result);
    }

    private static void CountUnmatchedBeatSubtree(BeatModel beat, GpExtensionReattachmentResult result)
    {
        result.BeatsUnmatched++;
        result.NotesUnmatched += beat.Notes.Count;
    }

    private static void CountUnmatchedBeatList(IReadOnlyList<BeatModel> beats, GpExtensionReattachmentResult result)
    {
        foreach (var beat in beats)
        {
            CountUnmatchedBeatSubtree(beat, result);
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
