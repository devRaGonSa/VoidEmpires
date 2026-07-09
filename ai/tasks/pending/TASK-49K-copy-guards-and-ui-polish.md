# TASK-49K

---
id: TASK-49K
title: Copy guards and UI polish
status: pending
type: tooling
team: frontend
supporting_teams: []
roadmap_item: "Block 49"
priority: medium
---

## Goal
Extend copy guard for Block 49 regressions.

## Context
The UI should not reintroduce construction-queue dependency copy, unit-defense level copy, checkbox copy, empty Home queue cards, or normal-UI dev materialize wording.

## Implementation steps

1. Inspect existing copy guard.
2. Add scoped checks for Block 49 forbidden copy.
3. Run guard and frontend build.

## Files to read first

- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Guard blocks requested copy regressions without breaking docs/dev.

## Constraints

- No false failures in docs/dev.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
