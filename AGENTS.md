# AGENTS.md - GPIO.NET Guidance

This repository is the canonical destination for the production-ready Guitar Pro parsing library.

## Mission

Build a robust .NET library that:
1. Reads Guitar Pro `.gp` files,
2. Extracts and parses `Content/score.gpif`,
3. Produces a clean, navigable object model representing the transcription.

## Primary Reference Project

Use this as the **main source of truth for prior consolidated logic**:
- `D:\source\GPML`

Most relevant areas in that repo:
- `D:\source\GPML\Source\GPIO`
- `D:\source\GPML\docs\CONSOLIDATION_NOTES_2026-02-20.md`
- `D:\source\GPML\reference\gpif-schema\GPIF.xsd`
- `D:\source\GPML\reference\gpif-schema\GPIF.dtd`

## Historical/Secondary References (read-only unless explicitly requested)

- `D:\source\cleanup\GPML\GPML-1`
- `D:\source\cleanup\GPML\GPML-2`
- `D:\source\cleanup\GPML\GPML-5`
- `D:\source\cleanup\GuitarProBrowser\*`
- `D:\source\GPIO`
- `D:\source\GPIF`
- `D:\source\GuitarProXml`

Treat these as lineage/reference material, not canonical architecture.

## Architecture Requirements

1. **Library-first design**
   - Keep core package free of host-specific concerns.
   - No web app/EF/background service coupling inside core parser/mapping.

2. **Two-layer model is expected**
   - **Raw GPIF model** for faithful XML representation.
   - **Mapped domain model** for ergonomic traversal and business logic.

3. **Reference resolution must be explicit**
   - GPIF uses indirection heavily (IDs and space-separated ref lists).
   - Implement deterministic, testable map/index stages.

4. **Error handling**
   - Fail with clear exceptions for missing critical entries (e.g., missing `Content/score.gpif`).
   - Gracefully handle optional or sparse sections without crashing.

5. **Testing strategy**
   - Add fixture-based tests using representative real GP files / extracted GPIF.
   - Include edge cases: tuplets, alternate endings, odd tunings, sparse properties, empty voices/beats.

## Quality Bar

- Prioritize correctness and maintainability over speed of implementation.
- Small focused commits with clear intent.
- Preserve backwards compatibility deliberately; document breaking changes.

## Collaboration Notes

If you are a future agent contributing here:
- Read this file first.
- Review `README.md` and existing tests before coding.
- Keep the API coherent and minimal.
- Prefer adding tests before/with behavior changes.
