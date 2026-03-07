namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using GPIO.NET.Models;

public class WriterNotePropertyFidelityTests
{
    [Fact]
    public async Task Unmapper_uses_staff_specific_tuning_for_additional_staff_notes()
    {
        var score = new GuitarProScore
        {
            Tracks =
            [
                new TrackModel
                {
                    Id = 0,
                    Name = "Piano",
                    Metadata = new TrackMetadata
                    {
                        TuningPitches = [40, 45, 50, 55, 59, 64],
                        Staffs =
                        [
                            new StaffMetadata
                            {
                                TuningPitches = [40, 45, 50, 55, 59, 64]
                            },
                            new StaffMetadata
                            {
                                TuningPitches = [23, 28, 33, 38, 43]
                            }
                        ]
                    },
                    Measures =
                    [
                        new MeasureModel
                        {
                            Index = 0,
                            TimeSignature = "4/4",
                            AdditionalStaffBars =
                            [
                                new MeasureStaffModel
                                {
                                    StaffIndex = 1,
                                    Beats =
                                    [
                                        new BeatModel
                                        {
                                            Id = 1,
                                            Duration = 0.5m,
                                            Notes =
                                            [
                                                new NoteModel
                                                {
                                                    Id = 22,
                                                    MidiPitch = 40,
                                                    StringNumber = 3
                                                }
                                            ]
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        var result = await new DefaultScoreUnmapper().UnmapAsync(score, TestContext.Current.CancellationToken);
        var note = result.RawDocument.NotesById.Values.Should().ContainSingle().Subject;

        note.Properties.Should().ContainSingle(p => p.Name == "Fret" && p.Fret == 2);
        note.Properties.Should().ContainSingle(p => p.Name == "String" && p.StringNumber == 3);
        note.Properties.Should().ContainSingle(p => p.Name == "Midi" && p.Number == 40);
    }
}
