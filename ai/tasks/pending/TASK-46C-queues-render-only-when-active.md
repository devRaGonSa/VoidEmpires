# TASK-46C

---
id: TASK-46C
title: Render queues only when active
status: pending
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Render queue sections only when active queue items exist.

## Context
The four core modules should not show empty queue cards or duplicate empty messages.

## Implementation steps

1. Inspect queue rendering in Construction, Research, Shipyard and Defenses.
2. Gate each queue section on active items.
3. Remove duplicate empty queue messages.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/QueueSummaryPanels.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx

## Acceptance criteria

- Construction shows "Obras en cola" only if active construction orders exist.
- Research shows "Investigacion activa" only if active research exists.
- Shipyard shows "Produccion orbital" only if active ship/orbital production exists.
- Defenses shows "Produccion defensiva" only if active defense production exists.
- Empty queue cards and duplicate empty messages are not rendered.

## Constraints

- Preserve existing queue behavior when active items exist.

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

