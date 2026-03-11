namespace Motif.Extensions.GuitarPro.UnitTests;

using FluentAssertions;
using Motif.Extensions.GuitarPro.Models;
using Motif.Models;
using System.Text.Json;

public class GuitarProExtensionAttachmentTests
{
    [Fact]
    public async Task Reader_attaches_score_track_measure_voice_beat_and_note_guitar_pro_extensions()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test.gp");

        var score = await new GuitarProReader().ReadAsync(fixturePath, cancellationToken: TestContext.Current.CancellationToken);

        score.GetGuitarPro().Should().NotBeNull();
        score.GetRequiredGuitarPro().Metadata.ScoreXml.Should().Contain("<Score");
        score.GetRequiredGuitarPro().MasterTrack.TrackIds.Should().NotBeEmpty();

        score.Tracks.Should().NotBeEmpty();
        score.Tracks[0].GetGuitarPro().Should().NotBeNull();
        score.Tracks[0].GetRequiredGuitarPro().Metadata.Xml.Should().Contain("<Track");
        score.Tracks[0].Measures[0].GetGuitarPro().Should().NotBeNull();
        score.Tracks[0].Measures[0].GetRequiredGuitarPro().Metadata.MasterBarXml.Should().Contain("<MasterBar");
        score.Tracks[0].Measures[0].Voices[0].GetGuitarPro().Should().NotBeNull();
        score.Tracks[0].Measures[0].Voices[0].GetRequiredGuitarPro().Metadata.Xml.Should().Contain("<Voice");
        score.Tracks[0].Measures[0].Beats[0].GetGuitarPro().Should().NotBeNull();
        score.Tracks[0].Measures[0].Beats[0].GetRequiredGuitarPro().Metadata.Xml.Should().Contain("<Beat");
        score.Tracks[0].Measures[0].Beats[0].Notes[0].GetGuitarPro().Should().NotBeNull();
        score.Tracks[0].Measures[0].Beats[0].Notes[0].GetRequiredGuitarPro().Metadata.Xml.Should().Contain("<Note");
    }

    [Fact]
    public async Task Json_round_trip_can_reattach_score_track_measure_voice_beat_and_note_guitar_pro_extensions_from_source_score()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test.gp");
        var sourceScore = await new GuitarProReader().ReadAsync(fixturePath, cancellationToken: TestContext.Current.CancellationToken);
        var json = sourceScore.ToJson(indented: false);
        var fromJson = JsonSerializer.Deserialize<Score>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        fromJson.Should().NotBeNull();
        fromJson!.GetGuitarPro().Should().BeNull();
        fromJson.Tracks[0].GetGuitarPro().Should().BeNull();
        fromJson.Tracks[0].Measures[0].GetGuitarPro().Should().BeNull();
        fromJson.Tracks[0].Measures[0].Voices[0].GetGuitarPro().Should().BeNull();
        fromJson.Tracks[0].Measures[0].Beats[0].GetGuitarPro().Should().BeNull();
        fromJson.Tracks[0].Measures[0].Beats[0].Notes[0].GetGuitarPro().Should().BeNull();

        var reattachment = fromJson.ReattachGuitarProExtensionsFrom(sourceScore);

        fromJson.GetRequiredGuitarPro().Metadata.ScoreXml.Should().Contain("<Score");
        fromJson.GetRequiredGuitarPro().MasterTrack.TrackIds.Should().NotBeEmpty();
        fromJson.Tracks[0].GetRequiredGuitarPro().Metadata.Xml.Should().Contain("<Track");
        fromJson.Tracks[0].Measures[0].GetRequiredGuitarPro().Metadata.MasterBarXml.Should().Contain("<MasterBar");
        fromJson.Tracks[0].Measures[0].Voices[0].GetRequiredGuitarPro().Metadata.Xml.Should().Contain("<Voice");
        fromJson.Tracks[0].Measures[0].Beats[0].GetRequiredGuitarPro().Metadata.Xml.Should().Contain("<Beat");
        fromJson.Tracks[0].Measures[0].Beats[0].Notes[0].GetRequiredGuitarPro().Metadata.Xml.Should().Contain("<Note");
        reattachment.ScoreAttached.Should().BeTrue();
        reattachment.HasUnmatchedTargets.Should().BeFalse();
        reattachment.TracksAttached.Should().Be(fromJson.Tracks.Count);
        reattachment.MeasuresAttached.Should().Be(fromJson.Tracks.Sum(track => track.Measures.Count));
        reattachment.VoicesAttached.Should().BeGreaterThan(0);
        reattachment.BeatsAttached.Should().BeGreaterThan(0);
        reattachment.NotesAttached.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task InvalidateGuitarProExtensions_removes_existing_extensions_from_the_full_score_tree()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test.gp");
        var score = await new GuitarProReader().ReadAsync(fixturePath, cancellationToken: TestContext.Current.CancellationToken);

        score.InvalidateGuitarProExtensions();

        score.GetGuitarPro().Should().BeNull();
        score.Tracks[0].GetGuitarPro().Should().BeNull();
        score.Tracks[0].Measures[0].GetGuitarPro().Should().BeNull();
        score.Tracks[0].Measures[0].Voices[0].GetGuitarPro().Should().BeNull();
        score.Tracks[0].Measures[0].Beats[0].GetGuitarPro().Should().BeNull();
        score.Tracks[0].Measures[0].Beats[0].Notes[0].GetGuitarPro().Should().BeNull();
    }

    [Fact]
    public async Task ReattachGuitarProExtensionsFrom_matches_measures_by_index_and_clears_stale_target_extensions()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test.gp");
        var sourceScore = await new GuitarProReader().ReadAsync(fixturePath, cancellationToken: TestContext.Current.CancellationToken);
        var sourceTrack = sourceScore.Tracks[0];
        var sourceMeasure = sourceTrack.Measures[0];

        var target = new Score
        {
            Tracks =
            [
                new TrackModel
                {
                    Id = sourceTrack.Id,
                    Measures =
                    [
                        new MeasureModel
                        {
                            Index = 999,
                            Beats =
                            [
                                new BeatModel
                                {
                                    Id = -1,
                                    Notes =
                                    [
                                        new NoteModel
                                        {
                                            Id = -2
                                        }
                                    ]
                                }
                            ]
                        },
                        new MeasureModel
                        {
                            Index = sourceMeasure.Index,
                            Beats = sourceMeasure.Beats.Select(CloneBeat).ToArray()
                        }
                    ]
                }
            ]
        };
        target.Tracks[0].Measures[0].SetExtension(new GpMeasureExtension
        {
            Metadata = new GpMeasureMetadata
            {
                MasterBarXml = "<stale />"
            }
        });

        var reattachment = target.ReattachGuitarProExtensionsFrom(sourceScore);

        reattachment.MeasuresAttached.Should().Be(1);
        reattachment.MeasuresUnmatched.Should().BeGreaterThan(0);
        reattachment.HasUnmatchedTargets.Should().BeTrue();
        target.Tracks[0].Measures[1].GetRequiredGuitarPro().Metadata.MasterBarXml
            .Should().Be(sourceMeasure.GetRequiredGuitarPro().Metadata.MasterBarXml);
        target.Tracks[0].Measures[0].GetGuitarPro().Should().BeNull();
    }

    private static BeatModel CloneBeat(BeatModel beat)
        => new()
        {
            Id = beat.Id,
            Notes = beat.Notes.Select(note => new NoteModel
            {
                Id = note.Id
            }).ToArray()
        };
}
