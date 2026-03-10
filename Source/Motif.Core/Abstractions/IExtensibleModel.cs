namespace Motif.Models;

using System.Diagnostics.CodeAnalysis;

public interface IExtensibleModel
{
    bool TryGetExtension<TExtension>([NotNullWhen(true)] out TExtension? extension)
        where TExtension : class, IModelExtension;

    IReadOnlyCollection<IModelExtension> GetExtensions();

    void SetExtension<TExtension>(TExtension extension)
        where TExtension : class, IModelExtension;

    bool RemoveExtension<TExtension>()
        where TExtension : class, IModelExtension;
}
