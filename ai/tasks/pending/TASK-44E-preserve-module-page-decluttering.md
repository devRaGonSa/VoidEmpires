# TASK-44E-preserve-module-page-decluttering

---
id: TASK-44E
title: Preserve module page decluttering
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: medium
---

## Goal
Ensure restoring the sidebar does not reintroduce duplicated in-page navigation.

## Requirements

- Construction must remain a building catalog.
- Research must remain a technology grid.
- Shipyard must remain a ship production grid.
- Defense must remain a defense catalog.
- Ground Army must remain a unit catalog.
- Do not re-add cards linking to other modules inside each module page.
- Planet/Inicio may contain concise activity summaries and command overview only, not repeated navigation blocks everywhere.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- docs/dev/product-readiness-report.md

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
