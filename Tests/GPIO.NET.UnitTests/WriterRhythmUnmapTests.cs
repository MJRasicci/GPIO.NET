namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using GPIO.NET.Models;

public class WriterRhythmUnmapTests
{
    [Fact]
    public async Task Unmapper_preserves_dotted_and_tuplet_rhythm_shapes_when_recognized()
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
                            Beats =
                            [
                                new BeatModel { Id = 1, Duration = 0.375m },      // dotted quarter
                                new BeatModel { Id = 2, Duration = 1m/12m }       // eighth triplet
                            ]
                        }
                    ]
                }
            ]
        };

        var unmapper = new DefaultScoreUnmapper();
        var result = await unmapper.UnmapAsync(score, TestContext.Current.CancellationToken);

        result.Diagnostics.Warnings.Should().BeEmpty();
        result.RawDocument.RhythmsById.Should().HaveCount(2);

        var rhythms = result.RawDocument.RhythmsById.Values.OrderBy(r => r.Id).ToArray();
        rhythms[0].NoteValue.Should().Be("Quarter");
        rhythms[0].AugmentationDots.Should().Be(1);

        rhythms[1].NoteValue.Should().Be("Eighth");
        rhythms[1].PrimaryTuplet.Should().NotBeNull();
        rhythms[1].PrimaryTuplet!.Numerator.Should().Be(3);
        rhythms[1].PrimaryTuplet!.Denominator.Should().Be(2);
    }
}
