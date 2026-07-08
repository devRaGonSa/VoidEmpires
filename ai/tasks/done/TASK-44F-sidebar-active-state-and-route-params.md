# TASK-44F-sidebar-active-state-and-route-params

---
id: TASK-44F
title: Sidebar active state and route params
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: high
---

## Goal
Make sidebar navigation preserve current planet/civilization context.

## Requirements

- Sidebar links should preserve selected `civilizationId`/`homePlanetId`/`planetId` where needed.
- Active module should be visually indicated.
- No raw ids shown as text.
- If route context is unavailable but authenticated `/me` has home planet, use `/me` home planet.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
