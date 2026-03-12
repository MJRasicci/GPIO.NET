# Motif

Motif is a .NET 10 music score library built around a mutable, format-agnostic `Score`
domain model. The current production extension is Guitar Pro support via
`Motif.Extensions.GuitarPro`, plus a companion CLI for inspection, conversion, and
round-trip diagnostics.

## Current Scope

- Read Guitar Pro `.gp` archives and extract `Content/score.gpif`
- Deserialize GPIF XML into a typed raw model
- Resolve GPIF references into a clean domain graph:
  `Score -> Tracks -> Staves -> StaffMeasures -> Voices -> Beats -> Notes`
- Preserve score-wide timeline and navigation state on `Score.TimelineBars`
- Rebuild derived playback order with `ScoreNavigation`
- Write edited scores back to `.gpif` or `.gp`
- Convert between `.gp`, `.gpif`, and mapped JSON with `motif-cli`

## Projects

| Project | Purpose |
| --- | --- |
| `Motif.Core` | Format-agnostic domain model, navigation helpers, and serialization helpers |
| `Motif.Extensions.GuitarPro` | Guitar Pro `.gp` / `.gpif` read-write support |
| `Motif` | Convenience package referencing Core and Guitar Pro support |
| `Motif.CLI` (`motif-cli`) | CLI for conversion, inspection, and batch diagnostics |

## Repository Layout

```text
Source/
  Motif/                             Convenience package
  Motif.Core/                        Core domain model and navigation helpers
  Motif.Extensions.GuitarPro/        Guitar Pro reader/writer, mapper, raw GPIF model
  Motif.CLI/                         CLI executable project
Tests/
  Motif.Core.UnitTests/              Core model, navigation, and serialization tests
  Motif.Extensions.GuitarPro.UnitTests/
                                     Guitar Pro mapping, writing, and round-trip tests
  Motif.IntegrationTests/            CLI integration and regression coverage
docs/
  CLI_WORKFLOW.md                    CLI usage and batch workflows
  LIBRARY_WORKFLOW.md                Recommended library edit/write workflow
```

## Library Quick Start

```csharp
using Motif;
using Motif.Extensions.GuitarPro;

var reader = new GuitarProReader();
var writer = new GuitarProWriter();

var score = await reader.ReadAsync("song.gp", cancellationToken: cancellationToken);

score.Title = "Edited Title";

// Rebuild derived playback state after navigation-affecting edits.
ScoreNavigation.RebuildPlaybackSequence(score);

var diagnostics = await writer.WriteWithDiagnosticsAsync(
    score,
    "song-edited.gp",
    cancellationToken);
```

`GuitarProWriter` always writes a valid `.gp` archive. If the destination path already
exists and is a valid archive, non-score ZIP entries are preserved and only
`Content/score.gpif` is replaced. If the destination does not exist, the writer uses the
embedded default archive template. For a new output path seeded from a different source
archive, use the CLI `--source-gp` workflow.

## CLI Quick Start

Run during development:

```bash
dotnet run --project Source/Motif.CLI -- <args>
```

Common commands:

```bash
# Export mapped JSON (default output: song.mapped.json)
dotnet run --project Source/Motif.CLI -- song.gp

# Extract raw GPIF
dotnet run --project Source/Motif.CLI -- song.gp song.score.gpif

# Convert raw GPIF to mapped JSON
dotnet run --project Source/Motif.CLI -- song.gpif song.json

# Write a new .gp archive from mapped JSON
dotnet run --project Source/Motif.CLI -- song.json output.gp

# Preserve non-score archive entries from an existing source archive
dotnet run --project Source/Motif.CLI -- song.json output.gp --source-gp original.gp

# Batch export every .gp file under a directory to JSON
dotnet run --project Source/Motif.CLI -- \
  --batch-input-dir ./songs \
  --batch-output-dir ./json

# Batch round-trip diagnostics across a corpus
dotnet run --project Source/Motif.CLI -- \
  --batch-input-dir ./songs \
  --batch-output-dir ./analysis \
  --batch-roundtrip-diagnostics
```

Formats are inferred from file extensions when possible. Use `--input-format` and
`--output-format` when extensions are missing or ambiguous. Boolean flags follow the same
pattern everywhere: `--flag`, `--flag=true`, and `--flag=false`.

## Supported Formats

| Format | Read | Write | Notes |
| --- | --- | --- | --- |
| `gp` | Yes | Yes | Guitar Pro ZIP archive containing `Content/score.gpif` |
| `gpif` | Yes | Yes | Raw GPIF XML |
| `json` | Yes | Yes | Mapped `Score` JSON, intended for editing and inspection |
| `musicxml` / `mxl` | No | No | Not part of the current CLI or library surface |
| `midi` | No | No | Not part of the current CLI or library surface |

The CLI intentionally rejects unsupported formats rather than silently routing them.

## Testing

Run the full test suite with:

```bash
dotnet test
```

Coverage includes real `.gp` fixtures, mapping fidelity, write diagnostics, public API
shape, and CLI regression tests.

## Documentation

- [docs/CLI_WORKFLOW.md](docs/CLI_WORKFLOW.md)
- [docs/LIBRARY_WORKFLOW.md](docs/LIBRARY_WORKFLOW.md)
- [AGENTS.md](AGENTS.md)

## License

[MIT](LICENSE.md)
