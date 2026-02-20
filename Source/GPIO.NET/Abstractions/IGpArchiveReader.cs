namespace GPIO.NET.Abstractions;

/// <summary>
/// Responsible for opening a .gp archive and exposing the GPIF score stream.
/// </summary>
public interface IGpArchiveReader
{
    ValueTask<Stream> OpenScoreStreamAsync(string filePath, CancellationToken cancellationToken = default);
}
