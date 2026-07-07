# TASK-41L

---
id: TASK-41L
title: Planet product hub final copy
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Planet page product-facing.

## Context
Planet should read as the main command hub, not a development diagnostics surface.

## Implementation steps

1. Inspect the Planet page, related components, materialization controls, diagnostics, and handoff cards.
2. Rename primary sections to Resumen del mundo, Recursos, Producción, Actividad orbital, and Accesos de mando.
3. Remove backend/development labels from product mode.
4. Hide materialization and refresh tools unless operator mode is active.
5. Keep read behavior and backend refresh behavior unchanged.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/api/
- docs/dev/product-mode-visibility-contract.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Planet page uses product-facing hub copy.
- Backend/development labels are absent from product mode.
- Materialization tools are operator-only.
- Read behavior remains unchanged.

## Constraints

- Do not add new gameplay semantics.
- Do not optimistic-update authoritative resources or queues.
- Do not expose raw ids in primary UI.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
