namespace Motif;

/// <summary>
/// Declares an archive contributor that Motif can discover from an assembly at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class MotifArchiveContributorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new attribute that points at a concrete <see cref="IArchiveContributor"/> type.
    /// </summary>
    /// <param name="contributorType">The contributor type to instantiate when the assembly is discovered.</param>
    /// <exception cref="ArgumentNullException"><paramref name="contributorType"/> is <see langword="null"/>.</exception>
    public MotifArchiveContributorAttribute(Type contributorType)
    {
        ContributorType = contributorType ?? throw new ArgumentNullException(nameof(contributorType));
    }

    /// <summary>
    /// Gets the concrete contributor type declared by the assembly.
    /// </summary>
    public Type ContributorType { get; }
}
