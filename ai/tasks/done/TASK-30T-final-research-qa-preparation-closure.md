# TASK-30T

---
id: TASK-30T
title: Final research QA preparation closure
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 30Q-30T - Research manual QA state preparation"
priority: medium
---

## Goal
Close the research QA preparation block, update the repository current state, and verify the final validated Development-only manual QA preparation path.

## Context
After the contract, endpoint, and helper are complete, the repository needs its task lifecycle updated, the final state documented, and the full validation suite re-run before the block is considered closed.

## Implementation steps

1. Update `ai/current-state.md` with the research enqueue acceptance, the reused-DB blocker, and the new Development-only preparation path.
2. Move `TASK-30Q` through `TASK-30T` into `ai/tasks/done` and ensure `ai/tasks/pending` returns to `.gitkeep`.
3. Run the final validation bundle, commit, and push the final block state.

## Files to read first

- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/research-cockpit-checklist.md
- scripts/check-dev-qa-scripts.ps1

## Expected files to modify

- ai/tasks/in-progress/TASK-30T-final-research-qa-preparation-closure.md
- ai/current-state.md
- ai/tasks/done/TASK-30Q-research-qa-state-preparation-contract.md
- ai/tasks/done/TASK-30R-research-qa-state-preparation-endpoint-and-tests.md
- ai/tasks/done/TASK-30S-research-qa-preparation-powershell-helper.md
- ai/tasks/done/TASK-30T-final-research-qa-preparation-closure.md

## Acceptance criteria

- The current state reflects the accepted research enqueue posture and the remaining user-driven manual success-path QA.
- `ai/tasks/pending` contains only `.gitkeep`.
- Final build, test, frontend build, and script checks all pass.
- The working tree is clean after commit and push.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

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
