# TASK-41AN

---
id: TASK-41AN
title: Copy regression product surface guard
status: pending
type: tooling
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Expand the copy regression guard to fail if forbidden dev/test terms appear in normal frontend UI.

## Context
Automated copy guard coverage should prevent normal UI from reintroducing development/test/prototype language.

## Implementation steps

1. Inspect the existing copy regression script and its allowlist behavior.
2. Add product-surface checks for Development, Dev, QA, Test, Prueba, Prototipo, Solo desarrollo, cockpit-validation, minimal-validation, endpoint, and localhost.
3. Allow exceptions only for docs, scripts, and operator-only code where needed.
4. Avoid false positives for route names or identifiers that are not user-facing.
5. Run the guard and frontend build.

## Files to read first

- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- docs/dev/product-mode-visibility-contract.md

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/

## Acceptance criteria

- Copy regression guard fails on forbidden terms in normal frontend UI.
- Legitimate docs/scripts/operator-only exceptions are allowed narrowly.
- Guard and frontend build pass.

## Constraints

- Do not weaken existing copy checks.
- Do not create broad allowlists that hide product UI violations.
- Keep route names/imports working.

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`

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
