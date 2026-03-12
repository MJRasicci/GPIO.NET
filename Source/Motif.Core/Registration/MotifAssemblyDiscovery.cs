namespace Motif;

using System.Reflection;

internal static class MotifAssemblyDiscovery
{
    public static IReadOnlyCollection<Assembly> EnumerateCandidateAssemblies()
    {
        var assembliesByName = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            TryAddAssembly(assembliesByName, assembly);
        }

        var baseDirectory = AppContext.BaseDirectory;
        if (!Directory.Exists(baseDirectory))
        {
            return assembliesByName.Values;
        }

        foreach (var path in Directory.EnumerateFiles(baseDirectory, "Motif*.dll", SearchOption.TopDirectoryOnly)
                     .OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            AssemblyName assemblyName;
            try
            {
                assemblyName = AssemblyName.GetAssemblyName(path);
            }
            catch (BadImageFormatException)
            {
                continue;
            }
            catch (FileLoadException)
            {
                continue;
            }

            if (assembliesByName.Values.Any(loaded => AssemblyName.ReferenceMatchesDefinition(loaded.GetName(), assemblyName)))
            {
                continue;
            }

            try
            {
                TryAddAssembly(assembliesByName, Assembly.Load(assemblyName));
            }
            catch
            {
                // Ignore optional Motif companion assemblies that are not loadable in the current app.
            }
        }

        return assembliesByName.Values;
    }

    private static void TryAddAssembly(Dictionary<string, Assembly> assembliesByName, Assembly assembly)
    {
        var identity = assembly.FullName;
        if (string.IsNullOrWhiteSpace(identity))
        {
            identity = assembly.GetName().Name ?? assembly.Location;
        }

        if (!string.IsNullOrWhiteSpace(identity) && !assembliesByName.ContainsKey(identity))
        {
            assembliesByName[identity] = assembly;
        }
    }
}
