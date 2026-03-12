namespace Motif.Models;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Exposes typed extension storage for domain-model nodes.
/// </summary>
public interface IExtensibleModel
{
    /// <summary>
    /// Attempts to retrieve an attached extension of the requested type.
    /// </summary>
    /// <typeparam name="TExtension">The extension type to retrieve.</typeparam>
    /// <param name="extension">Receives the attached extension when present.</param>
    /// <returns><see langword="true"/> when an extension of <typeparamref name="TExtension"/> is attached.</returns>
    bool TryGetExtension<TExtension>([NotNullWhen(true)] out TExtension? extension)
        where TExtension : class, IModelExtension;

    /// <summary>
    /// Returns the extensions currently attached to the model node.
    /// </summary>
    /// <returns>The attached extensions. Returns an empty collection when none are attached.</returns>
    IReadOnlyCollection<IModelExtension> GetExtensions();

    /// <summary>
    /// Attaches or replaces an extension of the given type.
    /// </summary>
    /// <typeparam name="TExtension">The extension type to attach.</typeparam>
    /// <param name="extension">The extension instance to store.</param>
    /// <exception cref="ArgumentNullException"><paramref name="extension"/> is <see langword="null"/>.</exception>
    void SetExtension<TExtension>(TExtension extension)
        where TExtension : class, IModelExtension;

    /// <summary>
    /// Removes an attached extension of the given type.
    /// </summary>
    /// <typeparam name="TExtension">The extension type to remove.</typeparam>
    /// <returns><see langword="true"/> when an attached extension was removed.</returns>
    bool RemoveExtension<TExtension>()
        where TExtension : class, IModelExtension;
}
