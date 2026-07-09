# TASK-48B

---
id: TASK-48B
title: Construction queue compact style
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 48"
priority: high
---

## Goal
Cambiar la cola de construccion a formato compacto.

## Context
Construccion debe mostrar una cola activa compacta similar a Investigacion activa y no un dashboard grande.

## Implementation steps

1. Revisar la vista de Construccion y los componentes compartidos de planeta.
2. Renderizar la cola activa solo cuando existan ordenes.
3. Mostrar edificio, nivel objetivo, estado y hora de cierre.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- ai/tasks/pending/TASK-48B-construction-queue-compact-style.md

## Acceptance criteria

- Si no hay cola activa, no se muestra bloque.
- Si hay cola activa, se muestra un bloque compacto tipo "Investigacion activa".
- Se muestra solo edificio, nivel objetivo, estado y hora de cierre.
- No se muestra tarjeta grande con coste, accion, empieza/termina y estructura tipo dashboard.

## Constraints

- No implementar simulacion global.
- No fakear estado en frontend.
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
