# TASK-43I-remove-cabina-terminology

---
id: TASK-43I
title: Remove cabina terminology
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Remove `cabina` terminology from normal UI.

## Context
The word `cabina` makes the interface feel like an internal operator cockpit instead of a strategy game. Normal UI should use terms like Laboratorio, Investigacion, Astillero, Sistemas, Instalaciones, or Edificios depending on context.

## Implementation steps

1. Search frontend source for `cabina` and related phrases.
2. Replace `cabina de investigacion` with Laboratorio or Investigacion.
3. Replace `cabina de astillero` with Astillero.
4. Replace `cabinas especializadas` with Sistemas, Instalaciones, or Edificios according to context.
5. Remove `siguientes cabinas` entirely.
6. Expand copy regression guard so `cabina` fails in normal frontend UI while allowing docs/dev/operator exceptions if needed.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx
- src/VoidEmpires.Frontend/src/components/PlanetModuleLayout.tsx
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx
- src/VoidEmpires.Frontend/src/components/PlanetModuleLayout.tsx
- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Normal frontend UI no longer contains `cabina`.
- Copy guard fails if `cabina` returns to normal UI.
- Docs/dev/operator exceptions remain possible where intentional.

## Constraints

- Keep Spanish-first game terminology.
- Do not remove route functionality unless it is redundant and handled in a specific route task.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
