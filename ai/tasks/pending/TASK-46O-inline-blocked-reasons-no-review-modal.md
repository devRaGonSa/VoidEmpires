# TASK-46O

---
id: TASK-46O
title: Inline blocked reasons no review modal
status: pending
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Replace block-review modal with inline blocked reasons.

## Context
Shipyard and Defenses blocked cards should explain missing building, missing research, insufficient resources, capacity requirement, or other reasons directly on the card in Spanish.

## Implementation steps

1. Inspect blocked state handling for Shipyard and Defenses.
2. Remove Revisar bloqueo and modal-only explanations.
3. Render Spanish inline blocked reasons on each blocked card.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/shipyardViewModel.ts
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx

## Acceptance criteria

- Shipyard and Defenses blocked cards show inline reasons.
- No Revisar bloqueo button remains.
- No modal solely for blocked-state explanation remains.

## Constraints

- Use Spanish player-facing copy.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git status`.
2. Stage intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

