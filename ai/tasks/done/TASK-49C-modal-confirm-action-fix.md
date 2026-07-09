# TASK-49C

---
id: TASK-49C
title: Modal confirm action fix
status: completed
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Ensure construction, research, shipyard, and defense confirm actions submit, close, and refresh.

## Context
Modals should not require a checkbox and successful actions should close review state after backend success.

## Implementation steps

1. Inspect construction, research, shipyard, and defense action flows.
2. Remove mandatory checkbox remnants.
3. Ensure success clears modal/review selection and refreshes authoritative state.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/GameModal.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx

## Acceptance criteria

- Buttons call backend, handle errors, close on success, and refresh state.
- Button labels match the requested Spanish copy.

## Constraints

- Do not change backend validation.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
