# TASK-46J

---
id: TASK-46J
title: Single shipyard catalog container
status: done
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Make Shipyard use one single ship catalog container.

## Context
All ship cards should appear together in one compact catalog grid, with blocked requirements shown inline.

## Implementation steps

1. Inspect Shipyard catalog grouping and blocking UI.
2. Render all ship cards in one grid.
3. Remove Revisar bloqueo and any block-review modal used only for requirements.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/shipyardViewModel.ts
- src/VoidEmpires.Frontend/src/api/shipyardTypes.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/shipyardViewModel.ts

## Acceptance criteria

- All ship cards are rendered in one catalog grid.
- No separate category containers such as exploration/logistics are rendered.
- Category/role may appear as a small badge inside each card.
- Desktop grid targets 4 cards per row.
- At least 4 visible ship cards.
- Revisar bloqueo and block-review modal are removed.
- Blocked ships show requirements inline on the card.

## Constraints

- Do not fake actions.

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
