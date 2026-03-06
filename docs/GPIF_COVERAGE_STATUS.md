# GPIF Coverage Status (Current)

This document tracks how completely `GPIO.NET` maps GPIF data.

Legend:
- ✅ **Typed**: explicit domain + raw model coverage with read/map/write parity
- 🟡 **Hybrid**: typed core + raw XML passthrough fallback for fidelity
- ⛔ **Passthrough-only / Missing**: not yet normalized into typed models

## 1) Score-level metadata

- ✅ Title / Artist / Album
- ✅ Subtitle / words / music / copyright / notices / headers/footers
- ✅ Layout/zoom fields (`ScoreSystems*`, `ScoreZoom*`, `MultiVoice`)

## 2) Master track / global context

- ✅ `MasterTrack.Tracks` references
- ✅ Master-track anacrusis flag (`Anacrusis`) with read/map/write parity
- ✅ Master-track automations (typed list)
- ✅ Tempo map projection from tempo automations (`TempoEventMetadata`)
- ✅ Unified automation timeline synthesis (master + track events, deterministic ordering, parsed numeric/reference hints, tempo projection attached when applicable)
- 🟡 Master-track RSE (typed minimal + raw XML passthrough)

## 3) Track metadata

- ✅ Name/short name/color/layout/playing style/basic flags
- ✅ Tuning fields (`TuningPitches`, label/instrument/visible)
- ✅ Typed instrument set (`Name/Type/LineCount`)
- ✅ Typed sounds (`Name/Label/Path/Role` + MIDI program fields)
- ✅ Typed playback state value
- ✅ Typed track automations list
- ✅ Typed staff list (`id/cref/tuning/capo/properties`)
- 🟡 Track subsystem blocks as raw XML passthrough for fidelity:
  - `InstrumentSetXml`, `StavesXml`, `SoundsXml`, `RseXml`,
  - `PlaybackStateXml`, `AudioEngineStateXml`, `MidiConnectionXml`,
  - `LyricsXml`, `AutomationsXml`, `TransposeXml`

## 4) Master-bar metadata

- ✅ Repeat start/end/count
- ✅ Alternate endings
- ✅ Section letter/text
- ✅ Directions (`Jump`, `Target`) + extra direction properties dictionary
- ✅ Key metadata (`AccidentalCount`, `Mode`, `TransposeAs`)
- ✅ Fermatas
- ✅ Master-bar `XProperties`

## 5) Bar / voice metadata

- ✅ Bar clef
- ✅ Bar properties dictionary
- ✅ Bar `XProperties`
- ✅ Multi-voice bar mapping/unmapping (all voice refs preserved)
- ✅ Voice properties dictionary
- ✅ Voice direction tags

## 6) Rhythm / beat / note core

- ✅ Base note values
- ✅ Dots + tuplets (primary/secondary factors)
- ✅ Tie duration stitching in mapped domain
- ✅ Beat note reference resolution
- ✅ Note pitch extraction/parsing

## 7) Note articulations

- ✅ Let ring / vibrato / tie / trill / accent / anti-accent
- ✅ Palm mute / muted / tapped / left-hand-tapped / HOPO flags
- ✅ HOPO semantic linkage (origin/destination note IDs + hammer-on/pull-off inference)
- ✅ Slide flags + decoded slide enums (validated against schema reference slide cases)
- ✅ Harmonic typed fields (`HType` text + semantic harmonic-kind mapping + fret)
- ✅ Bend typed curve fields (normalized values/offsets + inferred bend-type semantics)
- ✅ Fingering fields (`LeftFingering`, `RightFingering`) and ornament text
- ✅ Grace-note and beat-effect metadata (`GraceNotes`, `PickStroke`, `VibratoWTremBar`, `Brush`, `Slapped`, `Popped`)
- ✅ Palm-mute beat effect projection from note properties
- ✅ Beat whammy/tremolo-bar curve normalization (`WhammyBar*` property-family, values /50, offsets /100)
- ✅ Rasgueado pattern decoding (`Property name="Rasgueado"`)
- ✅ Dead-slapped beat semantics (`<DeadSlapped />` element)
- ✅ Arpeggio/brush semantic split and brush-duration normalization (`<Arpeggio>` vs `Brush`, XProperties `687935489`/`687931393`, and Android-style default `Brush` duration = 60 ticks)
- ✅ Trill-speed decoding from note XProperty (`XProperty id="688062467"`, threshold buckets: >=240 Sixteenth, >=120 ThirtySecond, >=60 SixtyFourth, <60 OneHundredTwentyEighth)
- ✅ Additional beat-effect elements (`<Tremolo>` with speed value, `<Chord>` ID, `<FreeText>` text)

## 8) Write path status

- ✅ Full write pipeline (`score -> raw GPIF -> zipped .gp`)
- ✅ Writer diagnostics (structured)
- 🟡 Full GP compatibility depends on preserving non-normalized XML segments

## 9) Patch path status

- ✅ Append notes
- ✅ Insert beats
- ✅ Add notes to existing beats
- ✅ Reorder notes within beat
- ✅ Update note pitch/articulation fields
- ✅ Delete notes/beats
- ✅ Append bars/voices
- ✅ Patch diagnostics with operation-level entries
- ✅ CLI: edit JSON -> plan patch -> apply patch (`--plan-only`, `--strict`)

## 10) Remaining highest-priority gaps

1. ⛔ Deep normalization of audio engine / MIDI connection / lyrics structures (currently passthrough-heavy)
2. ⛔ Complete schema-driven element-by-element coverage auditing vs `GPIF.xsd`
3. ⛔ Patch planner support for larger structural edits (new tracks/measures, advanced voice topology)

## Suggested next milestones

- **Milestone A:** Schema coverage audit tool (typed/hybrid/missing per XSD node)
- **Milestone B:** Advanced patch planner for measure/track creation and structural diffs
- **Milestone C:** Deeper normalization of audio engine / MIDI / lyrics blocks
