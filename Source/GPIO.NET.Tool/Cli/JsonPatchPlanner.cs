namespace GPIO.NET.Tool.Cli;

using GPIO.NET.Models;
using GPIO.NET.Models.Patching;

internal static class JsonPatchPlanner
{
    public static GpPatchDocument BuildPatch(GuitarProScore source, GuitarProScore edited)
    {
        var appendNotes = new List<AppendNotesPatch>();
        var updateArticulations = new List<UpdateNoteArticulationPatch>();
        var updatePitches = new List<UpdateNotePitchPatch>();

        foreach (var editedTrack in edited.Tracks)
        {
            var sourceTrack = source.Tracks.FirstOrDefault(t => t.Id == editedTrack.Id);
            if (sourceTrack is null)
            {
                continue;
            }

            var maxMeasures = Math.Min(sourceTrack.Measures.Count, editedTrack.Measures.Count);
            for (var mi = 0; mi < maxMeasures; mi++)
            {
                var srcMeasure = sourceTrack.Measures[mi];
                var edMeasure = editedTrack.Measures[mi];

                var commonBeatCount = Math.Min(srcMeasure.Beats.Count, edMeasure.Beats.Count);
                for (var bi = 0; bi < commonBeatCount; bi++)
                {
                    var srcBeat = srcMeasure.Beats[bi];
                    var edBeat = edMeasure.Beats[bi];

                    foreach (var edNote in edBeat.Notes)
                    {
                        var srcNote = srcBeat.Notes.FirstOrDefault(n => n.Id == edNote.Id);
                        if (srcNote is null)
                        {
                            continue;
                        }

                        if (srcNote.MidiPitch != edNote.MidiPitch && edNote.MidiPitch.HasValue)
                        {
                            updatePitches.Add(new UpdateNotePitchPatch
                            {
                                NoteId = edNote.Id,
                                MidiPitch = edNote.MidiPitch.Value
                            });
                        }

                        var patch = BuildArticulationPatch(srcNote, edNote);
                        if (patch is not null)
                        {
                            updateArticulations.Add(patch);
                        }
                    }
                }

                // append-only for extra beats (safe for first patch pass)
                for (var bi = srcMeasure.Beats.Count; bi < edMeasure.Beats.Count; bi++)
                {
                    var beat = edMeasure.Beats[bi];
                    appendNotes.Add(new AppendNotesPatch
                    {
                        TrackId = editedTrack.Id,
                        MasterBarIndex = mi,
                        VoiceIndex = 0,
                        RhythmNoteValue = ToRawNoteValue(beat.Duration),
                        AugmentationDots = GuessAugmentationDots(beat.Duration),
                        MidiPitches = beat.Notes.Where(n => n.MidiPitch.HasValue).Select(n => n.MidiPitch!.Value).ToArray()
                    });
                }
            }
        }

        return new GpPatchDocument
        {
            AppendNotes = appendNotes,
            UpdateNoteArticulations = updateArticulations,
            UpdateNotePitches = updatePitches
        };
    }

    private static UpdateNoteArticulationPatch? BuildArticulationPatch(NoteModel src, NoteModel edited)
    {
        bool? ChangeBool(bool a, bool b) => a == b ? null : b;
        int? ChangeInt(int? a, int? b) => a == b ? null : b;

        var patch = new UpdateNoteArticulationPatch
        {
            NoteId = edited.Id,
            LetRing = ChangeBool(src.Articulation.LetRing, edited.Articulation.LetRing),
            PalmMuted = ChangeBool(src.Articulation.PalmMuted, edited.Articulation.PalmMuted),
            Muted = ChangeBool(src.Articulation.Muted, edited.Articulation.Muted),
            HopoOrigin = ChangeBool(src.Articulation.HopoOrigin, edited.Articulation.HopoOrigin),
            HopoDestination = ChangeBool(src.Articulation.HopoDestination, edited.Articulation.HopoDestination),
            SlideFlags = ChangeInt(src.Articulation.SlideFlags, edited.Articulation.SlideFlags)
        };

        if (!patch.LetRing.HasValue && !patch.PalmMuted.HasValue && !patch.Muted.HasValue && !patch.HopoOrigin.HasValue && !patch.HopoDestination.HasValue && !patch.SlideFlags.HasValue)
        {
            return null;
        }

        return patch;
    }

    private static string ToRawNoteValue(decimal duration)
    {
        if (duration >= 1m) return "Whole";
        if (duration >= 0.5m) return "Half";
        if (duration >= 0.25m) return "Quarter";
        if (duration >= 0.125m) return "Eighth";
        if (duration >= 0.0625m) return "16th";
        if (duration >= 0.03125m) return "32nd";
        return "64th";
    }

    private static int GuessAugmentationDots(decimal duration)
    {
        // tiny heuristic for common dotted values
        if (duration == 0.375m || duration == 0.75m || duration == 0.1875m) return 1;
        return 0;
    }
}
