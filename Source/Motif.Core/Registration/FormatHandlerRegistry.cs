namespace Motif;

using System.Reflection;
using System.Text.Json;
using Motif.Models;

internal static class FormatHandlerRegistry
{
    private static readonly object SyncRoot = new();
    private static readonly List<ExplicitRegistration> ExplicitRegistrations = [];
    private static readonly IFormatHandler JsonHandler = new JsonFormatHandler();

    public static IDisposable Register(IFormatHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ValidateHandler(handler);

        var registration = new ExplicitRegistration(handler);
        lock (SyncRoot)
        {
            ExplicitRegistrations.Add(registration);
        }

        return new RegistrationHandle(registration);
    }

    public static IReadOnlyList<IFormatHandler> GetRegisteredHandlers()
    {
        var handlers = new List<IFormatHandler> { JsonHandler };

        lock (SyncRoot)
        {
            for (var i = ExplicitRegistrations.Count - 1; i >= 0; i--)
            {
                handlers.Add(ExplicitRegistrations[i].Handler);
            }
        }

        handlers.AddRange(DiscoverHandlers());
        return DeduplicateByType(handlers);
    }

    public static bool TryResolve(string formatHint, out IFormatHandler? handler)
    {
        var normalizedHint = NormalizeFormatHint(formatHint);
        if (string.Equals(normalizedHint, ".json", StringComparison.OrdinalIgnoreCase))
        {
            handler = JsonHandler;
            return true;
        }

        lock (SyncRoot)
        {
            for (var i = ExplicitRegistrations.Count - 1; i >= 0; i--)
            {
                var candidate = ExplicitRegistrations[i].Handler;
                if (SupportsExtension(candidate, normalizedHint))
                {
                    handler = candidate;
                    return true;
                }
            }
        }

        foreach (var candidate in DiscoverHandlers())
        {
            if (SupportsExtension(candidate, normalizedHint))
            {
                handler = candidate;
                return true;
            }
        }

        handler = null;
        return false;
    }

    public static string GetFormatHintFromPath(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var extension = Path.GetExtension(filePath);
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentException(
                $"Unable to determine the score format from '{filePath}'. Provide a file extension or use the stream overload with an explicit format hint.",
                nameof(filePath));
        }

        return NormalizeFormatHint(extension);
    }

    public static string NormalizeFormatHint(string formatHint)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(formatHint);

        var trimmed = formatHint.Trim();
        var extension = Path.GetExtension(trimmed);
        if (!string.IsNullOrWhiteSpace(extension)
            && (trimmed.Contains(Path.DirectorySeparatorChar)
                || trimmed.Contains(Path.AltDirectorySeparatorChar)
                || !trimmed.StartsWith(".", StringComparison.Ordinal)))
        {
            trimmed = extension;
        }

        if (!trimmed.StartsWith(".", StringComparison.Ordinal))
        {
            trimmed = "." + trimmed;
        }

        return trimmed.ToLowerInvariant();
    }

    private static IReadOnlyList<IFormatHandler> DiscoverHandlers()
    {
        var handlers = new List<IFormatHandler>();
        var seenHandlerTypes = new HashSet<Type>();

        foreach (var assembly in EnumerateCandidateAssemblies().OrderBy(a => a.FullName, StringComparer.Ordinal))
        {
            foreach (var attribute in assembly.GetCustomAttributes<MotifFormatHandlerAttribute>())
            {
                var handlerType = attribute.HandlerType;
                if (!seenHandlerTypes.Add(handlerType))
                {
                    continue;
                }

                if (!typeof(IFormatHandler).IsAssignableFrom(handlerType) || handlerType.IsAbstract)
                {
                    throw new InvalidOperationException(
                        $"Assembly '{assembly.GetName().Name}' declared format handler '{handlerType.FullName}', but it does not implement {nameof(IFormatHandler)} as a concrete type.");
                }

                if (Activator.CreateInstance(handlerType) is not IFormatHandler handler)
                {
                    throw new InvalidOperationException(
                        $"Assembly '{assembly.GetName().Name}' declared format handler '{handlerType.FullName}', but Motif could not create an instance.");
                }

                ValidateHandler(handler);
                handlers.Add(handler);
            }
        }

        return handlers
            .OrderBy(handler => handler.FormatName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(handler => handler.SupportedExtensions.FirstOrDefault() ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<IFormatHandler> DeduplicateByType(IEnumerable<IFormatHandler> handlers)
    {
        var unique = new List<IFormatHandler>();
        var seenTypes = new HashSet<Type>();

        foreach (var handler in handlers)
        {
            if (seenTypes.Add(handler.GetType()))
            {
                unique.Add(handler);
            }
        }

        return unique;
    }

    private static IEnumerable<Assembly> EnumerateCandidateAssemblies()
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

    private static void TryAddAssembly(IDictionary<string, Assembly> assembliesByName, Assembly assembly)
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

    private static bool SupportsExtension(IFormatHandler handler, string normalizedHint)
        => handler.SupportedExtensions.Any(extension =>
            string.Equals(NormalizeFormatHint(extension), normalizedHint, StringComparison.OrdinalIgnoreCase));

    private static void ValidateHandler(IFormatHandler handler)
    {
        if (string.IsNullOrWhiteSpace(handler.FormatName))
        {
            throw new InvalidOperationException($"Format handler '{handler.GetType().FullName}' must provide a non-empty {nameof(IFormatHandler.FormatName)}.");
        }

        if (handler.SupportedExtensions.Count == 0)
        {
            throw new InvalidOperationException($"Format handler '{handler.GetType().FullName}' must declare at least one supported extension.");
        }

        var normalizedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var extension in handler.SupportedExtensions)
        {
            var normalizedExtension = NormalizeFormatHint(extension);
            if (string.Equals(normalizedExtension, ".json", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalizedExtension, ".motif", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Format handler '{handler.GetType().FullName}' cannot register native Motif extension '{normalizedExtension}'.");
            }

            if (!normalizedExtensions.Add(normalizedExtension))
            {
                throw new InvalidOperationException(
                    $"Format handler '{handler.GetType().FullName}' declared duplicate extension '{normalizedExtension}'.");
            }
        }
    }

    private sealed class ExplicitRegistration(IFormatHandler handler)
    {
        public IFormatHandler Handler { get; } = handler;
    }

    private sealed class RegistrationHandle(ExplicitRegistration registration) : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            lock (SyncRoot)
            {
                ExplicitRegistrations.Remove(registration);
            }

            disposed = true;
        }
    }

    private sealed class JsonFormatHandler : IFormatHandler
    {
        public IReadOnlyList<string> SupportedExtensions { get; } = [".json"];

        public string FormatName => "Motif JSON";

        public IScoreReader CreateReader() => new JsonScoreReader();

        public IScoreWriter CreateWriter() => new JsonScoreWriter();
    }

    private sealed class JsonScoreReader : IScoreReader
    {
        public async ValueTask<Score> ReadAsync(Stream source, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(source);

            var score = await JsonSerializer.DeserializeAsync(source, MotifJsonContext.Default.Score, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidDataException("Unable to deserialize mapped score JSON.");

            ScoreNavigation.EnsurePlaybackSequence(score);
            return score;
        }

        public async ValueTask<Score> ReadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            await using var source = File.OpenRead(filePath);
            return await ReadAsync(source, cancellationToken).ConfigureAwait(false);
        }
    }

    private sealed class JsonScoreWriter : IScoreWriter
    {
        public async ValueTask WriteAsync(Score score, Stream destination, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(score);
            ArgumentNullException.ThrowIfNull(destination);

            await JsonSerializer.SerializeAsync(destination, score, MotifJsonContext.Default.Score, cancellationToken).ConfigureAwait(false);
            await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask WriteAsync(Score score, string filePath, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(score);
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var destination = File.Create(filePath);
            await WriteAsync(score, destination, cancellationToken).ConfigureAwait(false);
        }
    }
}
