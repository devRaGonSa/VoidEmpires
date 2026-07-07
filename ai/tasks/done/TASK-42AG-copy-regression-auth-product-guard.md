# TASK-42AG-copy-regression-auth-product-guard

---
id: TASK-42AG
title: Copy regression auth product guard
status: done
type: tooling
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Expand copy regression checks for removed local-game and development concepts.

## Context
Normal UI must not present local-session/new-game or development seed language as product copy. Docs/dev and operator-only exceptions may remain.

## Implementation steps

1. Review the existing copy regression script and exception model.
2. Add forbidden normal UI terms: `partida local`, `sesion local` as a primary product term, `new local game`, `Development-safe`, `seed`, and `cockpit-validation`.
3. Preserve docs/dev and operator-only exceptions where explicitly intended.
4. Run the guard and frontend build.

## Files to read first

- scripts/check-frontend-copy-regressions.ps1
- docs/dev/cockpit-copy-guidelines.md
- docs/dev/product-mode-visibility-contract.md
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1
- docs/dev/cockpit-copy-guidelines.md

## Acceptance criteria

- Copy guard fails for the listed forbidden normal UI terms.
- Legitimate docs/dev and operator exceptions remain allowed.
- Guard passes on the current codebase.

## Constraints

- Avoid broad false positives that block docs/dev.
- Keep script output actionable.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
