namespace Motif.Extensions.GuitarPro.Models;

using Motif.Models;

internal sealed class GpFidelityStateExtension : IModelExtension
{
    public bool HasSourceContext { get; set; }

    public bool FidelityInvalidated { get; set; }

    public GpExtensionReattachmentResult? LastReattachment { get; set; }
}
