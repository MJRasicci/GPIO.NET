namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using GPIO.NET.Models.Patching;

public class GuitarProPatcherTests
{
    [Fact]
    public async Task Patcher_appends_note_to_existing_voice_without_full_rewrite_model_loss()
    {
        var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample.gp");
        File.Exists(source).Should().BeTrue();

        var output = Path.Combine(Path.GetTempPath(), $"gpio-patched-{Guid.NewGuid():N}.gp");

        try
        {
            var reader = new GPIO.NET.GuitarProReader();
            var before = await reader.ReadAsync(source, cancellationToken: TestContext.Current.CancellationToken);

            var patcher = new GuitarProPatcher();
            await patcher.PatchAsync(
                source,
                output,
                new GpPatchDocument
                {
                    AppendNotes =
                    [
                        new AppendNotesPatch
                        {
                            TrackId = before.Tracks[0].Id,
                            MasterBarIndex = 0,
                            VoiceIndex = 0,
                            RhythmNoteValue = "Quarter",
                            MidiPitches = [64]
                        }
                    ]
                },
                TestContext.Current.CancellationToken);

            File.Exists(output).Should().BeTrue();

            var after = await reader.ReadAsync(output, cancellationToken: TestContext.Current.CancellationToken);
            after.Tracks.Count.Should().Be(before.Tracks.Count);

            var beforeNotes = before.Tracks[0].Measures[0].Beats.SelectMany(b => b.Notes).Count();
            var afterNotes = after.Tracks[0].Measures[0].Beats.SelectMany(b => b.Notes).Count();
            afterNotes.Should().BeGreaterThan(beforeNotes);
        }
        finally
        {
            if (File.Exists(output))
            {
                File.Delete(output);
            }
        }
    }
}
