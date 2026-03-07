namespace GPIO.NET.Tool.Cli;

using GPIO.NET.Models;
using GPIO.NET.Models.Patching;

internal static class JsonPatchPlanner
{
    public static JsonPatchPlanResult BuildPatch(GuitarProScore source, GuitarProScore edited)
    {
        var appendNotes = new List<AppendNotesPatch>();
        var insertBeats = new List<InsertBeatPatch>();
        var updateArticulations = new List<UpdateNoteArticulationPatch>();
        var updatePitches = new List<UpdateNotePitchPatch>();
        var deleteNotes = new List<DeleteNotePatch>();
        var deleteBeats = new List<DeleteBeatPatch>();
        var addNotesToBeats = new List<AddNotesToBeatPatch>();
        var reorderBeatNotes = new List<ReorderBeatNotesPatch>();
        var unsupported = new List<string>();

        foreach (var editedTrack in edited.Tracks)
        {
            var sourceTrack = source.Tracks.FirstOrDefault(t => t.Id == editedTrack.Id);
            if (sourceTrack is null)
            {
                unsupported.Add($"Track {editedTrack.Id} does not exist in source (track creation patch not auto-planned yet).");
                continue;
            }

            var maxMeasures = Math.Min(sourceTrack.Measures.Count, editedTrack.Measures.Count);
            for (var mi = 0; mi < maxMeasures; mi++)
            {
                var srcMeasure = sourceTrack.Measures[mi];
                var edMeasure = editedTrack.Measures[mi];
                var srcBeatsById = BuildOccurrenceMap(srcMeasure.Beats, beat => beat.Id);
                var editedBeatCounts = CountOccurrences(edMeasure.Beats, beat => beat.Id);
                foreach (var srcBeatGroup in srcBeatsById)
                {
                    var sourceCount = srcBeatGroup.Value.Count;
                    var editedCount = editedBeatCounts.GetValueOrDefault(srcBeatGroup.Key, 0);
                    if (editedCount == 0)
                    {
                        if (sourceCount == 1)
                        {
                            deleteBeats.Add(new DeleteBeatPatch { BeatId = srcBeatGroup.Key });
                        }
                        else
                        {
                            unsupported.Add(
                                $"Track {editedTrack.Id}, Measure {mi}: shared Beat {srcBeatGroup.Key} was removed entirely ({sourceCount} -> 0); shared beat reference deletions are not auto-planned yet.");
                        }

                        continue;
                    }

                    if (editedCount < sourceCount)
                    {
                        unsupported.Add(
                            $"Track {editedTrack.Id}, Measure {mi}: shared Beat {srcBeatGroup.Key} occurrence count changed ({sourceCount} -> {editedCount}); shared beat reference diffs are not auto-planned yet.");
                    }
                }

                var consumedBeatCounts = new Dictionary<int, int>();

                for (var bi = 0; bi < edMeasure.Beats.Count; bi++)
                {
                    var edBeat = edMeasure.Beats[bi];

                    if (TryTakeOccurrence(srcBeatsById, consumedBeatCounts, edBeat.Id, out var srcBeat))
                    {
                        var srcNotesById = BuildOccurrenceMap(srcBeat.Notes, note => note.Id);
                        var editedNoteCounts = CountOccurrences(edBeat.Notes, note => note.Id);
                        foreach (var srcNoteGroup in srcNotesById)
                        {
                            var sourceCount = srcNoteGroup.Value.Count;
                            var editedCount = editedNoteCounts.GetValueOrDefault(srcNoteGroup.Key, 0);
                            if (editedCount == 0)
                            {
                                if (sourceCount == 1)
                                {
                                    deleteNotes.Add(new DeleteNotePatch { NoteId = srcNoteGroup.Key });
                                }
                                else
                                {
                                    unsupported.Add(
                                        $"Track {editedTrack.Id}, Measure {mi}, Beat {srcBeat.Id}: shared Note {srcNoteGroup.Key} was removed entirely ({sourceCount} -> 0); shared note reference deletions are not auto-planned yet.");
                                }

                                continue;
                            }

                            if (editedCount < sourceCount)
                            {
                                unsupported.Add(
                                    $"Track {editedTrack.Id}, Measure {mi}, Beat {srcBeat.Id}: shared Note {srcNoteGroup.Key} occurrence count changed ({sourceCount} -> {editedCount}); shared note reference diffs are not auto-planned yet.");
                            }
                        }

                        var consumedNoteCounts = new Dictionary<int, int>();
                        var notesToAdd = new List<int>();
                        var matchedExistingNoteIds = new List<int>();
                        foreach (var edNote in edBeat.Notes)
                        {
                            if (!TryTakeOccurrence(srcNotesById, consumedNoteCounts, edNote.Id, out var srcNote))
                            {
                                if (edNote.MidiPitch.HasValue)
                                {
                                    notesToAdd.Add(edNote.MidiPitch.Value);
                                }

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

                            matchedExistingNoteIds.Add(edNote.Id);
                        }

                        if (notesToAdd.Count > 0)
                        {
                            addNotesToBeats.Add(new AddNotesToBeatPatch
                            {
                                BeatId = edBeat.Id,
                                MidiPitches = notesToAdd
                            });
                        }

                        var srcOrder = srcBeat.Notes.Where(n => n.Id >= 0).Select(n => n.Id).ToArray();
                        var editedOrder = matchedExistingNoteIds.ToArray();
                        if (editedOrder.Length == srcOrder.Length && !srcOrder.SequenceEqual(editedOrder))
                        {
                            reorderBeatNotes.Add(new ReorderBeatNotesPatch
                            {
                                BeatId = edBeat.Id,
                                NoteIds = editedOrder
                            });
                        }

                        continue;
                    }

                    // New beat (negative or unknown, or an extra occurrence beyond the source count).
                    var midiPitches = edBeat.Notes.Where(n => n.MidiPitch.HasValue).Select(n => n.MidiPitch!.Value).ToArray();
                    var op = new InsertBeatPatch
                    {
                        TrackId = editedTrack.Id,
                        MasterBarIndex = mi,
                        VoiceIndex = 0,
                        BeatInsertIndex = bi,
                        RhythmNoteValue = ToRawNoteValue(edBeat.Duration),
                        AugmentationDots = GuessAugmentationDots(edBeat.Duration),
                        MidiPitches = midiPitches
                    };

                    if (bi < srcMeasure.Beats.Count)
                    {
                        insertBeats.Add(op);
                    }
                    else
                    {
                        appendNotes.Add(new AppendNotesPatch
                        {
                            TrackId = op.TrackId,
                            MasterBarIndex = op.MasterBarIndex,
                            VoiceIndex = op.VoiceIndex,
                            RhythmNoteValue = op.RhythmNoteValue,
                            AugmentationDots = op.AugmentationDots,
                            MidiPitches = op.MidiPitches
                        });
                    }
                }

                // beat deletions are auto-planned via DeleteBeatPatch
            }

            if (editedTrack.Measures.Count > sourceTrack.Measures.Count)
            {
                unsupported.Add($"Track {editedTrack.Id}: appended measures are not auto-planned yet.");
            }
        }

        return new JsonPatchPlanResult
        {
            Patch = new GpPatchDocument
            {
                AppendNotes = appendNotes,
                InsertBeats = insertBeats,
                UpdateNoteArticulations = updateArticulations,
                UpdateNotePitches = updatePitches,
                DeleteNotes = deleteNotes,
                DeleteBeats = deleteBeats,
                AddNotesToBeats = addNotesToBeats,
                ReorderBeatNotes = reorderBeatNotes
            },
            UnsupportedChanges = unsupported
        };
    }

    private static Dictionary<int, List<T>> BuildOccurrenceMap<T>(IEnumerable<T> items, Func<T, int> idSelector)
        where T : class
    {
        var occurrences = new Dictionary<int, List<T>>();

        foreach (var item in items)
        {
            var id = idSelector(item);
            if (id < 0)
            {
                continue;
            }

            if (!occurrences.TryGetValue(id, out var list))
            {
                list = new List<T>();
                occurrences[id] = list;
            }

            list.Add(item);
        }

        return occurrences;
    }

    private static Dictionary<int, int> CountOccurrences<T>(IEnumerable<T> items, Func<T, int> idSelector)
    {
        var counts = new Dictionary<int, int>();

        foreach (var item in items)
        {
            var id = idSelector(item);
            if (id < 0)
            {
                continue;
            }

            counts[id] = counts.TryGetValue(id, out var count)
                ? count + 1
                : 1;
        }

        return counts;
    }

    private static bool TryTakeOccurrence<T>(
        IReadOnlyDictionary<int, List<T>> occurrencesById,
        Dictionary<int, int> consumedCounts,
        int id,
        out T? item)
        where T : class
    {
        item = null;
        if (id < 0 || !occurrencesById.TryGetValue(id, out var occurrences))
        {
            return false;
        }

        var consumed = consumedCounts.GetValueOrDefault(id);
        if (consumed >= occurrences.Count)
        {
            return false;
        }

        consumedCounts[id] = consumed + 1;
        item = occurrences[consumed];
        return true;
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
        => duration is 0.375m or 0.75m or 0.1875m ? 1 : 0;
}
