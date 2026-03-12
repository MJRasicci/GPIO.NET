namespace Motif.Core.UnitTests;

using FluentAssertions;
using Motif;
using Motif.Models;
using System.Text;

public class MotifScoreTests
{
    [Fact]
    public async Task MotifScore_can_round_trip_json_without_any_extension_package()
    {
        var score = CreateScore("Json Native");
        var tempDirectory = CreateTempDirectory();
        var filePath = Path.Combine(tempDirectory, "score.json");

        try
        {
            await MotifScore.SaveAsync(score, filePath, TestContext.Current.CancellationToken);
            var readBack = await MotifScore.OpenAsync(filePath, TestContext.Current.CancellationToken);

            readBack.Title.Should().Be("Json Native");
            readBack.Tracks.Should().ContainSingle();
            ScoreNavigation.HasCurrentPlaybackSequence(readBack).Should().BeTrue();
            MotifScore.CanOpen(filePath).Should().BeTrue();
            MotifScore.GetRegisteredFormats().Should().Contain(handler => handler.SupportedExtensions.Contains(".json"));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task Explicit_handler_registration_routes_path_and_stream_operations()
    {
        var handler = new RecordingFormatHandler();
        using var registration = MotifScore.RegisterHandler(handler);

        var score = CreateScore("Custom Format");
        var tempDirectory = CreateTempDirectory();
        var filePath = Path.Combine(tempDirectory, "score.fake");

        try
        {
            await MotifScore.SaveAsync(score, filePath, TestContext.Current.CancellationToken);
            var pathRead = await MotifScore.OpenAsync(filePath, TestContext.Current.CancellationToken);

            pathRead.Title.Should().Be("Custom Format");
            handler.Writer.PathWrites.Should().Be(1);
            handler.Reader.PathReads.Should().Be(1);
            MotifScore.CanOpen(filePath).Should().BeTrue();

            await using var stream = new MemoryStream();
            await MotifScore.SaveAsync(score, stream, ".fake", TestContext.Current.CancellationToken);
            stream.Position = 0;

            var streamRead = await MotifScore.OpenAsync(stream, "fake", TestContext.Current.CancellationToken);
            streamRead.Title.Should().Be("Custom Format");
            handler.Writer.StreamWrites.Should().Be(1);
            handler.Reader.StreamReads.Should().Be(1);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task Opening_unknown_format_throws_a_helpful_error()
    {
        var action = async () => await MotifScore.OpenAsync("song.xyz", TestContext.Current.CancellationToken);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
        exception.Message.Should().Contain(".xyz");
        exception.Message.Should().Contain("RegisterHandler");
    }

    private static Score CreateScore(string title)
    {
        var score = new Score
        {
            Title = title,
            TimelineBars =
            [
                new TimelineBar
                {
                    Index = 0,
                    TimeSignature = "4/4"
                }
            ],
            Tracks =
            [
                new Track
                {
                    Id = 1,
                    Name = "Lead",
                    Staves =
                    [
                        new Staff
                        {
                            StaffIndex = 0,
                            Measures =
                            [
                                new StaffMeasure
                                {
                                    Index = 0,
                                    StaffIndex = 0,
                                    Voices =
                                    [
                                        new Voice
                                        {
                                            VoiceIndex = 0,
                                            Beats =
                                            [
                                                new Beat
                                                {
                                                    Id = 1,
                                                    Offset = 0m,
                                                    Duration = 0.25m,
                                                    Notes =
                                                    [
                                                        new Note
                                                        {
                                                            Id = 1,
                                                            MidiPitch = 64,
                                                            Duration = 0.25m
                                                        }
                                                    ],
                                                    MidiPitches = [64]
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

        ScoreNavigation.RebuildPlaybackSequence(score);
        return score;
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "motif-core-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private sealed class RecordingFormatHandler : IFormatHandler
    {
        public RecordingFormatHandler()
        {
            Reader = new RecordingReader();
            Writer = new RecordingWriter();
        }

        public RecordingReader Reader { get; }

        public RecordingWriter Writer { get; }

        public IReadOnlyList<string> SupportedExtensions { get; } = [".fake"];

        public string FormatName => "Fake";

        public IScoreReader CreateReader() => Reader;

        public IScoreWriter CreateWriter() => Writer;
    }

    private sealed class RecordingReader : IScoreReader, IPathScoreReader
    {
        public int PathReads { get; private set; }

        public int StreamReads { get; private set; }

        public async ValueTask<Score> ReadAsync(Stream source, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(source);

            StreamReads++;
            using var reader = new StreamReader(source, Encoding.UTF8, leaveOpen: true);
            var title = await reader.ReadToEndAsync(cancellationToken);
            return CreateScore(title);
        }

        public async ValueTask<Score> ReadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            PathReads++;
            var title = await File.ReadAllTextAsync(filePath, cancellationToken);
            return CreateScore(title);
        }
    }

    private sealed class RecordingWriter : IScoreWriter, IPathScoreWriter
    {
        public int PathWrites { get; private set; }

        public int StreamWrites { get; private set; }

        public async ValueTask WriteAsync(Score score, Stream destination, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(score);
            ArgumentNullException.ThrowIfNull(destination);

            StreamWrites++;
            var bytes = Encoding.UTF8.GetBytes(score.Title ?? string.Empty);
            await destination.WriteAsync(bytes, cancellationToken);
            await destination.FlushAsync(cancellationToken);
        }

        public async ValueTask WriteAsync(Score score, string filePath, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(score);

            PathWrites++;
            var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, score.Title ?? string.Empty, cancellationToken);
        }
    }
}
