# TASK-48A

---
id: TASK-48A
title: Home remove detailed queue panels
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 48"
priority: high
---

## Goal
Quitar del Inicio los paneles detallados de colas y movimiento.

## Context
Inicio debe seguir mostrando resumen del planeta actual, civilizacion, campos, produccion y recursos basicos, sin tarjetas detalladas de colas vacias.

## Implementation steps

1. Revisar la composicion actual de Inicio.
2. Eliminar del Inicio los paneles detallados de colas y movimiento.
3. Mantener las paginas especificas sin cambios.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/components/PlanetOverviewPanel.tsx
- src/VoidEmpires.Frontend/src/components/QueueSummaryPanels.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- ai/tasks/pending/TASK-48A-home-remove-detailed-queue-panels.md

## Acceptance criteria

- Inicio muestra resumen del planeta actual, civilizacion, campos, produccion y recursos basicos.
- Inicio no muestra tarjetas "Sin obras en cola", "Sin investigacion activa", "Sin produccion orbital", "Sin defensas en cola" ni "Sin movimientos de flota".
- No se modifican Construccion, Investigacion, Astillero ni Defensas.

## Constraints

- No tocar login/register.
- No quitar sidebar.
- No quitar barra superior de recursos.
- No generar imagenes.
- Backend sigue siendo fuente de verdad.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
