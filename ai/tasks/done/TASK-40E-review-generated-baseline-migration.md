# TASK-40E

---
id: TASK-40E
title: Review generated SQL Server baseline migration
status: done
type: platform
team: database
supporting_teams:
  - backend
  - qa
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Review the generated `SqlServerInitialBaseline` migration for safety, completeness, and SQL Server compatibility.

## Context
The generated migration is not automatically trusted. It must be reviewed before an idempotent SQL script is generated for manual SSMS review.

## Implementation steps

1. Inspect the generated `Up` and `Down` methods.
2. Confirm all expected persisted entities and Identity tables are represented.
3. Confirm indexes exist for construction due orders, research due orders, shipyard due orders, planet ownership lookups, normalized names, and catalog keys where applicable.
4. Confirm decimal precision is explicit where needed.
5. Confirm string lengths are reasonable for SQL Server.
6. Confirm cascade deletes are intentional and do not imply unsafe gameplay data loss outside normal relational ownership.
7. Confirm no seed password, secret, real username, or full real connection string is present.
8. Document findings in `docs/dev/sql-server-migration-review.md`.

## Files to read first

- ai/architecture-index.md
- src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/
- src/VoidEmpires.Infrastructure/Persistence/Configurations/
- docs/dev/final-db-phase-readiness-report.md
- docs/dev/sql-server-migration-strategy.md

## Expected files to modify

- docs/dev/sql-server-migration-review.md

## Acceptance criteria

- The review document records whether the generated migration is complete and safe enough for SQL script generation.
- Required index, precision, string length, cascade, and secret checks are covered.
- Any discovered issue is either fixed within budget or split into a follow-up task.
- No migration is applied automatically.

## Constraints

- Do not change gameplay semantics.
- Do not connect to the real SQL Server.
- Do not run generated SQL against any database.
- Keep review findings factual and avoid claiming manual apply happened.

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

