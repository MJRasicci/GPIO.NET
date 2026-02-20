namespace GPIO.NET.Implementation;

using GPIO.NET.Abstractions;
using GPIO.NET.Models;
using GPIO.NET.Models.Raw;
using GPIO.NET.Utilities;

public sealed class DefaultScoreMapper : IScoreMapper
{
    public ValueTask<GuitarProScore> MapAsync(GpifDocument source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        cancellationToken.ThrowIfCancellationRequested();

        var tracks = source.Tracks
            .OrderBy(t => t.Id)
            .Select((track, trackOrdinal) => new TrackModel
            {
                Id = track.Id,
                Name = track.Name,
                Measures = MapMeasures(source, trackOrdinal)
            })
            .ToArray();

        var score = new GuitarProScore
        {
            Title = source.Score.Title,
            Artist = source.Score.Artist,
            Album = source.Score.Album,
            Tracks = tracks,
            PlaybackMasterBarSequence = BuildPlaybackSequence(source)
        };

        return ValueTask.FromResult(score);
    }

    private static List<MeasureModel> MapMeasures(GpifDocument source, int trackOrdinal)
    {
        var measures = new List<MeasureModel>(source.MasterBars.Count);

        foreach (var masterBar in source.MasterBars.OrderBy(m => m.Index))
        {
            var barRefs = ReferenceListParser.SplitRefs(masterBar.BarsReferenceList);
            var beats = new List<BeatModel>();
            var sourceBarId = -1;

            if (trackOrdinal < barRefs.Count && source.BarsById.TryGetValue(barRefs[trackOrdinal], out var bar))
            {
                sourceBarId = bar.Id;
                var voiceRefs = ReferenceListParser.SplitRefs(bar.VoicesReferenceList);
                if (voiceRefs.Count > 0 && source.VoicesById.TryGetValue(voiceRefs[0], out var voice))
                {
                    var beatRefs = ReferenceListParser.SplitRefs(voice.BeatsReferenceList);
                    decimal offset = 0;
                    foreach (var beatId in beatRefs)
                    {
                        if (!source.BeatsById.TryGetValue(beatId, out var beat))
                        {
                            continue;
                        }

                        var duration = ResolveDuration(source, beat.RhythmRef);
                        var midi = ReferenceListParser.SplitRefs(beat.NotesReferenceList)
                            .Select(id => source.NotesById.TryGetValue(id, out var n) ? n.MidiPitch : null)
                            .Where(p => p.HasValue)
                            .Select(p => p!.Value)
                            .ToArray();

                        beats.Add(new BeatModel
                        {
                            Id = beat.Id,
                            Offset = offset,
                            Duration = duration,
                            MidiPitches = midi
                        });

                        offset += duration;
                    }
                }
            }

            measures.Add(new MeasureModel
            {
                Index = masterBar.Index,
                TimeSignature = masterBar.Time,
                SourceBarId = sourceBarId,
                RepeatStart = masterBar.RepeatStart,
                RepeatEnd = masterBar.RepeatEnd,
                RepeatCount = masterBar.RepeatCount,
                AlternateEndings = masterBar.AlternateEndings,
                SectionLetter = masterBar.SectionLetter,
                SectionText = masterBar.SectionText,
                Jump = masterBar.Jump,
                Target = masterBar.Target,
                Beats = beats
            });
        }

        return measures;
    }

    private static IReadOnlyList<int> BuildPlaybackSequence(GpifDocument source)
    {
        var bars = source.MasterBars.OrderBy(m => m.Index).ToArray();
        var result = new List<int>(bars.Length * 2);

        var segnoIndex = bars.FirstOrDefault(b => string.Equals(b.Target, "Segno", StringComparison.OrdinalIgnoreCase))?.Index ?? 0;
        var codaIndex = bars.FirstOrDefault(b => string.Equals(b.Target, "Coda", StringComparison.OrdinalIgnoreCase))?.Index ?? -1;

        var repeatStack = new Stack<int>();
        var repeatVisits = new Dictionary<int, int>();
        var consumedJumps = new HashSet<int>();

        var cursor = 0;
        var guard = 0;
        while (cursor >= 0 && cursor < bars.Length && guard++ < 10000)
        {
            var bar = bars[cursor];

            if (bar.RepeatStart)
            {
                repeatStack.Push(cursor);
            }

            var endingVisit = repeatVisits.TryGetValue(cursor, out var visit) ? visit + 1 : 1;
            if (!ShouldPlayAlternateEnding(bar.AlternateEndings, endingVisit))
            {
                cursor++;
                continue;
            }

            result.Add(cursor);

            if (!consumedJumps.Contains(cursor))
            {
                if (TryResolveJump(bar.Jump, segnoIndex, codaIndex, out var jumpIndex))
                {
                    consumedJumps.Add(cursor);
                    cursor = jumpIndex;
                    continue;
                }
            }

            if (bar.RepeatEnd && repeatStack.Count > 0)
            {
                var start = repeatStack.Peek();
                var count = repeatVisits.TryGetValue(cursor, out var done) ? done : 0;
                var maxPasses = Math.Max(2, bar.RepeatCount <= 0 ? 2 : bar.RepeatCount);

                if (count < maxPasses - 1)
                {
                    repeatVisits[cursor] = count + 1;
                    cursor = start;
                    continue;
                }

                repeatStack.Pop();
            }

            cursor++;
        }

        return result;
    }

    private static bool ShouldPlayAlternateEnding(string alternateEndings, int repeatVisit)
    {
        var endings = ReferenceListParser.SplitRefs(alternateEndings);
        if (endings.Count == 0)
        {
            return true;
        }

        return endings.Contains(repeatVisit);
    }

    private static bool TryResolveJump(string jump, int segnoIndex, int codaIndex, out int index)
    {
        index = -1;
        if (string.IsNullOrWhiteSpace(jump))
        {
            return false;
        }

        if (jump.StartsWith("DaCapo", StringComparison.OrdinalIgnoreCase))
        {
            index = 0;
            return true;
        }

        if (jump.StartsWith("DaSegno", StringComparison.OrdinalIgnoreCase))
        {
            index = segnoIndex;
            return true;
        }

        if (jump.StartsWith("DaCoda", StringComparison.OrdinalIgnoreCase) && codaIndex >= 0)
        {
            index = codaIndex;
            return true;
        }

        return false;
    }

    private static decimal ResolveDuration(GpifDocument source, int rhythmRef)
    {
        if (!source.RhythmsById.TryGetValue(rhythmRef, out var rhythm))
        {
            return 0m;
        }

        return rhythm.NoteValue switch
        {
            "Whole" => 1m,
            "Half" => 1m / 2m,
            "Quarter" => 1m / 4m,
            "Eighth" => 1m / 8m,
            "16th" => 1m / 16m,
            "32nd" => 1m / 32m,
            "64th" => 1m / 64m,
            _ => 0m
        };
    }
}
