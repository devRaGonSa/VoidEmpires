# TASK-39P

---
id: TASK-39P
title: Close controlled SQL Server creation block
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Close Block 39A-39P after all prior tasks are complete, with final validation results and a clear user-facing handoff.

## Context

This is the final closure task. It must confirm the pending queue is clean, move completed tasks to `ai/tasks/done`, commit and push final state, and produce a concise closure summary for the user.

## Implementation steps

1. Verify TASK-39A through TASK-39O are completed and moved to `ai/tasks/done`.
2. Complete TASK-39P and move it to `ai/tasks/done`.
3. Ensure `ai/tasks/pending` contains only `.gitkeep`.
4. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
   - `git status`
   - `dir ai\tasks\pending`
5. Run `scripts/check-repo-secret-scan.ps1` separately if it is not included by the QA script.
6. Commit and push the final state.
7. Output:
   - commit list;
   - validation results;
   - test count;
   - exact user checklist file;
   - exact smoke command template;
   - exact run command template;
   - explicit statement that no real password was committed;
   - explicit statement that no migration was applied automatically to SQL Server.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/dev/sql-server-user-checklist.md
- docs/dev/sql-server-runbook.md
- docs/dev/final-db-phase-readiness-report.md
- ai/tasks/done/

## Expected files to modify

- ai/tasks/in-progress/TASK-39P-final-controlled-sqlserver-creation-closure.md
- ai/tasks/done/TASK-39P-final-controlled-sqlserver-creation-closure.md
- ai/current-state.md
- docs/dev/final-db-phase-readiness-report.md

## Acceptance criteria

- TASK-39A through TASK-39P are in `ai/tasks/done`.
- `ai/tasks/pending` contains only `.gitkeep`.
- Final validation passes.
- Final output includes all required closure details.
- No real password is committed and no migration is applied automatically.

## Constraints

- Do not connect to or mutate a real SQL Server unless a preceding task explicitly added an opt-in smoke and the user provided local credentials outside the repo.
- Do not apply migrations automatically.
- Do not commit secrets.
- Do not overclaim production readiness.

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
- `dir ai\tasks\pending`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits.
