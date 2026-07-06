# TASK-40P

---
id: TASK-40P
title: Close SQL Server baseline migration block
status: pending
type: platform
team: platform
supporting_teams:
  - database
  - qa
  - devops
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Close the SQL Server baseline migration block after all tasks from `TASK-40A` through `TASK-40P` are complete.

## Context
This task is the closure checkpoint for the block. It should move completed tasks to `ai/tasks/done`, verify that pending contains only `.gitkeep`, commit and push the final state, and summarize the validated outcome.

## Implementation steps

1. Confirm `TASK-40A` through `TASK-40O` are complete and committed.
2. Move `TASK-40A` through `TASK-40P` to `ai/tasks/done`.
3. Confirm `ai/tasks/pending` contains only `.gitkeep`.
4. Run the final validation commands listed below.
5. Commit and push the final state.
6. Output the commit list, validation results, test count, whether `SqlServerInitialBaseline` was generated, whether the idempotent SQL script was generated, the exact script generation command, the manual SSMS apply checklist path, and explicit statements that no real password was committed and no migration was applied automatically.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- ai/tasks/pending/TASK-40A-sqlserver-baseline-migration-audit.md
- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-user-checklist.md
- docs/dev/sql-server-migration-strategy.md

## Expected files to modify

- ai/tasks/pending/
- ai/tasks/done/
- ai/current-state.md

## Acceptance criteria

- `TASK-40A` through `TASK-40P` are moved to `ai/tasks/done`.
- `ai/tasks/pending` contains only `.gitkeep`.
- Final validation commands pass, or any blocker is recorded honestly before stopping.
- Final output includes the required block summary and safety statements.
- No secrets are committed and no automatic SQL Server apply occurs.

## Constraints

- Do not run `dotnet ef database update`.
- Do not execute generated SQL automatically.
- Do not push if validation fails unless the failure and reason are explicitly documented and the repository workflow permits it.

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

1. Run `git diff --stat` and verify the task stays within the change budget.
2. Run `git diff --name-only` and compare modified files with the expected files above.
3. Stage the intended files.
4. Commit with a clear message.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files per implementation task.
- Prefer changes under 200 lines of code per implementation task.
- Prefer fewer than 3 commits per implementation task.
