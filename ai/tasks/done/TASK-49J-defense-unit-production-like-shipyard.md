# TASK-49J

---
id: TASK-49J
title: Defense unit production like shipyard
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Defenses unit cards must behave like shipyard.

## Context
Unit defenses use planetary asset production with quantity input and Construir action.

## Implementation steps

1. Inspect defense UI and backend asset production target.
2. Ensure unit cards show quantity input, current quantity, Construir, and inline block reason.
3. Ensure no "Nivel 0 -> 1" appears for unit defenses.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts

## Acceptance criteria

- Missile, laser, ion, and plasma defenses are unit-based.
- Special defenses remain level-based only if explicitly modeled.
- AssetProductionTarget.Planetary is used.

## Constraints

- No combat.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
