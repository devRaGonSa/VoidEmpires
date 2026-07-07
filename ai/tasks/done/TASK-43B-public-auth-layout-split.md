# TASK-43B-public-auth-layout-split

---
id: TASK-43B
title: Public auth layout split
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Split public auth pages from the authenticated game shell.

## Context
Login and registration are public entry screens. They must not render inside the game sidebar, resource bar, or module navigation layout.

## Implementation steps

1. Review current route composition in `App.tsx` and shell behavior.
2. Create or refine a public auth layout for `/login`, `/register`, and `/registro`.
3. Ensure public pages do not show the authenticated sidebar, resource bar, or module navigation.
4. Preserve existing login and registration behavior.
5. Keep routes lazy-loaded according to the existing guard.
6. Build the frontend and run the route lazy-import guard.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/LoginPage.tsx
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/PublicAuthLayout.tsx
- src/VoidEmpires.Frontend/src/pages/LoginPage.tsx
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- `/login`, `/register`, and `/registro` render without game sidebar.
- Public auth pages do not show the resource bar or module navigation.
- Auth behavior remains functional.
- Route lazy-import validation passes.

## Constraints

- Do not remove registration/login functionality.
- Do not expose game-only navigation in public pages.
- Keep UI Spanish-first.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
