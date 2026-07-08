# TASK-46G

---
id: TASK-46G
title: Single building catalog container
status: pending
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Make Construction use one single building catalog container.

## Context
All buildings should appear together in one compact catalog grid. Categories can influence order or badges but must not split the page into large sections.

## Implementation steps

1. Inspect Construction catalog grouping and sorting.
2. Render all building cards in one grid.
3. Sort by product order: economy/resource production, energy/storage, colony/core infrastructure, research, shipyard/orbital, military/ground, advanced.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx
- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Domain/Buildings/BuildingCatalog.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx

## Acceptance criteria

- All building cards are rendered in one catalog grid.
- No separate category containers such as logistics/construction are rendered.
- Category may appear as a small badge inside each card.
- Desktop grid targets 4 cards per row.
- At least 4 visible building cards.

## Constraints

- Preserve current construction actions and backend source of truth.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

