# TASK-46Q

---
id: TASK-46Q
title: Copy guard module polish
status: pending
type: tooling
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Extend copy regression guard for the new visual findings.

## Context
Normal frontend UI should fail static copy checks if the removed module clutter returns.

## Implementation steps

1. Inspect the frontend copy regression script.
2. Add forbidden strings for the module polish findings.
3. Allow docs/dev exceptions only if needed.

## Files to read first

- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Guard fails normal frontend UI for: continuar mundo, tecnologias completadas, progreso cientifico, contexto conservado, vista normalizada, revisar bloqueo, sin accion local, produccion defensiva no disponible aqui, construccion v1, investigacion v1, astillero v1, defensas v1, edificios actuales, and infraestructura as standalone construction dashboard title.
- Docs/dev exceptions are allowed only if needed.

## Constraints

- Keep guard deterministic and lightweight.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

