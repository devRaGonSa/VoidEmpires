# TASK-43R-shipyard-card-information-hierarchy

---
id: TASK-43R
title: Shipyard card information hierarchy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: medium
---

## Goal
Improve ship card hierarchy.

## Context
Ship cards should prioritize ship name, stock, and production action, then cost/duration/requirements, then role or short tactical description.

## Implementation steps

1. Review shipyard card content and presentation helpers.
2. Make primary information ship name, stock, and produce action.
3. Make secondary information cost, duration, and requirements.
4. Make tertiary information role or short tactical description.
5. Remove duplicated `Astillero orbital` section noise.
6. Build the frontend.

## Files to read first

- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Ship cards have clear primary, secondary, and tertiary information.
- Redundant section noise is removed.
- Blocked state remains clear.

## Constraints

- Do not add combat or fleet movement.
- Keep Spanish-first player copy.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
