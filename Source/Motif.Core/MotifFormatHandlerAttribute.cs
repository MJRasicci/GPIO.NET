namespace Motif;

/// <summary>
/// Declares a score format handler that Motif can discover from an assembly at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class MotifFormatHandlerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new attribute that points at a concrete <see cref="IFormatHandler"/> type.
    /// </summary>
    /// <param name="handlerType">The handler type to instantiate when the assembly is discovered.</param>
    /// <exception cref="ArgumentNullException"><paramref name="handlerType"/> is <see langword="null"/>.</exception>
    public MotifFormatHandlerAttribute(Type handlerType)
    {
        HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
    }

    /// <summary>
    /// Gets the concrete handler type declared by the assembly.
    /// </summary>
    public Type HandlerType { get; }
}
