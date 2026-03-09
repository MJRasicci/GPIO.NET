# AGENTS.md — Motif Guidance

This is the canonical production repository for the Motif library and companion CLI.

---

## What This Repo Does

Motif reads and writes Guitar Pro (`.gp`) files — ZIP archives containing a GPIF XML document
with a heavily reference-based structure. The library:

1. Opens `.gp` archives and extracts `Content/score.gpif`
2. Deserializes GPIF XML into a typed raw model
3. Resolves all cross-references into a clean, navigable domain model
4. Writes `.gp` archives from an edited domain model

---

## Repository Structure

```
Source/
  Motif/                                   Package wrapping Motif.Core and Motif.Extensions.GuitarPro
  Motif.Core/                              Core library (no dependencies beyond .NET 10)
  Motif.Extensions.GuitarPro/              Guitar Pro file format support (depends on Motif.Core)
  Motif.CLI/                               CLI tool (published as motif-cli executable)
Tests/
  Motif.Core.UnitTests/                    Fixture-based unit + integration tests
  Motif.Extensions.GuitarPro.UnitTests/    Fixture-based unit + integration tests
docs/
  CLI_WORKFLOW.md                          Common CLI workflows
  v1_Todo.md                               Checklist for v1 release, use this to track your work and decide what to do next
```

---

## Architecture: The Processing Pipeline

Every read flows through three explicit layers:

```
.gp file
  └─► IGpArchiveReader      → opens ZIP, exposes score.gpif stream
  └─► IGpifDeserializer     → deserializes XML into GpifDocument (raw model)
  └─► IScoreMapper          → resolves references → GuitarProScore (domain model)
```

Every write flows in reverse:

```
GuitarProScore (edited)
  └─► IScoreUnmapper        → produces GpifDocument from domain model
  └─► IGpifSerializer       → serializes to XML
  └─► IGpArchiveWriter      → writes ZIP archive
```

### Key entry points

| Class | Purpose |
|---|---|
| `GuitarProReader` | Top-level read API |
| `GuitarProWriter` | Top-level write API |
| `DefaultScoreMapper` | Raw GPIF → domain model |
| `DefaultScoreUnmapper` | Domain model → raw GPIF |
| `XmlGpifDeserializer` | XML → `GpifDocument` |
| `XmlGpifSerializer` | `GpifDocument` → XML |

---

## Models

### Raw model (`Models/Raw/`)
Faithful XML representation. Maps 1:1 to GPIF elements.
Hybrid fields use typed core + raw XML passthrough for elements not yet fully normalized.

### Domain model (`Models/`)
`GuitarProScore` — clean, navigable object graph:
```
Score → Tracks → Measures → Voices → Beats → Notes
```
Consumers should never need to deal with GPIF reference mechanics.

---

## CLI Tool

The tool is published as a single-file self-contained executable named `motif-cli`.

**Run during development:**
```
dotnet run --project Source/Motif.CLI -- <args>
```

For full CLI usage, use `--help` or see [docs/CLI_WORKFLOW.md](docs/CLI_WORKFLOW.md).

**Supported output formats:** `json`, `gpif`, `gp`, `musicxml` (planned), `midi` (planned)

---

## Testing

Tests live in `Tests/Motif.Core.UnitTests/` and cover:

- End-to-end reads against real `.gp` fixture files
- Articulation and rhythm mapping
- Navigation resolver (repeat/alternate ending sequences)
- Write path (round-trip fidelity, writer diagnostics, articulation parity)
- Public API surface shape

Run all tests:
```
dotnet test
```

Always add or update tests alongside behavior changes.

---

## Design Principles

- **Correctness over convenience** — musical semantics must be preserved exactly
- **Determinism** — identical input → identical output; no hidden state
- **Separation of concerns** — parsing, mapping, unmapping, and output are distinct pipeline stages
- **Ergonomics** — consumers operate on `GuitarProScore`, never on raw GPIF references
- **Library-first** — the core library has no host-specific dependencies (no web, EF, DI frameworks)

---

## Current Status and Gaps

See [docs/v1_Todo.md](docs/v1_Todo.md) for the v1 release plan and remaining work.

Highest-priority open areas:
1. DS/DC/Coda/Fine full notation-engine semantics in `DefaultNavigationResolver`
2. Deeper normalization of audio engine / MIDI connection / lyrics (currently passthrough)
3. Schema-driven coverage audit against the GPIF XSD

---

## Working in This Repo

- Keep the raw model and domain model in sync — changes to one typically require changes to both
- The `DefaultScoreMapper` and `DefaultScoreUnmapper` are the most complex files; read them fully before editing
- Boolean CLI flags support `--flag`, `--flag=true`, `--flag=false` — keep new flags consistent with this pattern
