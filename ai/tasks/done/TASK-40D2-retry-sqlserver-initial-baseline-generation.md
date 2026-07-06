# TASK-40D2

---
id: TASK-40D2
title: Retry SQL Server initial baseline migration generation
status: done
type: platform
team: database
supporting_teams:
  - backend
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Regenerate `SqlServerInitialBaseline` after SQL Server design-time migration history isolation is in place.

## Context
`TASK-40D` rejected the first generated migration because EF reused the root PostgreSQL-shaped snapshot. `TASK-40D1` added a SQL Server-specific design-time migrations assembly that should report no migrations and no snapshot until SQL Server migrations exist.

This task retries generation and accepts the files only if they scaffold as an initial SQL Server schema baseline.

## Implementation steps

1. Run the documented command from `docs/dev/sql-server-migration-strategy.md`.
2. Generate the migration named `SqlServerInitialBaseline`.
3. Ensure files are created under `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`.
4. Confirm the root PostgreSQL migration folder and root `VoidEmpiresDbContextModelSnapshot` are not modified.
5. Inspect the generated migration for initial-schema `CreateTable`/`CreateIndex` shape, SQL Server provider metadata, absence of secrets, and absence of real connection strings.
6. If generation still produces a provider-transition migration or modifies root PostgreSQL artifacts, remove the scaffold, document the exact reason, and create one narrowed follow-up task.
7. Do not run `dotnet ef database update`.

## Files to read first

- ai/architecture-index.md
- docs/dev/sql-server-migration-strategy.md
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs
- src/VoidEmpires.Infrastructure/Persistence/SqlServerDesignTimeMigrationsAssembly.cs
- src/VoidEmpires.Infrastructure/Persistence/Migrations/VoidEmpiresDbContextModelSnapshot.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/
- docs/dev/sql-server-migration-strategy.md
- docs/dev/sql-server-migration-review.md
- ai/tasks/pending/

## Acceptance criteria

- `SqlServerInitialBaseline` migration files exist under the SQL Server migration folder, or any remaining failure is honestly documented with a narrowed follow-up task.
- The generated migration is an initial SQL Server schema baseline, not a PostgreSQL-to-SQL Server transition migration.
- The root PostgreSQL migration files and root snapshot are unchanged.
- Generated files contain no secrets, real usernames, passwords, or full real connection strings.
- No migration is applied automatically.

## Constraints

- Do not remove existing PostgreSQL/default migrations.
- Do not modify gameplay behavior.
- Do not connect to the real SQL Server.
- Do not run generated SQL against any database.

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
