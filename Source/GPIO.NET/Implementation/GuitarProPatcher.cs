namespace GPIO.NET.Implementation;

using GPIO.NET.Abstractions;
using GPIO.NET.Models.Patching;
using System.IO.Compression;
using System.Xml.Linq;

public sealed class GuitarProPatcher : IGuitarProPatcher
{
    public async ValueTask PatchAsync(string sourceGpPath, string outputGpPath, GpPatchDocument patch, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(patch);

        if (!File.Exists(sourceGpPath))
        {
            throw new FileNotFoundException($"Source gp file not found: {sourceGpPath}", sourceGpPath);
        }

        var outDir = Path.GetDirectoryName(outputGpPath);
        if (!string.IsNullOrWhiteSpace(outDir))
        {
            Directory.CreateDirectory(outDir);
        }

        await using var source = await ZipFile.OpenReadAsync(sourceGpPath, cancellationToken).ConfigureAwait(false);
        var scoreEntry = source.GetEntry("Content/score.gpif") ?? throw new InvalidDataException("Archive missing Content/score.gpif");

        XDocument gpif;
        await using (var scoreStream = await scoreEntry.OpenAsync(cancellationToken).ConfigureAwait(false))
        {
            gpif = await XDocument.LoadAsync(scoreStream, LoadOptions.None, cancellationToken).ConfigureAwait(false);
        }

        ApplyPatch(gpif, patch);

        if (File.Exists(outputGpPath))
        {
            File.Delete(outputGpPath);
        }

        using var target = ZipFile.Open(outputGpPath, ZipArchiveMode.Create);
        foreach (var entry in source.Entries)
        {
            var targetEntry = target.CreateEntry(entry.FullName, CompressionLevel.Optimal);
            await using var inStream = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var outStream = await targetEntry.OpenAsync(cancellationToken).ConfigureAwait(false);

            if (string.Equals(entry.FullName, "Content/score.gpif", StringComparison.OrdinalIgnoreCase))
            {
                await gpif.SaveAsync(outStream, SaveOptions.None, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await inStream.CopyToAsync(outStream, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private static void ApplyPatch(XDocument gpif, GpPatchDocument patch)
    {
        var root = gpif.Root ?? throw new InvalidDataException("Invalid GPIF document.");

        var tracksEl = root.Element("Tracks") ?? throw new InvalidDataException("GPIF missing Tracks.");
        var masterBarsEl = root.Element("MasterBars") ?? throw new InvalidDataException("GPIF missing MasterBars.");
        var barsEl = root.Element("Bars") ?? throw new InvalidDataException("GPIF missing Bars.");
        var voicesEl = root.Element("Voices") ?? throw new InvalidDataException("GPIF missing Voices.");
        var beatsEl = root.Element("Beats") ?? throw new InvalidDataException("GPIF missing Beats.");
        var notesEl = root.Element("Notes") ?? throw new InvalidDataException("GPIF missing Notes.");
        var rhythmsEl = root.Element("Rhythms") ?? throw new InvalidDataException("GPIF missing Rhythms.");

        var trackList = tracksEl.Elements("Track").ToList();
        var masterBarList = masterBarsEl.Elements("MasterBar").ToList();

        foreach (var op in patch.AppendNotes)
        {
            var trackOrderIndex = trackList.FindIndex(t => ParseInt(t.Attribute("id")?.Value) == op.TrackId);
            if (trackOrderIndex < 0)
            {
                throw new InvalidOperationException($"Track id {op.TrackId} not found.");
            }

            if (op.MasterBarIndex < 0 || op.MasterBarIndex >= masterBarList.Count)
            {
                throw new InvalidOperationException($"Master bar index {op.MasterBarIndex} out of range.");
            }

            var masterBar = masterBarList[op.MasterBarIndex];
            var barRefs = SplitRefs(masterBar.Element("Bars")?.Value);
            if (trackOrderIndex >= barRefs.Count)
            {
                throw new InvalidOperationException($"Master bar {op.MasterBarIndex} has no bar for track order index {trackOrderIndex}.");
            }

            var barId = barRefs[trackOrderIndex];
            var barEl = barsEl.Elements("Bar").FirstOrDefault(b => ParseInt(b.Attribute("id")?.Value) == barId)
                        ?? throw new InvalidOperationException($"Bar id {barId} not found.");

            var voiceRefs = SplitRefs(barEl.Element("Voices")?.Value);
            if (op.VoiceIndex < 0 || op.VoiceIndex >= voiceRefs.Count)
            {
                throw new InvalidOperationException($"Voice index {op.VoiceIndex} not available for bar {barId}.");
            }

            var voiceId = voiceRefs[op.VoiceIndex];
            var voiceEl = voicesEl.Elements("Voice").FirstOrDefault(v => ParseInt(v.Attribute("id")?.Value) == voiceId)
                          ?? throw new InvalidOperationException($"Voice id {voiceId} not found.");

            var nextRhythmId = NextId(rhythmsEl, "Rhythm");
            var nextBeatId = NextId(beatsEl, "Beat");
            var nextNoteId = NextId(notesEl, "Note");

            rhythmsEl.Add(BuildRhythm(nextRhythmId, op));

            var noteIds = new List<int>();
            foreach (var midi in op.MidiPitches)
            {
                notesEl.Add(BuildNote(nextNoteId, midi));
                noteIds.Add(nextNoteId);
                nextNoteId++;
            }

            beatsEl.Add(new XElement("Beat",
                new XAttribute("id", nextBeatId),
                new XElement("Rhythm", new XAttribute("ref", nextRhythmId)),
                new XElement("Notes", JoinRefs(noteIds))));

            var beatRefs = SplitRefs(voiceEl.Element("Beats")?.Value);
            beatRefs.Add(nextBeatId);
            voiceEl.SetElementValue("Beats", JoinRefs(beatRefs));
        }
    }

    private static XElement BuildRhythm(int id, AppendNotesPatch op)
    {
        var rhythm = new XElement("Rhythm",
            new XAttribute("id", id),
            new XElement("NoteValue", op.RhythmNoteValue));

        for (var i = 0; i < op.AugmentationDots; i++)
        {
            rhythm.Add(new XElement("AugmentationDot"));
        }

        if (op.TupletNumerator is > 0 && op.TupletDenominator is > 0)
        {
            rhythm.Add(new XElement("PrimaryTuplet",
                new XElement("Num", op.TupletNumerator.Value),
                new XElement("Den", op.TupletDenominator.Value)));
        }

        return rhythm;
    }

    private static XElement BuildNote(int id, int midi)
    {
        var (step, accidental, octave) = FromMidi(midi);
        return new XElement("Note",
            new XAttribute("id", id),
            new XElement("Properties",
                new XElement("Property",
                    new XAttribute("name", "Pitch"),
                    new XElement("Pitch",
                        new XElement("Step", step),
                        new XElement("Accidental", accidental),
                        new XElement("Octave", octave)))));
    }

    private static int NextId(XElement container, string elementName)
    {
        var max = container.Elements(elementName)
            .Select(e => ParseInt(e.Attribute("id")?.Value))
            .DefaultIfEmpty(0)
            .Max();
        return max + 1;
    }

    private static int ParseInt(string? value) => int.TryParse(value, out var i) ? i : -1;

    private static List<int> SplitRefs(string? refs)
        => string.IsNullOrWhiteSpace(refs)
            ? []
            : refs.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ParseInt).Where(i => i >= 0).ToList();

    private static string JoinRefs(IEnumerable<int> ids) => string.Join(' ', ids);

    private static (string step, string accidental, int octave) FromMidi(int midi)
    {
        var octave = midi / 12;
        var cls = midi % 12;
        return cls switch
        {
            0 => ("C", "", octave),
            1 => ("C", "#", octave),
            2 => ("D", "", octave),
            3 => ("D", "#", octave),
            4 => ("E", "", octave),
            5 => ("F", "", octave),
            6 => ("F", "#", octave),
            7 => ("G", "", octave),
            8 => ("G", "#", octave),
            9 => ("A", "", octave),
            10 => ("A", "#", octave),
            11 => ("B", "", octave),
            _ => ("C", "", octave)
        };
    }
}
