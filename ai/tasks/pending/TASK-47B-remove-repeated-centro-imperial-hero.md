# TASK-47B

---
id: TASK-47B
title: Remove repeated Centro imperial hero
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: high
---

## Goal
Remove the repeated "Centro imperial" container from authenticated module pages.

## Context
Core module pages should be direct OGame-like screens with the authenticated sidebar and top resource bar, compact module title, optional active queue, and one catalog grid.

## Implementation steps

1. Find the shared component or copy that renders "Centro imperial".
2. Remove the repeated large hero/card above Construction, Research, Shipyard, Defenses, and other safe authenticated module pages using it.
3. Keep each module's compact title/header.
4. Preserve sidebar and top resource bar.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx

## Acceptance criteria

- The repeated "Centro imperial" hero is gone from module pages.
- The authenticated game shell remains intact.

## Constraints

- Do not remove sidebar.
- Do not remove top resource bar.
- Do not change public login/register pages.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1

