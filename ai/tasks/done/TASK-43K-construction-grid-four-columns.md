# TASK-43K-construction-grid-four-columns

---
id: TASK-43K
title: Construction grid four columns
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: medium
---

## Goal
Adjust the construction catalog layout to a compact four-column desktop grid.

## Context
The building catalog should be scannable like a browser strategy game catalog, not a set of huge containers.

## Implementation steps

1. Review the Construction page and shared catalog card styles.
2. Make desktop layout target four building cards per row.
3. Add tablet and mobile fallbacks.
4. Ensure each card shows name, level, cost, duration, requirement/block reason, and build/enqueue action.
5. Avoid layout shifts and oversized containers.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Desktop construction catalog targets four cards per row.
- Tablet/mobile layouts remain readable.
- Cards are compact and scannable.
- Required card information remains visible.

## Constraints

- Do not hide blocked buildings.
- Do not use viewport-scaled font sizes.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
