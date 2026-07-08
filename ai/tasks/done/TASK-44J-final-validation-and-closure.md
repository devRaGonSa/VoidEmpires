# TASK-44J-final-validation-and-closure

---
id: TASK-44J
title: Final validation and closure
status: pending
type: release
team: platform
supporting_teams: [frontend, security]
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: high
---

## Goal
Close Block 44.

## Requirements

- Move all `TASK-44*` to `ai/tasks/done`.
- `ai/tasks/pending` must contain only `.gitkeep`.
- Commit and push final state.

## Expected final state

- Login/Register standalone without sidebar.
- Authenticated Inicio/Planet/game modules show left sidebar.
- Top resource bar remains.
- Anonymous users do not see game content mixed with `Entrar / Crear cuenta`.
- Duplicated in-page module navigation remains removed.
- Copy guard remains green.
- Lazy route guard remains green.
- Build/test/frontend/scripts pass.

## Files to read first

- ai/current-state.md
- docs/dev/product-readiness-report.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- scripts/check-repo-secret-scan.ps1

## Expected files to modify

- ai/current-state.md
- ai/tasks/done/TASK-44A-shell-regression-audit.md
- ai/tasks/done/TASK-44B-restore-game-shell-sidebar.md
- ai/tasks/done/TASK-44C-fix-authenticated-vs-anonymous-rendering.md
- ai/tasks/done/TASK-44D-game-layout-route-wrapper.md
- ai/tasks/done/TASK-44E-preserve-module-page-decluttering.md
- ai/tasks/done/TASK-44F-sidebar-active-state-and-route-params.md
- ai/tasks/done/TASK-44G-resource-bar-game-shell-integration.md
- ai/tasks/done/TASK-44H-auth-shell-static-guards.md
- ai/tasks/done/TASK-44I-docs-and-current-state-sidebar-correction.md
- ai/tasks/done/TASK-44J-final-validation-and-closure.md

## Final validation required

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`
- `git status`
- `dir ai/tasks/pending`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
