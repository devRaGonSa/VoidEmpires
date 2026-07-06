# TASK-40D1

---
id: TASK-40D1
title: Isolate SQL Server migration history for baseline generation
status: pending
type: platform
team: database
supporting_teams:
  - backend
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Ensure SQL Server baseline generation sees only SQL Server migration history and snapshot state, so `SqlServerInitialBaseline` can scaffold as an initial schema baseline instead of a PostgreSQL-to-SQL Server transition migration.

## Context
`TASK-40D` ran the documented SQL Server generation command, but EF Core reused the existing root `VoidEmpiresDbContextModelSnapshot`. The generated migration contained `DropIndex`, `DropPrimaryKey`, `Rename*`, and hundreds of `AlterColumn` operations from PostgreSQL column types into SQL Server column types, so it was rejected and removed.

The SQL Server output folder alone is not enough. The design-time migration services need an isolated SQL Server migration history/snapshot discovery path before generation is retried.

## Implementation steps

1. Inspect EF Core migration discovery for `VoidEmpiresDbContext` and the current root PostgreSQL migration snapshot.
2. Design the smallest repository-local mechanism that lets SQL Server design-time generation ignore root PostgreSQL migrations and use only `Persistence/Migrations/SqlServer`.
3. Preserve normal runtime defaults and ordinary PostgreSQL migration behavior.
4. Add focused tests if the chosen mechanism is testable without connecting to SQL Server.
5. Update `docs/dev/sql-server-migration-strategy.md` with the isolated migration-history approach.
6. Do not generate `SqlServerInitialBaseline` in this task unless the implementation is purely preparatory and the next task explicitly remains the generation retry.
7. Do not run `dotnet ef database update`.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- ai/orchestrator/di-analysis.md
- docs/dev/sql-server-migration-strategy.md
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs
- src/VoidEmpires.Infrastructure/Persistence/Migrations/VoidEmpiresDbContextModelSnapshot.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Persistence/
- tests/VoidEmpires.Tests/
- docs/dev/sql-server-migration-strategy.md

## Acceptance criteria

- SQL Server design-time migration generation has an isolated migration history/snapshot discovery path.
- PostgreSQL remains the default provider and existing root migrations are not removed or rewritten.
- The documented SQL Server generation command no longer reuses the root PostgreSQL-shaped snapshot.
- No real SQL Server credentials or full connection strings are committed or printed.
- No migration is applied automatically.

## Constraints

- Do not remove existing PostgreSQL/default migrations.
- Do not change gameplay behavior.
- Do not connect to the real SQL Server from tests.
- Keep the change minimal and provider-specific.

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
