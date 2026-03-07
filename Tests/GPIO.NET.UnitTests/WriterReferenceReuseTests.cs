namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using GPIO.NET.Models;
using System.Globalization;
using System.Text.Json;

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

    [Fact]
    public async Task Unmapper_does_not_copy_master_bar_xproperties_onto_written_bars()
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
                            XProperties = new Dictionary<string, int>
                            {
                                ["687931393"] = 60
                            }
                        }
                    ]
                }
            ]
        };

        var unmapper = new DefaultScoreUnmapper();
        var result = await unmapper.UnmapAsync(score, TestContext.Current.CancellationToken);

        result.Diagnostics.Warnings.Should().BeEmpty();
        result.RawDocument.MasterBars.Should().ContainSingle();
        result.RawDocument.MasterBars[0].XProperties.Should().Contain("687931393", 60);
        result.RawDocument.BarsById.Should().ContainSingle();
        result.RawDocument.BarsById.Values.Single().XProperties.Should().BeEmpty();
    }

    [Fact]
    public async Task Reader_maps_multistaff_master_bar_slots_without_shifting_following_tracks()
    {
        var repoRoot = FindRepositoryRoot();
        var sourceGp = Path.Combine(repoRoot, "Temp", "Last To Know.gp");
        File.Exists(sourceGp).Should().BeTrue();

        var reader = new GPIO.NET.GuitarProReader();
        var score = await reader.ReadAsync(sourceGp, cancellationToken: TestContext.Current.CancellationToken);

        var piano = score.Tracks.Single(track => track.Id == 9);
        var drums = score.Tracks.Single(track => track.Id == 10);

        piano.Measures.Should().NotBeEmpty();
        piano.Measures[0].Clef.Should().Be("G2");
        piano.Measures[0].AdditionalStaffBars.Should().ContainSingle();
        piano.Measures[0].AdditionalStaffBars[0].StaffIndex.Should().Be(1);
        piano.Measures[0].AdditionalStaffBars[0].Clef.Should().Be("F4");

        drums.Measures.Should().NotBeEmpty();
        drums.Measures[0].SourceBarId.Should().Be(11);
        drums.Measures[0].Clef.Should().Be("Neutral");
    }

    [Fact]
    public async Task Json_round_trip_preserves_raw_counts_for_last_to_know_fixture()
    {
        var repoRoot = FindRepositoryRoot();
        var sourceGp = Path.Combine(repoRoot, "Temp", "Last To Know.gp");
        File.Exists(sourceGp).Should().BeTrue();

        var raw = await ReadJsonRoundTrippedRawAsync(sourceGp);

        raw.BarsById.Should().HaveCount(684);
        raw.VoicesById.Should().HaveCount(621);
        raw.BeatsById.Should().HaveCount(706);
        raw.NotesById.Should().HaveCount(542);
        raw.RhythmsById.Should().HaveCount(11);
        raw.MasterBars.Should().OnlyContain(masterBar => SplitRefs(masterBar.BarsReferenceList).Count == 12);
    }

    [Fact]
    public async Task Json_round_trip_preserves_raw_counts_for_schema_reference_fixture()
    {
        var repoRoot = FindRepositoryRoot();
        var sourceGp = Path.Combine(repoRoot, "Temp", "schema-reference.gp");
        File.Exists(sourceGp).Should().BeTrue();

        var raw = await ReadJsonRoundTrippedRawAsync(sourceGp);

        raw.BarsById.Should().HaveCount(45);
        raw.VoicesById.Should().HaveCount(48);
        raw.BeatsById.Should().HaveCount(164);
        raw.NotesById.Should().HaveCount(111);
        raw.RhythmsById.Should().HaveCount(15);
        raw.MasterBars.Should().OnlyContain(masterBar => SplitRefs(masterBar.BarsReferenceList).Count == 1);
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

    private static async Task<GPIO.NET.Models.Raw.GpifDocument> ReadJsonRoundTrippedRawAsync(string gpPath)
    {
        var reader = new GPIO.NET.GuitarProReader();
        var score = await reader.ReadAsync(gpPath, cancellationToken: TestContext.Current.CancellationToken);

        var json = score.ToJson(indented: false);
        var fromJson = JsonSerializer.Deserialize<GuitarProScore>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        fromJson.Should().NotBeNull();

        var unmapper = new DefaultScoreUnmapper();
        var writeResult = await unmapper.UnmapAsync(fromJson!, TestContext.Current.CancellationToken);
        writeResult.Diagnostics.Warnings.Should().BeEmpty();
        return writeResult.RawDocument;
    }

    private static IReadOnlyList<int> SplitRefs(string refs)
        => refs.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(value => int.Parse(value, CultureInfo.InvariantCulture) >= 0)
            .Select(value => int.Parse(value, CultureInfo.InvariantCulture))
            .ToArray();
}
