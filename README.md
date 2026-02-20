# GPIO.NET

A clean, production-focused .NET library for reading Guitar Pro files (`.gp`), extracting/translating GPIF score data, and exposing a clear object model for musical traversal.

## Project Goals

1. **Accurate parsing**
   - Open `.gp` archives safely.
   - Extract and deserialize `Content/score.gpif`.
   - Preserve score semantics (tracks, bars, voices, beats, notes, rhythms, properties, articulations, etc.).

2. **Clean domain model**
   - Move from GPIF's reference-heavy XML shape (cross-indexed IDs and space-delimited references) to an ergonomic OOP model.
   - Make iteration intuitive:
     - Score → Tracks → Measures/Bars → Voices → Beats → Notes
   - Keep links back to source metadata where useful.

3. **Deterministic mapping layer**
   - Maintain a robust intermediate mapping/index stage for resolving GPIF references.
   - Handle edge cases (missing refs, optional sections, malformed/partial data) safely.

4. **Production readiness**
   - Test-driven fixtures across real-world GP files.
   - Stable API surface, clear versioning, and strong documentation.
   - No app/runtime concerns in core library (no web host/db pipeline coupling).

## Non-Goals (for core package)

- UI, web app, or database persistence concerns.
- Hardcoded machine-specific paths.

## Current Direction

This repo is the new canonical home for the finalized library. Historical experiments and reference implementations are documented in `AGENTS.md` for migration guidance.

## Tooling

A small companion CLI exists for quick local conversion/output:

- Project: `Source/GPIO.NET.Tool`
- Usage: `dotnet run --project Source/GPIO.NET.Tool -- <input.gp> [output-path] [options]`
- Default output path (if omitted) depends on format:
  - `json` -> `<input>.mapped.json`
  - `gpif` -> `<input>.score.gpif`
  - `midi` -> `<input>.mid` (planned, not yet implemented)

Common options:
- `--format json|gpif|midi`
- `--out <path>`
- `--json-indent[=true|false]`
- `--json-ignore-null[=true|false]`
- `--json-ignore-default[=true|false]`

Writer modes (mapped JSON -> .gp):
- Full write: `--from-json --format json`
- Patch existing GP (preferred while coverage matures):
  - `--from-json --patch-from-json --source-gp <existing.gp> --format json`
- `--diagnostics-out <path>` to capture diagnostics
- `--diagnostics-json` to write diagnostics as JSON

Example edit workflow:
1. Export: `dotnet run --project Source/GPIO.NET.Tool -- input.gp score.json --format json`
2. Edit `score.json`
3. Patch original GP: `dotnet run --project Source/GPIO.NET.Tool -- score.json output.gp --from-json --patch-from-json --source-gp input.gp --format json`
