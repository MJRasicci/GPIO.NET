namespace GPIO.NET.UnitTests;

using FluentAssertions;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;

public class CliPatchPlannerRegressionTests
{
    [Fact]
    public async Task No_edit_patch_from_json_is_zero_op_for_simple_score_fixture()
    {
        var repoRoot = FindRepositoryRoot();
        var sourceGp = Path.Combine(repoRoot, "Temp", "SimpleScore.gp");
        File.Exists(sourceGp).Should().BeTrue();

        var toolProject = Path.Combine(repoRoot, "Source", "GPIO.NET.Tool", "GPIO.NET.Tool.csproj");
        Directory.Exists(Path.GetDirectoryName(toolProject)!).Should().BeTrue();

        var tempDir = Path.Combine(Path.GetTempPath(), $"gpio-cli-regression-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        var jsonPath = Path.Combine(tempDir, "SimpleScore.json");
        var planPath = Path.Combine(tempDir, "SimpleScore.plan.json");
        var outputGp = Path.Combine(tempDir, "SimpleScore.patched.gp");

        try
        {
            await RunDotNetAsync(
                $"run --project \"{toolProject}\" -- \"{sourceGp}\" \"{jsonPath}\" --format json",
                repoRoot);

            await RunDotNetAsync(
                $"run --project \"{toolProject}\" -- \"{jsonPath}\" \"{planPath}\" --from-json --patch-from-json --source-gp \"{sourceGp}\" --format json --plan-only",
                repoRoot);

            await RunDotNetAsync(
                $"run --project \"{toolProject}\" -- \"{jsonPath}\" \"{outputGp}\" --from-json --patch-from-json --source-gp \"{sourceGp}\" --format json",
                repoRoot);

            using var planDocument = JsonDocument.Parse(await File.ReadAllTextAsync(planPath, TestContext.Current.CancellationToken));
            var patchElement = planDocument.RootElement.GetProperty("Patch");
            var operationCount = patchElement.EnumerateObject().Sum(property => property.Value.GetArrayLength());
            var unsupportedCount = planDocument.RootElement.GetProperty("UnsupportedChanges").GetArrayLength();

            operationCount.Should().Be(0);
            unsupportedCount.Should().Be(0);

            var sourceBytes = await ReadScoreGpifBytesAsync(sourceGp, TestContext.Current.CancellationToken);
            var outputBytes = await ReadScoreGpifBytesAsync(outputGp, TestContext.Current.CancellationToken);
            outputBytes.Should().Equal(sourceBytes);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

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

    private static async Task RunDotNetAsync(string arguments, string workingDirectory)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        process.Start();
        var stdout = await process.StandardOutput.ReadToEndAsync(TestContext.Current.CancellationToken);
        var stderr = await process.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
        await process.WaitForExitAsync(TestContext.Current.CancellationToken);

        if (process.ExitCode != 0)
        {
            throw new Xunit.Sdk.XunitException(
                $"dotnet {arguments} failed with exit code {process.ExitCode}{Environment.NewLine}stdout:{Environment.NewLine}{stdout}{Environment.NewLine}stderr:{Environment.NewLine}{stderr}");
        }
    }

    private static async Task<byte[]> ReadScoreGpifBytesAsync(string gpPath, CancellationToken cancellationToken)
    {
        using var archive = ZipFile.OpenRead(gpPath);
        var entry = archive.GetEntry("Content/score.gpif");
        entry.Should().NotBeNull();

        await using var stream = entry!.Open();
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, cancellationToken);
        return buffer.ToArray();
    }
}
