namespace GPIO.NET;

using GPIO.NET.Models;
using System.Text.Json;

public static class GuitarProScoreJson
{
    public static string ToJson(this GuitarProScore score, bool indented = true)
    {
        ArgumentNullException.ThrowIfNull(score);

        var options = new JsonSerializerOptions
        {
            WriteIndented = indented
        };

        return JsonSerializer.Serialize(score, options);
    }
}
