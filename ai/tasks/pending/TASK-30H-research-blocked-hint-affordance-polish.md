# TASK-30H

---
id: TASK-30H-research-blocked-hint-affordance-polish
title: Polish blocked research affordances
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Ensure blocked research options are visually distinct and cannot be mistaken for executable actions.

## Context
Blocked research hints are still shown for discovery, but must not expose a path to submit.

## Implementation steps

1. Audit available vs blocked card rendering in `ResearchPage.tsx`.
2. Ensure blocked states never render primary confirm affordance.
3. Add reason labels in Spanish:
   - `faltan recursos`
   - `requisito pendiente`
   - `investigación en curso`
   - `no disponible en esta lectura`
   - `fuera de alcance`
4. Confirm available items remain visually distinct from blocked.
5. Validate no gameplay behavior changes from visual updates.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- Blocked items cannot be confused as immediately actionable.
- All blocked reasons are clear in Spanish.
- Distinction between available and blocked is visually clear.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- No broad visual redesign.
- No mutation changes in this task.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `feat(frontend): polish blocked research affordances`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
