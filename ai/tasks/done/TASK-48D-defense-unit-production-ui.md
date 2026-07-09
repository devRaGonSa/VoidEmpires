# TASK-48D

---
id: TASK-48D
title: Defense unit production UI
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 48"
priority: high
---

## Goal
Hacer que las defensas unitarias se comporten visualmente como Astillero.

## Context
Bateria de misiles, Torreta laser, Canon ionico y Canon de plasma deben mostrarse como produccion por unidades usando el backend existente.

## Implementation steps

1. Revisar el view model y la carta de defensas.
2. Asegurar cantidad actual, input de unidades, boton Construir y motivo inline de bloqueo.
3. Evitar "Nivel 0 -> 1" en defensas unitarias.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- ai/tasks/pending/TASK-48D-defense-unit-production-ui.md

## Acceptance criteria

- Las defensas unitarias muestran cantidad actual, input de unidades, boton Construir y motivo de bloqueo inline si aplica.
- No se muestra "Nivel 0 -> 1" para defensas unitarias.
- Las defensas especiales por nivel se mantienen solo si estan marcadas como especiales.
- No se anade combate.

## Constraints

- No fakear estado en frontend.
- Backend sigue siendo fuente de verdad.
- No añadir combate.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
