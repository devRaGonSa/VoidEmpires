# TASK-43F-planet-route-alias-or-merge

---
id: TASK-43F
title: Planet route alias or merge
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Resolve redundancy between Inicio and Planeta.

## Context
Players should not see two different vague planet/home pages. Choose the simplest routing model that preserves useful deep links.

## Implementation steps

1. Review current home, planet, and route helper behavior.
2. Decide whether `/planet` aliases to the same current planet overview used by Inicio or `/` redirects to the canonical planet overview after auth.
3. Update sidebar labels so duplicated Inicio/Planeta entries do not both point to the same vague concept.
4. Preserve deep links with `civilizationId` and `planetId` where needed.
5. Run route lazy-import guard.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Acceptance criteria

- Inicio and Planeta no longer present two redundant vague pages.
- Sidebar labels make sense for the chosen route model.
- Deep links remain supported where needed.
- Lazy route validation passes.

## Constraints

- Preserve lazy loading.
- Avoid broad route rewrites beyond this decision.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
