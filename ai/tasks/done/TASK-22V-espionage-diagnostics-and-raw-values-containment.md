# TASK-22V

---
id: TASK-22V
title: Phase 22V - Espionage diagnostics and raw values containment
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22Q-22Z - Espionage copy normalization and final read-only polish"
priority: high
---

## Goal

Ensure raw technical values stay inside collapsed diagnostics and no longer leak into the primary Espionage UI unnecessarily.

## Purpose

The accepted cross-cockpit polish baseline keeps DTO names, row counts, identifiers, and backend-oriented labels away from primary player-facing cards. This task brings Espionage fully into that standard.

## Current problem

Espionage consumes several derived readings and can easily leak technical artifacts such as row counts, raw field names, DTO-style wording, capability keys, or source labels into visible summaries. Even when useful for debugging, they should not dominate the primary UI.

## Context

Espionage already relies on derived strategic-map readiness data. The current repository pattern is to keep gameplay-facing meaning in visible cards and technical detail inside collapsed diagnostics or clearly secondary sections.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Frontend/src/utils/espionageViewModel.ts`
- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`
- `docs/dev/cockpit-copy-guidelines.md`

## Component discovery

Search the Espionage frontend files for raw or technical wording, then identify whether those values are meant for diagnostics, support text, or accidental primary labels. Reuse existing disclosure or `<details>` patterns already used in other cockpits.

## Implementation requirements

1. Search Espionage frontend files for technical leakage, including:
   - `id`
   - `dto`
   - `payload`
   - `raw`
   - `row`
   - `endpoint`
   - `capability`
   - `signalProfile`
   - `coverageRows`
2. Move technical or raw values into collapsed diagnostics if they are currently visible in primary cards.
3. Keep diagnostics collapsed by default.
4. Use Spanish wrappers for technical detail, for example:
   - `Lectura técnica`
   - `Datos técnicos`
   - `Identificadores`
5. Do not dump large JSON payloads into the primary UI.
6. Preserve useful debugging information when it helps local QA or developer troubleshooting.
7. Prefer small presentation refactors over deleting information outright.

## UI/UX requirements

- Primary UI should stay clean and gameplay-oriented.
- Diagnostics should be discoverable but secondary.
- Technical details must not compete visually with target summaries, confidence cues, or handoff actions.

## Backend/API requirements

- None.

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Frontend/src/utils/espionageViewModel.ts`
- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`

## Safety constraints

- No behavior changes
- No API changes
- No removal of genuinely useful diagnostics without an equivalent secondary location

## Acceptance criteria

- The primary Espionage UI no longer exposes raw technical values unnecessarily.
- Diagnostics remain collapsed by default.
- Technical details, when present, are clearly secondary and wrapped in Spanish.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Some truncated GUIDs or raw source names may remain in diagnostics only.
- Avoid overfitting the page to hide all technical detail if that would make QA harder; containment is the goal, not total deletion.

## Commit and push

1. Run `git status`.
2. Confirm only Espionage presentation/view-model files changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep the task focused on containment, not on redesigning diagnostics.
