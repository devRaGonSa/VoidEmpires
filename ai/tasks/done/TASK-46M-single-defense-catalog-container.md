# TASK-46M

---
id: TASK-46M
title: Single defense catalog container
status: done
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Make Defenses use one single defense catalog container.

## Context
All defensive cards should appear together in one compact catalog grid.

## Implementation steps

1. Inspect Defenses catalog grouping and copy.
2. Render all defense cards in one grid.
3. Remove confusing local/no-production copy and unit-defense level copy.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- src/VoidEmpires.Frontend/src/api/defenseTypes.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts

## Acceptance criteria

- All defense cards are rendered in one catalog grid.
- No separate category containers are rendered.
- Category/type may appear as small badge inside card.
- Desktop grid targets 4 cards per row.
- At least 4 visible defense cards.
- Remove "Sin accion local", "Produccion defensiva no disponible aqui", and confusing level copy for unit-based defenses.

## Constraints

- Preserve backend source of truth.

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
