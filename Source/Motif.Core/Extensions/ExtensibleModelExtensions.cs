namespace Motif.Models;

public static class ExtensibleModelExtensions
{
    public static TExtension? GetExtension<TExtension>(this IExtensibleModel model)
        where TExtension : class, IModelExtension
    {
        ArgumentNullException.ThrowIfNull(model);

        return model.TryGetExtension<TExtension>(out var extension)
            ? extension
            : null;
    }

    public static TExtension GetRequiredExtension<TExtension>(this IExtensibleModel model)
        where TExtension : class, IModelExtension
    {
        ArgumentNullException.ThrowIfNull(model);

        return model.TryGetExtension<TExtension>(out var extension)
            ? extension
            : throw new InvalidOperationException($"Extension '{typeof(TExtension).FullName}' is not attached to '{model.GetType().FullName}'.");
    }
}
