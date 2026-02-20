namespace GPIO.NET.Abstractions;

using GPIO.NET.Models.Patching;

public interface IGuitarProPatcher
{
    ValueTask PatchAsync(string sourceGpPath, string outputGpPath, GpPatchDocument patch, CancellationToken cancellationToken = default);
}
