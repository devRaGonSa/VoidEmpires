# TASK-40I

---
id: TASK-40I
title: Document manual SSMS apply runbook
status: pending
type: platform
team: docs
supporting_teams:
  - database
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Create the exact manual SSMS application runbook for the generated SQL Server script.

## Context
Schema application must remain a human-controlled action. The docs should guide the user through backup, review, manual execution, and read-only verification without claiming the migration was applied.

## Implementation steps

1. Update `docs/dev/sql-server-user-checklist.md`.
2. Update `docs/dev/sql-server-runbook.md`.
3. Include steps to back up `VoidEmpires_Dev`, open the generated SQL script in SSMS, verify the target database, execute manually only after review, inspect tables, query `__EFMigrationsHistory`, and rerun the read-only smoke.
4. Keep all examples free of passwords and full real connection strings.
5. Explicitly state that the repository scripts do not apply the migration automatically.
6. Do not claim the migration has been applied.

## Files to read first

- ai/architecture-index.md
- docs/dev/sql-server-user-checklist.md
- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-migration-strategy.md
- scripts/sqlserver-connection-smoke.ps1

## Expected files to modify

- docs/dev/sql-server-user-checklist.md
- docs/dev/sql-server-runbook.md

## Acceptance criteria

- The manual SSMS apply checklist is complete and ordered.
- Backup, target database verification, manual execution, table inspection, migration-history check, and read-only smoke are included.
- No secrets or real full connection strings are committed.
- The docs do not claim an apply occurred.

## Constraints

- Do not run SSMS or execute SQL.
- Do not run `dotnet ef database update`.
- Do not modify scripts unless a documentation reference is broken.

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

