# TASK-45K

---
id: TASK-45K
title: Copy guard core module clutter
status: pending
type: quality
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Extend copy guard to prevent clutter panels from returning.

## Context
Normal frontend UI should fail static copy checks if forbidden internal module strings are reintroduced.

## Implementation steps

1. Extend the frontend copy regression guard for forbidden strings: seleccion disponible, mundo guardado, uso local, lectura segura, cargar contexto, contexto cientifico, contexto defensivo, entrada de vista, abrir vista, abrir defensas, dashboard defensivo, que entra aqui, id de civilizacion, id de planeta.
2. Allow docs, dev, or operator-only exceptions only if absolutely necessary.
3. Verify the guard and frontend build.

## Files to read first

- ai/architecture-index.md
- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Copy guard fails normal frontend UI when forbidden clutter copy appears.
- Existing legitimate docs or operator-only files are not incorrectly blocked.

## Constraints

- Do not weaken existing copy guard coverage
- Keep the change minimal

## Validation

Before completing the task ensure:

- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
- npm run build --prefix src/VoidEmpires.Frontend

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
