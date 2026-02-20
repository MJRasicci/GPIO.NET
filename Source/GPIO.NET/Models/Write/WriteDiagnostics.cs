namespace GPIO.NET.Models.Write;

public sealed class WriteDiagnostics
{
    private readonly List<string> warnings = [];

    public IReadOnlyList<string> Warnings => warnings;

    public void Warn(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            warnings.Add(message);
        }
    }
}
