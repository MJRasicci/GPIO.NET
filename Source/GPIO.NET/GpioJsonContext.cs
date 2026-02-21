namespace GPIO.NET;

using GPIO.NET.Models;
using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(GuitarProScore))]
internal partial class GpioJsonContext : JsonSerializerContext
{
}
