# Motif v1 Todo

## Snapshot

- `Motif.Core` is now the format-agnostic, mutable score model.
- `Motif.Extensions.GuitarPro` owns the GPIF/raw/archive/XML read-write pipeline.
- Core exposes `IScoreReader` / `IScoreWriter`; the GP package implements them.
- Core model data is extensible, JSON-serializable, and now owns navigation via `ScoreNavigation`.
- The CLI works against the package split and routes by inferred/explicit formats instead of GP-only modes.
- Core and GP test suites are split and currently passing in targeted verification.

## v1 Goals

- Ship `Motif.Core` as the stable edit-first music domain model.
- Ship `Motif.Extensions.GuitarPro` as the v1 format package with strong round-trip fidelity.
- Finalize Core vs Guitar Pro boundaries, public API shape, packaging, and docs.

## Non-Goals

- Playback engine
- Notation engraving/rendering engine
- Editor UI framework
- DAW abstraction layer
- Universal normalization across every music format
- Shipping multiple fully-supported formats in v1

## Architecture Constraints

- Core is mutable and optimized for direct programmatic editing.
- Core musical data is authoritative; extension data is supplemental fidelity state.
- Import flow: deserialize source -> map Core -> attach extension fidelity.
- Export flow: preserve usable extension data -> infer from Core/defaults -> emit diagnostics when fidelity degrades.
- Traversal/cache/navigation state is derived state and must be recomputed after traversal-affecting edits.
- Target hierarchy remains `Score -> Track[] -> Staff[] -> StaffMeasure[] -> Voice[] -> Beat[] -> Note[]`.
- Timeline-global measure state belongs on `Score.MeasurePositions`.

## Step Status

- `[x]` Step 1 - Extensibility contracts in Core
  Landed: `IModelExtension` / `IExtensibleModel`, typed helpers, GP extension attachments, Core-only JSON.

- `[~]` Step 2 - Split Core domain vs Guitar Pro fidelity
  Remaining: audit surviving GP-shaped Core properties, decide whether `*Model` names stay, add `GpStaffExtension` after the hierarchy refactor.

- `[~]` Step 3 - Raw cache invariants
  Remaining: define extension invalidation/preservation/regeneration policy, reuse/default rules, and fidelity diagnostics expectations.

- `[x]` Step 4 - Guitar Pro format I/O ownership
  Landed: GPIF/raw/archive/XML/mapper/unmapper live in the GP package; low-level GP seams are internal.

- `[x]` Step 5 - Format-agnostic reader/writer contracts
  Landed: `IScoreReader` / `IScoreWriter` live in Core and are implemented by `GuitarProReader` / `GuitarProWriter`.

- `[~]` Step 6 - Navigation in Core
  Landed: `ScoreNavigation`, `Score.Anacrusis`, Core-owned playback traversal recompute, Core navigation tests.
  Remaining: define invalidation/recompute rules and revisit the input shape after `Score.MeasurePositions`.

- `[~]` Step 7 - CLI
  Landed: package-split CLI, source-extension reattachment for no-op JSON writes, format-pair routing, legacy flag compatibility, GPIF batch export.
  Remaining: revisit routing once non-Guitar-Pro inputs/outputs exist.

- `[~]` Step 8 - Tests
  Landed: Core/GP test split, API surface tests, Core navigation coverage.
  Remaining: hierarchy-refactor tests and extension invalidation/defaulting/regeneration tests.

- `[ ]` Step 9 - Public API review
  Remaining: audit `Motif.Core`, trim GP leakage, decide whether legacy `*Model` names stay, rerun API surface tests after each cleanup pass.

- `[ ]` Step 10 - Packaging and release prep
  Remaining: package metadata, Source Link/symbols/license/readme metadata, dependency validation, release builds, final docs verification.

## Next Up

1. Define derived-state and extension-cache policy.
   - Navigation recompute rules
   - Extension invalidation/preservation/regeneration rules
   - Diagnostics expectations when exact fidelity cannot be preserved

2. Land the hierarchy refactor.
   - Replace `Track.Measures` + `Measure.AdditionalStaffBars` with `Track.Staves` + `Score.MeasurePositions`
   - Normalize malformed imports where possible
   - Add `GpStaffExtension`

3. Run the public API cleanup pass.
   - Audit remaining GP-shaped Core properties such as `BeatModel.Wah`, `BeatModel.Golpe`, `BeatModel.VibratoWithTremBarStrength`, and `NoteArticulationModel.AntiAccentValue`
   - Decide whether legacy `*Model` names stay for v1

4. Finish packaging and release docs.
   - NuGet metadata, Source Link, symbols, readme/license metadata
   - Final package/dependency review
   - Release builds and docs verification

## Release Gate

- Core hierarchy and derived-state policy are finalized.
- Remaining GP-shaped Core/API seams are intentionally resolved.
- Public API review, packaging, docs, and release builds are complete.
- Tests match the finalized architecture and pass.
