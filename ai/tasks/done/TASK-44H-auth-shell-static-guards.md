# TASK-44H-auth-shell-static-guards

---
id: TASK-44H
title: Auth shell static guards
status: pending
type: tooling
team: frontend
supporting_teams: []
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: high
---

## Goal
Add or update lightweight guards to prevent this regression.

## Requirements

- Add frontend/static guard if feasible to ensure game routes use GameLayout/sidebar.
- Ensure public auth routes do not use GameLayout/sidebar.
- Extend existing route/lazy guard only if appropriate.
- Avoid brittle visual tests.

## Files to read first

- scripts/check-frontend-route-lazy-imports.ps1
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/PublicAuthLayout.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx

## Expected files to modify

- scripts/check-frontend-route-lazy-imports.ps1

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
