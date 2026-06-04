# TASK-22R

---
id: TASK-22R
title: Phase 22R - Espionage signal and coverage label normalization
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22Q-22Z - Espionage copy normalization and final read-only polish"
priority: high
---

## Goal

Translate and normalize Espionage signal and coverage labels into concise Spanish gameplay language.

## Purpose

Signal and coverage summaries are currently safe functionally, but they still expose English row-oriented wording that feels like raw read-model output. This task converts those summaries into player-facing Spanish without changing the read-only intelligence boundary.

## Current problem

Visible labels still include English phrases such as:

- `passive signal rows available`
- `No passive signal rows available`
- `sensor profile rows`
- `detection coverage rows`
- `visible transfer trajectories`

These phrases leak backend row terminology, read awkwardly in cards, and weaken the accepted cockpit polish baseline.

## Context

The Espionage cockpit only interprets passive observations already derived from visibility, transfer overlays, sensor metadata, and detection-readiness summaries. These are not active surveillance mechanics and must not be reframed as executable missions or real-time scanning.

## Files to read first

- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/espionageViewModel.ts`
- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`

## Component discovery

Locate the helpers that currently generate signal and coverage summaries. Check whether the labels are built from counts in the page component, derived view-model properties, or presentation functions. Reuse the existing presentation owner and avoid sprinkling ad hoc string conditionals through JSX.

## Implementation requirements

1. Add or refine helper functions so signal and coverage summaries are authored in one stable place.
2. Replace English phrases with clear Spanish equivalents, including patterns such as:
   - `1 passive signal rows available` -> `1 señal pasiva disponible`
   - `6 passive signal rows available` -> `6 señales pasivas disponibles`
   - `No passive signal rows available` -> `Sin señales pasivas disponibles`
   - `1 sensor profile rows` -> `1 lectura de perfil de sensores`
   - `3 local sensor profile rows` -> `3 lecturas locales de sensores`
   - `3 detection coverage rows` -> `3 lecturas de cobertura de detección`
   - `1 visible transfer trajectories` -> `1 trayectoria de transferencia visible`
3. Handle singular and plural correctly.
4. Remove `row` or `rows` wording from visible card copy.
5. Prefer gameplay-facing terms such as:
   - `señal`
   - `lectura`
   - `cobertura`
   - `trayectoria`
   - `observación`
6. Keep raw signal type names or source keys only in diagnostics if they remain useful for debugging.

## UI/UX requirements

- Spanish-first visible summaries
- Compact card text that scans quickly
- No misleading implication of active surveillance or live tracking
- Empty states should read as intentional lack of current observations, not as a broken data source

## Backend/API requirements

- None expected.
- Avoid backend changes unless the frontend cannot safely separate user-facing labels from raw source names.

## Expected files to modify

- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/espionageViewModel.ts`
- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`

## Safety constraints

- No active surveillance execution
- No polling or WebSockets
- No new API requests
- No gameplay scope expansion

## Acceptance criteria

- No visible signal or coverage card shows `rows`, `passive signal`, `sensor profile`, `detection coverage`, or `visible transfer trajectories` in English.
- Singular and plural summaries are grammatically correct.
- The cockpit remains clearly read-only.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Some raw backend labels may still appear in collapsed diagnostics if they remain useful and clearly secondary.
- If multiple helpers already format similar counts, prefer consolidating the copy owner rather than duplicating Spanish pluralization logic.

## Commit and push

1. Run `git status`.
2. Confirm only the Espionage presentation layer changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep the change under 200 lines if possible by reusing existing helpers.
