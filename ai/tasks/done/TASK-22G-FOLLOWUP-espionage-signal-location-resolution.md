# TASK-22G-FOLLOWUP

---
id: TASK-22G-FOLLOWUP
title: Espionage signal location resolution and handoff polish
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: medium
---

## Goal

Resolve passive-signal cards to their best known system or planet context so the catalog never falls back to an ambiguous location when the read model already contains enough related information.

## Context

`TASK-22G` added the grouped intelligence catalog, but the change budget was already stretched. Signal-only rows still deserve a small follow-up so handoff labels and location subtitles stay truthful for planet-bound passive signals.

## Implementation steps

1. Review how passive signals are mapped from the backend into the espionage view-model.
2. Link planet-bound signals to their parent target or system when that relationship is already present in the UI state.
3. Keep the catalog read-only while improving subtitle and handoff accuracy for signal cards.

## Files to read first

- `src/VoidEmpires.Frontend/src/utils/espionageViewModel.ts`
- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Infrastructure/StrategicMap/DevEspionageUiStateService.cs`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/utils/espionageViewModel.ts`
- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`

## Acceptance criteria

- Signal cards resolve to the best available system or planet context when that information is already present.
- Handoff labels for signal-only cards remain read-only and truthful.
- The frontend build passes.
- No build artifacts are committed.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend` succeeds
- no new warnings or obvious regressions are introduced

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
