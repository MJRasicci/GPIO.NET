namespace GPIO.NET.UnitTests;

using FluentAssertions;
using GPIO.NET.Implementation;
using GPIO.NET.Models.Patching;

public class PatchNoteReorderTests
{
    [Fact]
    public async Task Patcher_can_reorder_notes_within_existing_beat()
    {
        var sourceFile = Path.Combine(AppContext.BaseDirectory, "Fixtures", "schema-reference.gp");
        var output = Path.Combine(Path.GetTempPath(), $"gpio-reorder-{Guid.NewGuid():N}.gp");

        try
        {
            var reader = new GPIO.NET.GuitarProReader();
            var before = await reader.ReadAsync(sourceFile, cancellationToken: TestContext.Current.CancellationToken);

            var beat = before.Tracks.SelectMany(t => t.Measures).SelectMany(m => m.Beats).FirstOrDefault(b => b.Id > 0 && b.Notes.Count > 1);
            beat.Should().NotBeNull();

            var reversed = beat!.Notes.Select(n => n.Id).Reverse().ToArray();

            var patcher = new GuitarProPatcher();
            var result = await patcher.PatchAsync(
                sourceFile,
                output,
                new GpPatchDocument
                {
                    ReorderBeatNotes = [ new ReorderBeatNotesPatch { BeatId = beat.Id, NoteIds = reversed } ]
                },
                TestContext.Current.CancellationToken);

            result.Diagnostics.Entries.Any(e => e.Operation == "reorder-beat-notes").Should().BeTrue();

            var after = await reader.ReadAsync(output, cancellationToken: TestContext.Current.CancellationToken);
            var beatAfter = after.Tracks.SelectMany(t => t.Measures).SelectMany(m => m.Beats).First(b => b.Id == beat.Id);
            beatAfter.Notes.Select(n => n.Id).Should().Equal(reversed);
        }
        finally
        {
            if (File.Exists(output)) File.Delete(output);
        }
    }
}
