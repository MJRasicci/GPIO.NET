namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using GPIO.NET.Models;
using System.Globalization;

public class WriterReferenceReuseTests
{
    [Fact]
    public async Task Unmapper_preserves_shared_reference_counts_for_simple_score_fixture()
    {
        var repoRoot = FindRepositoryRoot();
        var sourceGp = Path.Combine(repoRoot, "Temp", "SimpleScore.gp");
        File.Exists(sourceGp).Should().BeTrue();

        var reader = new GPIO.NET.GuitarProReader();
        var score = await reader.ReadAsync(sourceGp, cancellationToken: TestContext.Current.CancellationToken);

        var unmapper = new DefaultScoreUnmapper();
        var result = await unmapper.UnmapAsync(score, TestContext.Current.CancellationToken);

        result.Diagnostics.Warnings.Should().BeEmpty();
        result.RawDocument.BarsById.Should().HaveCount(4);
        result.RawDocument.VoicesById.Should().HaveCount(4);
        result.RawDocument.BeatsById.Should().HaveCount(6);
        result.RawDocument.NotesById.Should().HaveCount(12);
        result.RawDocument.RhythmsById.Should().HaveCount(1);
        result.RawDocument.VoicesById.Values.Sum(v => SplitRefs(v.BeatsReferenceList).Count).Should().Be(16);
        result.RawDocument.BeatsById.Values.Sum(b => SplitRefs(b.NotesReferenceList).Count).Should().Be(28);
    }

    [Fact]
    public async Task Unmapper_reuses_shared_beats_notes_and_rhythms_when_ids_repeat()
    {
        var score = new GuitarProScore
        {
            Tracks =
            [
                new TrackModel
                {
                    Id = 0,
                    Name = "Guitar",
                    Measures =
                    [
                        new MeasureModel
                        {
                            Index = 0,
                            TimeSignature = "4/4",
                            Voices =
                            [
                                new MeasureVoiceModel
                                {
                                    VoiceIndex = 0,
                                    SourceVoiceId = 0,
                                    Beats =
                                    [
                                        CreateBeat(0, CreateNote(0, 48), CreateNote(1, 52)),
                                        CreateBeat(0, CreateNote(0, 48), CreateNote(1, 52)),
                                        CreateBeat(0, CreateNote(0, 48), CreateNote(1, 52)),
                                        CreateBeat(1, CreateNote(2, 55))
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        var unmapper = new DefaultScoreUnmapper();
        var result = await unmapper.UnmapAsync(score, TestContext.Current.CancellationToken);

        result.Diagnostics.Warnings.Should().BeEmpty();
        result.RawDocument.BeatsById.Should().HaveCount(2);
        result.RawDocument.NotesById.Should().HaveCount(3);
        result.RawDocument.RhythmsById.Should().HaveCount(1);

        var voice = result.RawDocument.VoicesById[0];
        voice.BeatsReferenceList.Should().Be("0 0 0 1");

        result.RawDocument.BeatsById[0].NotesReferenceList.Should().Be("0 1");
        result.RawDocument.BeatsById[1].NotesReferenceList.Should().Be("2");
    }

    [Fact]
    public async Task Unmapper_preserves_measure_with_zero_voices_without_synthesizing_empty_voice()
    {
        var score = new GuitarProScore
        {
            Tracks =
            [
                new TrackModel
                {
                    Id = 0,
                    Name = "Guitar",
                    Measures =
                    [
                        new MeasureModel
                        {
                            Index = 0,
                            TimeSignature = "4/4",
                            SourceBarId = 5,
                            Voices = [],
                            Beats = []
                        }
                    ]
                }
            ]
        };

        var unmapper = new DefaultScoreUnmapper();
        var result = await unmapper.UnmapAsync(score, TestContext.Current.CancellationToken);

        result.Diagnostics.Warnings.Should().BeEmpty();
        result.RawDocument.VoicesById.Should().BeEmpty();
        result.RawDocument.BarsById.Should().ContainKey(5);
        result.RawDocument.BarsById[5].VoicesReferenceList.Should().Be("-1 -1 -1 -1");
        result.RawDocument.MasterBars.Should().ContainSingle();
        result.RawDocument.MasterBars[0].BarsReferenceList.Should().Be("5");
    }

    private static BeatModel CreateBeat(int id, params NoteModel[] notes)
        => new()
        {
            Id = id,
            Duration = 0.25m,
            Notes = notes
        };

    private static NoteModel CreateNote(int id, int midiPitch)
        => new()
        {
            Id = id,
            MidiPitch = midiPitch
        };

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Source", "GPIO.NET.Tool", "GPIO.NET.Tool.csproj")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root from test output directory.");
    }

    private static IReadOnlyList<int> SplitRefs(string refs)
        => refs.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(value => int.Parse(value, CultureInfo.InvariantCulture) >= 0)
            .Select(value => int.Parse(value, CultureInfo.InvariantCulture))
            .ToArray();
}
