namespace GPIO.NET.Abstractions;

using GPIO.NET.Models;
using GPIO.NET.Models.Write;

public interface IScoreUnmapper
{
    ValueTask<WriteResult> UnmapAsync(GuitarProScore score, CancellationToken cancellationToken = default);
}
