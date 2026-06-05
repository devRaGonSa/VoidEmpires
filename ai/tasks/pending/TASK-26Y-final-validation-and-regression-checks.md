# TASK-26Y Final Validation And Regression Checks

---
id: TASK-26Y
title: Run final validation and regression checks for the lazy-loading block
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: high
---

## Purpose
Run the full repository validation sequence at the end of the lazy-loading block and confirm that no pending task files remain except `.gitkeep`.

## Current problem
The lazy-loading block affects shared frontend routing and build structure. Final validation must prove that the backend build, tests, frontend build, and QA scripts still pass together.

## Context from current implementation
The repository already has a defined validation cadence. This task serves as the final regression gate before closure of the block.

## Goal
Execute and record the final validation sequence, including any added route-lazy-import guard, and confirm the repository is in a clean task-queue state.

## Implementation steps
1. Run the full validation command set after all implementation and documentation tasks are complete.
2. Record test, build, and script outcomes.
3. Confirm `ai/tasks/pending` contains only `.gitkeep`.
4. Confirm the intended files are committed and the working tree is clean after commit.
5. Carry forward any known non-blocking warning only if it is documented.

## Files to inspect first
- ai/current-state.md
- docs/dev/frontend-foundation-smoke-checklist.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify
- ai/current-state.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements
- Run:
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- the route-lazy-import check if one was added
- Ensure:
- tests pass
- frontend build passes
- script checks pass
- no pending task files remain except `.gitkeep`
- the working tree is clean after the completion commit
- Do not run visual QA automatically.

## Frontend requirements
- None beyond preserving the accepted cockpit routes and successful build.

## Backend/API requirements
- None.

## Safety constraints
- Do not treat undocumented warnings as acceptable.
- Keep the task focused on validation and regression confirmation.

## Acceptance criteria
- Final validation is green.
- Any known non-blocking warning is documented clearly.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- run the route-lazy-import check if added

## Notes / residual risks
- Visual route QA remains user-driven. This task validates the repeatable command-based checks only.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
