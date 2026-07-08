# TASK-45N

---
id: TASK-45N
title: Final validation and closure
status: done
type: release
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Close Block 45.

## Context
All TASK-45 items must be moved to done, pending must contain only .gitkeep, and final validation must pass before the final state is pushed.

## Implementation steps

1. Confirm Construction, Research, Shipyard and Defenses each show queue plus catalog only.
2. Confirm each catalog has at least four visible entries.
3. Confirm desktop grids target four cards per row.
4. Confirm no selection, context, manual loader, raw id, or redundant resource blocks remain in normal UI.
5. Run the full final validation suite.
6. Move all TASK-45 files to ai/tasks/done and leave pending with only .gitkeep.
7. Commit and push final state.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- ai/tasks/in-progress
- ai/tasks/done
- src/VoidEmpires.Frontend/src

## Expected files to modify

- ai/tasks/done
- ai/tasks/in-progress
- ai/tasks/pending

## Acceptance criteria

- All TASK-45 files are in ai/tasks/done.
- ai/tasks/pending contains only .gitkeep.
- Build, test, frontend and guard scripts pass.
- Final state is committed and pushed.

## Constraints

- Do not remove authenticated sidebar
- Do not remove top resource bar
- Do not claim manual or browser QA
- Do not commit secrets

## Validation

Before completing the task ensure:

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1
- git status
- dir ai/tasks/pending

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
