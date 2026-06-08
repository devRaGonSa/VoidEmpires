# TASK-31D

---
id: TASK-31D
title: Shipyard confirmation panel
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Add an explicit confirmation step to Shipyard production so real Development database mutation requires a deliberate final action.

## Context
The confirmed mutation pattern is already accepted in Construction and Research. Shipyard needs a clear warning and an explicit final action that explains Development-only persistence, backend-owned resource deduction, and the absence of automatic completion from the normal UI.

## Implementation steps

1. Add a dedicated confirmation panel, section, or modal to Shipyard that activates only after a valid candidate is selected.
2. Render the required Spanish safety copy about creating a real Development database production order, backend-confirmed resource deduction, and no automatic completion.
3. Add `Confirmar produccion` and `Cancelar` actions with availability tied to the selected candidate and current readiness state.
4. Ensure the new confirmation surface is purely local state in this task unless a compile-only helper refactor is needed.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Shipyard has an explicit confirmation step before real enqueue is wired.
- The required Spanish safety copy is present and understandable.
- The confirm action stays disabled unless a valid candidate is selected.
- No accidental POST is triggered from selection or confirmation rendering alone.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not wire the real POST in this task unless required only for compile continuity

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
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
