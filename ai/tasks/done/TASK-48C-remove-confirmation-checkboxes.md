# TASK-48C

---
id: TASK-48C
title: Remove confirmation checkboxes
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 48"
priority: high
---

## Goal
Eliminar checkbox obligatorio de confirmacion en modales.

## Context
Las modales de Construccion, Investigacion, Astillero y Defensas deben pedir accion directa con resumen, boton principal y cancelar, sin checkbox obligatorio.

## Implementation steps

1. Revisar los modales de Construccion, Investigacion, Astillero y Defensas.
2. Quitar estados y controles de checkbox obligatorio.
3. Mantener resumen de coste/duracion y validaciones backend.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- ai/tasks/pending/TASK-48C-remove-confirmation-checkboxes.md

## Acceptance criteria

- Construccion, Investigacion, Astillero y Defensas no requieren checkbox obligatorio.
- Las modales muestran resumen de coste/duracion, boton principal y cancelar.
- Botones principales: Iniciar construccion, Iniciar investigacion, Iniciar produccion, Construir defensas.
- No se cambian validaciones backend.

## Constraints

- No tocar login/register.
- No implementar simulacion global.
- Backend sigue siendo fuente de verdad.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
