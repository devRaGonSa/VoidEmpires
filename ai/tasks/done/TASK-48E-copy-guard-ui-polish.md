# TASK-48E

---
id: TASK-48E
title: Copy guard UI polish
status: done
type: tooling
team: frontend
supporting_teams: []
roadmap_item: "Block 48"
priority: medium
---

## Goal
Evitar que vuelva la UI incorrecta.

## Context
El copy guard debe bloquear regresiones concretas del polish UI de Block 48 sin romper textos validos en docs/dev.

## Implementation steps

1. Revisar el copy guard actual.
2. Extender patrones para checkbox obligatorio, colas vacias de Inicio y "Nivel 0 -> 1" en defensas unitarias si procede.
3. Ejecutar el guard.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1
- ai/tasks/pending/TASK-48E-copy-guard-ui-polish.md

## Acceptance criteria

- El guard evita checkbox obligatorio, "Sin movimientos de flota" en Inicio, "Sin produccion orbital" en Inicio, "Sin defensas en cola" en Inicio y "Nivel 0 -> 1" en defensas unitarias.
- No se rompen textos validos en docs/dev.

## Constraints

- No tocar login/register.
- No generar imagenes.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
