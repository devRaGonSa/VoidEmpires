# TASK-44D-game-layout-route-wrapper

---
id: TASK-44D
title: Game layout route wrapper
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: high
---

## Goal
Ensure all game routes use one consistent GameLayout wrapper.

## Requirements

- Centralize routing so game pages cannot accidentally render in public layout.
- GameLayout should contain:
  - left sidebar;
  - top resource bar;
  - page body;
  - compact account/current planet header if already present.
- PublicLayout should contain:
  - brand/header;
  - login/register forms or public landing;
  - no game sidebar;
  - no resource bar.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/PublicAuthLayout.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/components/AuthRequiredState.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
