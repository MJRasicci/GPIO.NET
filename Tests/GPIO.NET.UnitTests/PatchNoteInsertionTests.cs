namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using GPIO.NET.Models.Patching;

public class PatchNoteInsertionTests
{
    [Fact]
    public async Task Patcher_can_add_note_to_existing_beat()
    {
        var sourceFile = Path.Combine(AppContext.BaseDirectory, "Fixtures", "test.gp");
        var reader = new GPIO.NET.GuitarProReader();
        var before = await reader.ReadAsync(sourceFile, cancellationToken: TestContext.Current.CancellationToken);

        var targetBeat = before.Tracks[0].Measures[0].Beats.FirstOrDefault(b => b.Id > 0);
        targetBeat.Should().NotBeNull();
        var beforeCount = targetBeat!.Notes.Count;

        var output = Path.Combine(Path.GetTempPath(), $"gpio-add-note-{Guid.NewGuid():N}.gp");
        try
        {
            var patcher = new GuitarProPatcher();
            var result = await patcher.PatchAsync(
                sourceFile,
                output,
                new GpPatchDocument
                {
                    AddNotesToBeats =
                    [
                        new AddNotesToBeatPatch
                        {
                            BeatId = targetBeat.Id,
                            MidiPitches = [72]
                        }
                    ]
                },
                TestContext.Current.CancellationToken);

            result.Diagnostics.Entries.Any(e => e.Operation == "add-notes-to-beat").Should().BeTrue();

            var after = await reader.ReadAsync(output, cancellationToken: TestContext.Current.CancellationToken);
            var afterBeat = after.Tracks[0].Measures[0].Beats.First(b => b.Id == targetBeat.Id);
            afterBeat.Notes.Count.Should().BeGreaterThan(beforeCount);
        }
        finally
        {
            if (File.Exists(output)) File.Delete(output);
        }
    }
}
