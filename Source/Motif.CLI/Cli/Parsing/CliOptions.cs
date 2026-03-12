namespace Motif.CLI;

internal sealed class CliOptions
{
    public string InputPath { get; init; } = string.Empty;

    public string? OutputPath { get; init; }

    public CliFormat InputFormat { get; init; } = CliFormat.GuitarPro;

    public CliFormat OutputFormat { get; init; } = CliFormat.Json;

    public bool JsonIndented { get; init; } = true;

    public bool JsonIgnoreNull { get; init; }

    public bool JsonIgnoreDefaults { get; init; }

    public string? SourceScorePath { get; init; }

    public string? SourceGpPath { get; init; }

    public string? DiagnosticsOutPath { get; init; }

    public bool DiagnosticsAsJson { get; init; }

    public string? BatchInputDir { get; init; }

    public string? BatchOutputDir { get; init; }

    public bool BatchRoundTripDiagnostics { get; init; }

    public bool ContinueOnError { get; init; } = true;

    public string? FailureLogPath { get; init; }
}
