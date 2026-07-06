# TASK-40N

---
id: TASK-40N
title: Update current state for SQL Server baseline migration
status: pending
type: platform
team: docs
supporting_teams:
  - database
roadmap_item: "SQL Server initial baseline migration"
priority: medium
---

## Goal
Update `ai/current-state.md` with the SQL Server baseline migration status.

## Context
The current state document must accurately reflect what has happened and what remains manual. It must not overclaim schema application or catalog seeding.

## Implementation steps

1. Record that `VoidEmpires_Dev` connection smoke passed manually before this block.
2. Record whether `SqlServerInitialBaseline` was generated, or deferred with the exact reason.
3. Record whether the idempotent SQL script was generated, or deferred with the exact reason.
4. Record that no automatic database apply occurred.
5. Record that no password was committed.
6. Record that schema apply remains manual and user-confirmed.
7. Record that catalog seed remains the next step after schema exists.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/dev/sql-server-migration-strategy.md
- docs/dev/sql-server-migration-review.md
- docs/dev/sql-server-runbook.md

## Expected files to modify

- ai/current-state.md

## Acceptance criteria

- `ai/current-state.md` accurately summarizes the SQL Server baseline status.
- The summary distinguishes generated, deferred, and manually-applied states.
- No real credentials or full real connection strings are recorded.
- No claim is made that migrations were applied automatically.

## Constraints

- Documentation-only change.
- No database connection.
- No schema or gameplay changes.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat` and verify the task stays within the change budget.
2. Run `git diff --name-only` and compare modified files with the expected files above.
3. Stage the intended files.
4. Commit with a clear message.
5. Push the branch if it has an upstream and this repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits.

