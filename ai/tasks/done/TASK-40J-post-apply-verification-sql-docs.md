# TASK-40J

---
id: TASK-40J
title: Add post-apply verification SQL snippets
status: done
type: platform
team: database
supporting_teams:
  - docs
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Prepare safe read-only SQL snippets for post-apply verification.

## Context
After a human manually applies the SQL Server baseline script, the user needs read-only checks to confirm the expected schema and migration-history state.

## Implementation steps

1. Add `docs/dev/sql-server-post-apply-verification.sql` or a similarly named SQL document.
2. Include safe read-only queries to list tables.
3. Include a read-only query against `__EFMigrationsHistory`.
4. Include count queries for core tables that are safe when tables are empty.
5. Include index-verification queries for important lookup and due-order indexes.
6. Include catalog table verification queries if catalog tables are created by the baseline.
7. Do not include writes, destructive SQL, passwords, or real connection strings.

## Files to read first

- ai/architecture-index.md
- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-migration-review.md
- src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/
- src/VoidEmpires.Infrastructure/Persistence/Configurations/

## Expected files to modify

- docs/dev/sql-server-post-apply-verification.sql
- docs/dev/sql-server-runbook.md

## Acceptance criteria

- A read-only post-apply SQL verification file exists.
- The SQL checks tables, migration history, core table counts, key indexes, and catalog tables where applicable.
- The SQL contains no writes or destructive statements.
- The runbook links to or references the verification file.

## Constraints

- Do not execute the SQL file.
- Do not include credentials.
- Do not claim the schema has been applied.

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

