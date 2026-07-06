# TASK-41V

---
id: TASK-41V
title: Galaxy product readiness copy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Galaxy page product-facing.

## Context
Galaxy should use strategic map language while keeping exploration and colonization mutations unavailable.

## Implementation steps

1. Inspect Galaxy page hero copy, map states, handoffs, diagnostics, and unavailable actions.
2. Replace dev/readiness wording with strategic map product language.
3. Describe exploration and colonization as unavailable or pending activation without dev references.
4. Keep colonization mutation absent.
5. Hide technical diagnostics from product mode.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/GalaxyPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/GalaxyPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Galaxy page uses strategic product language.
- Exploration/colonization unavailable copy has no dev references.
- No colonization mutation is added.

## Constraints

- Do not add exploration execution or colonization mutation.
- Do not fake map state.
- Preserve route params and lazy loading.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`

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
