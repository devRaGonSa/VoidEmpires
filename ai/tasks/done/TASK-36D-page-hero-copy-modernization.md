# TASK-36D

---
id: TASK-36D
title: Page hero copy modernization
status: done
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Replace obsolete generic prototype hero copy with compact current-state page headers.

## Context
Some pages still say the frontend is conservative/read-only even though Construction, Research, Shipyard, resource materialization, and queue materialization now perform controlled Development mutations.

## Implementation steps

1. Inspect hero/header blocks on Planet, Construction, Research, Shipyard, Defenses, and Fleets.
2. Remove or replace obsolete copy such as:
   - `Superficie de mando solo para desarrollo`;
   - `Las rutas actuales del frontend siguen siendo conservadoras y no ejecutan mutaciones...`.
3. Use concise accurate copy:
   - mutating Development pages: `Mutaciones Development confirmadas`;
   - read-only pages: `Lectura / readiness`;
   - diagnostics remain secondary only.
4. Avoid repeating long explanations on every page.
5. Keep Spanish-first copy.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx

## Acceptance criteria

- Obsolete prototype messaging is removed from primary UI.
- Page headers are shorter and accurate.
- Read-only labels remain only where true.
- Frontend build and copy guard pass.

## Constraints

- Do not add gameplay behavior.
- Do not hide Development scope for mutating tools.
- Do not perform or claim full visual QA.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36D message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
