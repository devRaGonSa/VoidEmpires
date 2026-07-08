# TASK-46F

---
id: TASK-46F
title: Remove module version and status pills
status: done
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Remove player-facing module version/status pills from the four core modules.

## Context
Internal version/status labels are not useful player-facing UI in Construction, Research, Shipyard, or Defenses.

## Implementation steps

1. Inspect module header props and badges.
2. Remove Construccion v1, Investigacion v1, Astillero v1, Defensas v1, Vista normalizada, Contexto conservado, and redundant Cuenta activa badges inside module body.
3. Keep useful Spanish player-facing title and description.

## Files to read first

- src/VoidEmpires.Frontend/src/components/PlanetModuleLayout.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/PlanetModuleLayout.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx

## Acceptance criteria

- Module version/status pills listed in the goal are removed from the four core modules.
- Useful player-facing titles and descriptions remain.

## Constraints

- Do not remove shell/sidebar/resource bar status that belongs outside module bodies.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git status`.
2. Stage intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
