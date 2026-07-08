# TASK-47C

---
id: TASK-47C
title: Guard against Centro imperial copy returning
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: high
---

## Goal
Prevent removed generic module hero copy from returning to normal frontend UI.

## Context
The copy regression guard protects product-facing frontend pages from old internal or generic shell copy.

## Implementation steps

1. Extend the frontend copy regression guard to reject "Centro imperial".
2. Reject "Sistemas de colonia, investigacion, astillero y flotas".
3. Reject "lectura de estado y confirmaciones explicitas".
4. Keep docs/dev exceptions only if needed.

## Files to read first

- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Guard fails on the removed generic hero copy in normal UI files.
- Current frontend copy passes the guard.

## Constraints

- Do not block legitimate documentation if the guard already excludes docs.

## Validation

- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
- npm run build --prefix src/VoidEmpires.Frontend
