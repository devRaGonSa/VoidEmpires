# TASK-43Q-shipyard-grid-four-columns

---
id: TASK-43Q
title: Shipyard grid four columns
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: medium
---

## Goal
Change Shipyard layout to a compact grid.

## Context
The ship catalog should target four compact cards per row on desktop and remain readable on smaller screens.

## Implementation steps

1. Review Shipyard page and catalog card styles.
2. Make desktop layout target four ship cards per row.
3. Ensure cards show ship name, role/class, stock, cost, duration, requirement/block reason, and produce action.
4. Remove any fleet movement action affordances if present.
5. Build the frontend.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Desktop shipyard catalog targets four cards per row.
- Cards are compact and scannable.
- No fleet movement actions are present.

## Constraints

- Do not hide blocked ships.
- Do not add movement or combat.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
