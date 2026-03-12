namespace Motif.Models;

/// <summary>
/// Convenience helpers for retrieving typed model extensions.
/// </summary>
public static class ExtensibleModelExtensions
{
    /// <summary>
    /// Returns an attached extension when present.
    /// </summary>
    /// <typeparam name="TExtension">The extension type to retrieve.</typeparam>
    /// <param name="model">The model node to inspect.</param>
    /// <returns>The attached extension, or <see langword="null"/> when the extension is absent.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
    public static TExtension? GetExtension<TExtension>(this IExtensibleModel model)
        where TExtension : class, IModelExtension
    {
        ArgumentNullException.ThrowIfNull(model);

        return model.TryGetExtension<TExtension>(out var extension)
            ? extension
            : null;
    }

    /// <summary>
    /// Returns an attached extension and throws when the extension is absent.
    /// </summary>
    /// <typeparam name="TExtension">The extension type to retrieve.</typeparam>
    /// <param name="model">The model node to inspect.</param>
    /// <returns>The attached extension.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// No extension of type <typeparamref name="TExtension"/> is attached to <paramref name="model"/>.
    /// </exception>
    public static TExtension GetRequiredExtension<TExtension>(this IExtensibleModel model)
        where TExtension : class, IModelExtension
    {
        ArgumentNullException.ThrowIfNull(model);

        return model.TryGetExtension<TExtension>(out var extension)
            ? extension
            : throw new InvalidOperationException($"Extension '{typeof(TExtension).FullName}' is not attached to '{model.GetType().FullName}'.");
    }
}
