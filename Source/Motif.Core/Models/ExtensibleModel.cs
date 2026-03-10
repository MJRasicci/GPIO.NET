namespace Motif.Models;

using System.Diagnostics.CodeAnalysis;

public abstract class ExtensibleModel : IExtensibleModel
{
    private static readonly IReadOnlyCollection<IModelExtension> EmptyExtensions = Array.Empty<IModelExtension>();
    private Dictionary<Type, IModelExtension>? extensions;

    public bool TryGetExtension<TExtension>([NotNullWhen(true)] out TExtension? extension)
        where TExtension : class, IModelExtension
    {
        if (extensions is not null && extensions.TryGetValue(typeof(TExtension), out var candidate))
        {
            extension = (TExtension)candidate;
            return true;
        }

        extension = default;
        return false;
    }

    public IReadOnlyCollection<IModelExtension> GetExtensions()
        => extensions is { Count: > 0 }
            ? extensions.Values.ToArray()
            : EmptyExtensions;

    public void SetExtension<TExtension>(TExtension extension)
        where TExtension : class, IModelExtension
    {
        ArgumentNullException.ThrowIfNull(extension);

        extensions ??= [];
        extensions[typeof(TExtension)] = extension;
    }

    public bool RemoveExtension<TExtension>()
        where TExtension : class, IModelExtension
        => extensions is not null && extensions.Remove(typeof(TExtension));
}
