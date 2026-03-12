namespace Motif.Models;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Marker interface for metadata attached to an <see cref="IExtensibleModel"/> node.
/// </summary>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface identifies typed model extensions attached to domain nodes.")]
public interface IModelExtension
{
}
