# TASK-15H

---
id: TASK-15H
title: Sidebar labels and module status consistency
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Make the sidebar and module status labels consistent with the new boundaries.

## Current problem
The sidebar modules exist, but the pages do not clearly communicate which are implemented, placeholders, read-only, prepared or coming soon.

## Context from current implementation
The shell already shows `Mapa de mando` and multiple nav links. This task should not redesign the shell; it should align labels and status copy with the module boundary model.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/components/AppShell` or equivalent
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/components/AppShell*`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/*.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Add consistent status treatment for sidebar modules if the current design supports it:
  - `Activa`
  - `Solo lectura`
  - `Preparada`
  - `Próximamente`
- Avoid cluttering the sidebar if badges make it too noisy.
- Make active route highlighting reliable for:
  - Planeta
  - Construcción
  - Investigación
  - Ejército Tierra
  - Astillero
  - Defensas
  - Flotas
  - Galaxia
- Align route names and displayed labels.
- Ensure module pages use the same displayed label as the sidebar.

## UI/UX requirements
- Keep the sidebar simple.
- Do not overdecorate.
- The user should understand they are changing cabins, not opening debug panels.

## Backend/API requirements
- No backend change.

## Safety constraints
- No gameplay change.

## Acceptance criteria
- Sidebar active state works across all module routes.
- Labels are consistent.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- More advanced module badges can wait until auth/player state exists.
