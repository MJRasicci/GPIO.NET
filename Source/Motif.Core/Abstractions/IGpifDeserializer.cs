namespace GPIO.NET.Abstractions;

using GPIO.NET.Models.Raw;

/// <summary>
/// Deserializes GPIF XML into a raw object model that preserves source structure.
/// </summary>
public interface IGpifDeserializer
{
    ValueTask<GpifDocument> DeserializeAsync(Stream scoreStream, CancellationToken cancellationToken = default);
}
