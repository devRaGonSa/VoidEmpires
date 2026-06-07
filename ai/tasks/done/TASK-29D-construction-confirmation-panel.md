# TASK-29D

---
id: TASK-29D-construction-confirmation-panel
title: Add explicit confirmation panel before enqueue
status: obsolete
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Add explicit confirmation UI before real enqueue with mandatory Spanish safety messaging.

## Context
No backend submit should happen in this task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

## Expected files to modify

- ai/tasks/pending/TASK-29D-construction-confirmation-panel.md
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

## Implementation steps

1. Add confirmation panel/modal/section tied to selected action.
2. Include exact Spanish copy:
   - Esta acción creará una orden real de construcción en la base de datos de Development.
   - Los recursos se descontarán cuando el backend confirme la orden.
   - No se completará automáticamente desde esta cabina.
3. Add `Confirmar orden` and `Cancelar` controls.
4. Keep confirm disabled until eligible action is selected.

## Acceptance criteria

- Confirmation is explicit and visible.
- Selection alone does not mutate anything.

## Resolution notes

- The current `/construction` route already renders an explicit confirmation section through `PlanetPage variant="construction"`.
- That route has advanced beyond this milestone and already includes real enqueue submission behavior from later work in the branch.
- Re-implementing this task as read-only would require removing newer behavior and would be a regression, so no frontend code change was applied here.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
