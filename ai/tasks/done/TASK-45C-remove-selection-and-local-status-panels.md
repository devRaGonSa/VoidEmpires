# TASK-45C

---
id: TASK-45C
title: Remove selection and local status panels
status: done
type: frontend
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Remove selection and status panels that are not useful to the player.

## Context
Core modules should not duplicate selected planet or civilization context or show internal safety/status copy.

## Implementation steps

1. Remove normal UI cards and badges for Seleccion disponible, Mundo guardado, Uso local, Lectura segura and technical Colonia activa.
2. Remove copy such as "Cada vista vuelve a comprobar la cuenta actual...".
3. Keep only concise player-relevant context if needed in the shell or header.
4. Do not duplicate selected planet or civilization information inside every module page.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src

## Acceptance criteria

- The four core modules no longer show player-irrelevant selection or local status panels.
- Selected context is not duplicated inside every module page.

## Constraints

- Do not remove authenticated sidebar
- Do not remove top resource bar
- Do not modify unrelated modules
- Keep the change minimal

## Validation

Before completing the task ensure:

- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1

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
