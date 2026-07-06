# TASK-41AF

---
id: TASK-41AF
title: Product confirmation modals
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Polish confirmation modals into product language.

## Context
Construction, Research, and Shipyard modals should sound like real game actions without development database warnings in primary copy.

## Implementation steps

1. Inspect shared modal components and the Construction, Research, and Shipyard modal usage.
2. Rewrite modal titles, body copy, button labels, and confirmation text into product language.
3. Remove development database warnings from primary modal copy.
4. Show any technical note only in operator mode if needed.
5. Keep confirmation checkbox and existing mutation behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx

## Acceptance criteria

- Construction, Research, and Shipyard modals use product language.
- Development database warnings are not in primary modal copy.
- Confirmation checkbox and backend behavior remain unchanged.

## Constraints

- Do not change gameplay mutation semantics.
- Do not optimistic-update authoritative state.
- Keep operator technical notes secondary.

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
