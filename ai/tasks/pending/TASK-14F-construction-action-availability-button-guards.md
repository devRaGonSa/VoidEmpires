# TASK-14F

---
id: TASK-14F
title: Phase 14F - Construction action availability button guards
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Make construction action buttons visually match their actual availability.

## Context
Some blocked construction cards still present as if they are ready actions. This task should make blocked actions look blocked, prevent accidental mutation calls, and keep the UI honest about why a command cannot run.

## Implementation steps

1. Review how construction actions are enabled, disabled, and labeled in the Planet UI.
2. Make blocked actions visibly disabled or secondary.
3. Prevent blocked actions from calling mutation endpoints.
4. Replace misleading action labels with Spanish reasons or detail prompts.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`, for guard styling patterns if useful

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- construction action presentation styles or helpers, if needed

## Acceptance criteria

- Insufficient-resource actions no longer look like ready blue actions.
- Blocked actions are visibly disabled or secondary.
- Clicking a blocked action does not call a mutation endpoint.
- Spanish reasons are concise and readable.

## Constraints

- Keep blocked actions honest and non-executable.
- Do not add blind mutation buttons.
- If a details affordance remains, label it as a view action rather than a prepare action.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms blocked cards are visually blocked and cannot enqueue

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
