# TASK-30C

---
id: TASK-30C-research-hint-selection-state
title: Add explicit research selection state (no mutation)
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Allow users to select and review one research candidate in Research without submitting any mutation.

## Context
Selection and review must exist first so confirmation and enqueue wiring can be added safely in later tasks.

## Implementation steps

1. Add local selected research state in `ResearchPage.tsx`.
2. Render selection affordance for available cards only.
3. Keep blocked cards disabled or visually non-actionable.
4. Show selected summary fields:
   - technology name
   - category
   - current level
   - target level
   - duration
   - cost
   - expected scope
5. Keep all labels and status text in Spanish.
6. Confirm there is no state transition that posts to backend in this task.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Acceptance criteria

- User can select one available research candidate.
- Blocked technologies cannot initiate a submit action.
- Selected summary displays required fields.
- No backend POST is triggered.

## Constraints

- No mutation or POST in this task.
- No auto-complete controls.
- Do not rely on raw payload text for primary UX.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `feat(frontend): add research selection state without submit`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
