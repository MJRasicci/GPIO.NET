namespace GPIO.NET.Abstractions;

using GPIO.NET.Models;
using GPIO.NET.Models.Raw;

/// <summary>
/// Maps raw GPIF structures into the public, traversal-friendly domain model.
/// </summary>
public interface IScoreMapper
{
    ValueTask<GuitarProScore> MapAsync(GpifDocument source, CancellationToken cancellationToken = default);
}
