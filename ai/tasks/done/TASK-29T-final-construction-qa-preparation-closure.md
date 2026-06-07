# TASK-29T

---
id: TASK-29T
title: Finalize construction manual QA state-preparation block and closeout
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 29Q-29T"
priority: medium
---

## Goal
Close Block 29Q-29T after adding endpoint, tests, helper, and docs.

## Context
The block completes the manual QA path for `/construction` on shared Development databases with a scoped, non-production preparation mechanism.

## Implementation steps

1. Move Block 29Q�29T task files from pending through done.
2. Update `ai/current-state.md` with the accepted manual QA note and exact command sequence.
3. Execute the required validation set and collect results:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
4. Confirm pending tasks contain only `.gitkeep` and clean working tree.

## Files to read first

- `ai/current-state.md`
- `ai/tasks`
- `ai/orchestrator/*`
- outputs from the required validation commands

## Expected files to modify

- `ai/tasks/in-progress/TASK-29Q...` through done
- `ai/tasks/pending`
- `ai/current-state.md`

## Acceptance criteria

- Block is closed with complete command list and validation results.
- `ai/tasks/pending` contains only `.gitkeep`.

## Validation

- Required command list in Validation section.
- Working tree ready for commit.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

