# TASK-41AT

---
id: TASK-41AT
title: Final product surface validation
status: done
type: validation
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Run full automated validation.

## Context
This task is the automated gate before closure tasks. It should not perform browser/manual QA.

## Implementation steps

1. Run backend build and tests.
2. Run frontend build.
3. Run the dev/QA scripts guard, lazy route guard, frontend copy guard, and repo secret scan.
4. Run `git status`.
5. Fix only regressions caused by Block 41 changes or create follow-up tasks if the change budget is exceeded.

## Files to read first

- ai/current-state.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- scripts/check-repo-secret-scan.ps1

## Expected files to modify

- ai/current-state.md
- docs/dev/
- scripts/
- src/VoidEmpires.Frontend/

## Acceptance criteria

- All required automated validation commands pass.
- Validation output is recorded for closure if appropriate.
- No browser/manual QA is claimed.

## Constraints

- Do not apply migrations automatically.
- Do not require SQL Server for ordinary tests.
- Do not commit secrets.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`
- `git status`

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
