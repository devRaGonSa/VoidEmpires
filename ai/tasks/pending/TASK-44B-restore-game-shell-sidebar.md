# TASK-44B-restore-game-shell-sidebar

---
id: TASK-44B
title: Restore game shell sidebar
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: high
---

## Goal
Restore persistent left sidebar for authenticated game pages.

## Requirements

- Authenticated game layout must render left sidebar on desktop.
- Sidebar must include core modules:
  - Inicio
  - Construccion
  - Investigacion
  - Astillero
  - Defensas
  - Flotas
  - Galaxia
  - Mercado
  - Alianza
  - Ranking
  - Espionaje
  - Ejercito tierra
- Sidebar must not appear on login/register pages.
- Sidebar must not be replaced by top-only buttons.
- Existing top resource bar must remain.
- Mobile may collapse/responsive, but desktop must show sidebar visibly.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
