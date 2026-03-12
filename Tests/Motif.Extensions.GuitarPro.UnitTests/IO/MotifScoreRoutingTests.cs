namespace Motif.Extensions.GuitarPro.UnitTests;

using FluentAssertions;
using Motif;
using Motif.Extensions.GuitarPro;
using Motif.Extensions.GuitarPro.Abstractions;

public class MotifScoreRoutingTests
{
    [Fact]
    public void MotifScore_discovers_guitar_pro_handlers_when_the_extension_assembly_is_present()
    {
        var formats = MotifScore.GetRegisteredFormats();

        formats.Should().ContainSingle(format => format is GpFormatHandler);
        formats.Should().ContainSingle(format => format is GpifFormatHandler);
        MotifScore.CanOpen("song.gp").Should().BeTrue();
        MotifScore.CanOpen("song.gpif").Should().BeTrue();
        MotifScore.CreateReader("gp").Should().BeAssignableTo<IGuitarProReader>();
        MotifScore.CreateWriter("gp").Should().BeAssignableTo<IGuitarProWriter>();
        MotifScore.CreateWriter("gpif").Should().BeAssignableTo<IGpifWriter>();
    }

    [Fact]
    public async Task MotifScore_can_open_gp_and_save_gpif_via_discovered_handlers()
    {
        var fixturePath = GuitarProFixture.PathFor("test.gp");
        var tempDirectory = CreateTempDirectory();
        var gpifPath = Path.Combine(tempDirectory, "roundtrip.gpif");

        try
        {
            var score = await MotifScore.OpenAsync(fixturePath, TestContext.Current.CancellationToken);
            await MotifScore.SaveAsync(score, gpifPath, TestContext.Current.CancellationToken);

            var roundTripped = await MotifScore.OpenAsync(gpifPath, TestContext.Current.CancellationToken);
            roundTripped.Title.Should().Be(score.Title);
            roundTripped.Tracks.Count.Should().Be(score.Tracks.Count);
            roundTripped.TimelineBars.Count.Should().Be(score.TimelineBars.Count);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task MotifScore_can_save_gp_via_the_discovered_guitar_pro_handler()
    {
        var fixturePath = GuitarProFixture.PathFor("test.gp");
        var tempDirectory = CreateTempDirectory();
        var outputPath = Path.Combine(tempDirectory, "roundtrip.gp");

        try
        {
            var score = await MotifScore.OpenAsync(fixturePath, TestContext.Current.CancellationToken);
            await MotifScore.SaveAsync(score, outputPath, TestContext.Current.CancellationToken);

            var readBack = await new GuitarProReader().ReadAsync(outputPath, TestContext.Current.CancellationToken);
            readBack.Title.Should().Be(score.Title);
            readBack.Tracks.Count.Should().Be(score.Tracks.Count);
            readBack.TimelineBars.Count.Should().Be(score.TimelineBars.Count);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task MotifScore_can_create_a_gpif_writer_with_diagnostics_via_discovered_handlers()
    {
        var fixturePath = GuitarProFixture.PathFor("test.gp");
        var tempDirectory = CreateTempDirectory();
        var outputPath = Path.Combine(tempDirectory, "roundtrip.gpif");

        try
        {
            var score = await MotifScore.OpenAsync(fixturePath, TestContext.Current.CancellationToken);
            var writer = MotifScore.CreateWriter("gpif").Should().BeAssignableTo<IGpifWriter>().Subject;
            var diagnostics = await writer.WriteWithDiagnosticsAsync(score, outputPath, TestContext.Current.CancellationToken);

            File.Exists(outputPath).Should().BeTrue();
            diagnostics.Should().NotBeNull();
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "motif-gp-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
