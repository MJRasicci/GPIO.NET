namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;

public class PatchDeletionTests
{
    [Fact]
    public async Task Patcher_delete_note_and_beat_apply_successfully()
    {
        var sourceFile = Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample.gp");
        var reader = new GPIO.NET.GuitarProReader();
        var before = await reader.ReadAsync(sourceFile, cancellationToken: TestContext.Current.CancellationToken);

        var firstMeasure = before.Tracks[0].Measures[0];
        var targetBeat = firstMeasure.Beats.FirstOrDefault(b => b.Id > 0 && b.Notes.Count > 0);
        targetBeat.Should().NotBeNull();
        var targetNoteId = targetBeat!.Notes[0].Id;

        var output = Path.Combine(Path.GetTempPath(), $"gpio-del-{Guid.NewGuid():N}.gp");
        try
        {
            var patcher = new GuitarProPatcher();
            var result = await patcher.PatchAsync(
                sourceFile,
                output,
                new GPIO.NET.Models.Patching.GpPatchDocument
                {
                    DeleteNotes = [ new GPIO.NET.Models.Patching.DeleteNotePatch { NoteId = targetNoteId } ],
                    DeleteBeats = [ new GPIO.NET.Models.Patching.DeleteBeatPatch { BeatId = targetBeat.Id } ]
                },
                TestContext.Current.CancellationToken);

            result.Diagnostics.Entries.Any(e => e.Operation == "delete-note").Should().BeTrue();
            result.Diagnostics.Entries.Any(e => e.Operation == "delete-beat").Should().BeTrue();

            var after = await reader.ReadAsync(output, cancellationToken: TestContext.Current.CancellationToken);
            var afterBeatIds = after.Tracks[0].Measures[0].Beats.Select(b => b.Id).ToHashSet();
            afterBeatIds.Should().NotContain(targetBeat.Id);
        }
        finally
        {
            if (File.Exists(output)) File.Delete(output);
        }
    }
}
