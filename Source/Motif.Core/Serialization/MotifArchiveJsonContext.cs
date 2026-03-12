namespace Motif;

using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true)]
[JsonSerializable(typeof(MotifArchiveManifest))]
[JsonSerializable(typeof(MotifArchiveSource))]
internal sealed partial class MotifArchiveJsonContext : JsonSerializerContext
{
}
