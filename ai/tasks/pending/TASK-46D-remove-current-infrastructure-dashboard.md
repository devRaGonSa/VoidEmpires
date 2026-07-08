# TASK-46D

---
id: TASK-46D
title: Remove current infrastructure dashboard
status: pending
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Remove separate infrastructure/current-building dashboard from Construction.

## Context
Construction should present optional active queue and a single building catalog; current levels belong inside cards.

## Implementation steps

1. Inspect Construction page infrastructure/current building panels.
2. Remove separate Infraestructura / Edificios actuales block.
3. Ensure each building card shows the current level.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx
- src/VoidEmpires.Frontend/src/api/planetTypes.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx

## Acceptance criteria

- No separate Infraestructura / Edificios actuales panel is shown.
- Current building level appears inside each building catalog card.
- Construction page is optional active queue plus single building catalog.

## Constraints

- Do not duplicate resources inside module body.

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

