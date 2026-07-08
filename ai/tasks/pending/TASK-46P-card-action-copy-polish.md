# TASK-46P

---
id: TASK-46P
title: Card action copy polish
status: pending
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Polish action labels across the four modules.

## Context
Action labels should be clear, Spanish-first and player-facing across Construction, Research, Shipyard and Defenses.

## Implementation steps

1. Inspect card action labels in the four modules.
2. Normalize labels to Construir, En cola, Bloqueado, Investigar, Investigando, Producir, and Unidades where applicable.
3. Remove forbidden/no-action copy.

## Files to read first

- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx
- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts
- src/VoidEmpires.Frontend/src/utils/defensePresentation.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx
- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx

## Acceptance criteria

- Construction labels: Construir, En cola where applicable, Bloqueado only if blocked.
- Research labels: Investigar, Investigando where applicable.
- Shipyard label: Producir; quantity label: Unidades.
- Defenses label: Construir; quantity label Unidades for unit-based defenses.
- Avoid Revisar bloqueo, Sin accion local, Accion no disponible aqui, and Produccion no disponible aqui.

## Constraints

- Do not change action semantics.

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

