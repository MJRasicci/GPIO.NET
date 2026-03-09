namespace GPIO.NET.Models.Write;

using GPIO.NET.Models.Raw;

public sealed class WriteResult
{
    public required GpifDocument RawDocument { get; init; }

    public required WriteDiagnostics Diagnostics { get; init; }
}
