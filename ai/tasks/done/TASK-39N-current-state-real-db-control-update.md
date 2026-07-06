# TASK-39N

---
id: TASK-39N
title: Update current state for controlled real database preparation
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: medium
---

## Goal

Update `ai/current-state.md` with the controlled SQL Server creation status without overclaiming.

## Context

The repository should record that SQL Server real DB creation is prepared, but user action remains manual and no automatic migration apply has occurred.

## Implementation steps

1. Read `ai/current-state.md` and the SQL Server runbook/checklist.
2. State that SQL Server real DB creation is prepared.
3. State that the recommended database name is `VoidEmpires_Dev`.
4. State that no migration has been applied automatically.
5. State that no password is committed.
6. State that manual user action remains required.
7. State that optional smoke scripts exist and remain opt-in.
8. Record validation status based on the commands run in this task.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-user-checklist.md
- docs/dev/final-db-phase-readiness-report.md

## Expected files to modify

- ai/current-state.md

## Acceptance criteria

- `ai/current-state.md` accurately reflects controlled SQL Server readiness.
- The update does not claim that migrations were applied or that the real DB exists.
- Validation status is recorded.
- No secrets are added.

## Constraints

- Do not connect to SQL Server.
- Do not apply migrations.
- Do not overclaim readiness.
- Keep user-facing references Spanish-first where applicable.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `git diff --stat`
- `git diff --name-only`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote if the branch is configured for remote collaboration.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits.
