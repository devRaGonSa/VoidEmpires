# TASK-14O

---
id: TASK-14O
title: Phase 14O - Planet construction cross navigation and state continuity
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Connect Planet, Construction, Galaxy, and Fleets cleanly through URL context.

## Context
The new construction flow needs to preserve context as the user moves between surfaces. This task should keep civilization and planet references consistent and avoid the previous mistake of confusing planetId with civilizationId.

## Implementation steps

1. Review URL context and navigation helpers across Galaxy, Planet, Construction, and Fleets.
2. Make Planet links preserve context toward Galaxy, Fleets, and Construction.
3. Make Construction links return to Planet, Galaxy, and Fleets where context exists.
4. Keep cross-surface context simple and query-param based.

## Files to read first

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `/construction` route page

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`, if needed
- `/construction` route page and helpers

## Acceptance criteria

- Galaxy to Planet preserves the correct context.
- Planet to Construction works.
- Construction to Planet works.
- Planet to Fleets works.
- No ownership error appears when using the seeded Aurelia context.

## Constraints

- Do not add global state unless the app already uses it for this pattern.
- Do not confuse planet and civilization ids.
- Keep navigation links non-invasive.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms the cross-navigation flow works with the seeded Aurelia planet

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
