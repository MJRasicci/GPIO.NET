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
