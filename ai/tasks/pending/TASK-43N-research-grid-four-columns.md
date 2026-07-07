# TASK-43N-research-grid-four-columns

---
id: TASK-43N
title: Research grid four columns
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: medium
---

## Goal
Change Research layout from one huge row per item to a compact grid.

## Context
The technology catalog should target four compact research cards per row on desktop, with responsive fallback for narrower screens.

## Implementation steps

1. Review Research page and card styles.
2. Make desktop layout target four research cards per row.
3. Ensure cards show technology name, level, cost, duration, requirement/block reason, and research action.
4. Avoid full-width cards unless the screen is narrow.
5. Build the frontend.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Desktop research catalog targets four cards per row.
- Cards are compact and scannable.
- Narrow screens remain readable.

## Constraints

- Do not hide blocked technologies.
- Do not use oversized card layouts.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
