# TASK-46B

---
id: TASK-46B
title: Remove continuation panels from modules
status: done
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Remove "Continuar mundo" and account continuation panels from authenticated module pages.

## Context
Account/session controls belong in shell, header, or account pages, not Construction, Research, Shipyard, or Defenses module bodies.

## Implementation steps

1. Inspect the four module pages and shared session/banner components.
2. Remove continuation/account panels from authenticated module bodies.
3. Ensure no Crear cuenta or Limpiar seleccion button remains inside these module pages.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx
- src/VoidEmpires.Frontend/src/components/PlanetModuleLayout.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/PlanetModuleLayout.tsx

## Acceptance criteria

- Continuar mundo panel is removed from Construction, Research, Shipyard and Defenses.
- No Crear cuenta button inside authenticated module pages.
- No Limpiar seleccion button inside authenticated module pages.
- Account/session controls remain outside module body.

## Constraints

- Do not remove authenticated sidebar.
- Do not remove top resource bar.
- Do not change public Login/Register shell split.

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
