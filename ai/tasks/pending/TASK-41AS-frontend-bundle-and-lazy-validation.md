# TASK-41AS

---
id: TASK-41AS
title: Frontend bundle and lazy validation
status: pending
type: validation
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Validate frontend build, route lazy loading, and bundle budget.

## Context
Product-facing copy and layout changes must preserve the lazy route architecture and keep the entry chunk acceptable.

## Implementation steps

1. Run the frontend build.
2. Run the lazy route guard.
3. Review Vite output for entry chunk size and route chunks.
4. Fix only regressions caused by Block 41 changes.
5. Record validation results in the closure task if appropriate.

## Files to read first

- scripts/check-frontend-route-lazy-imports.ps1
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/package.json

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- scripts/check-frontend-route-lazy-imports.ps1

## Acceptance criteria

- Frontend build passes.
- Lazy route guard passes.
- Entry chunk remains acceptable relative to the existing baseline.

## Constraints

- Preserve lazy loading.
- Do not add final assets.
- Do not introduce broad route imports.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
