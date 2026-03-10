namespace Motif.Models;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface identifies typed model extensions attached to domain nodes.")]
public interface IModelExtension
{
}
