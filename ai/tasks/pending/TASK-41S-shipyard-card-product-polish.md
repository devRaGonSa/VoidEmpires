# TASK-41S

---
id: TASK-41S
title: Shipyard card product polish
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Polish shipyard production cards.

## Context
Ship cards should show role, class, stock, cost, duration, and requirements without implying movement or mission gameplay.

## Implementation steps

1. Inspect Shipyard card rendering and helper copy.
2. Make role, class, stock, cost, duration, and requirements clear.
3. Use an existing placeholder asset slot where available.
4. Remove technical metadata from primary cards.
5. Avoid fleet movement or mission actions.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Shipyard cards clearly show requested product information.
- Placeholder slot is intentional when present.
- No fleet movement action appears.

## Constraints

- Do not add or generate final image assets.
- Do not change ship production semantics.
- Do not fake stock or requirements.

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
