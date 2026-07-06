# TASK-40A

---
id: TASK-40A
title: Audit SQL Server baseline migration setup
status: done
type: platform
team: database
supporting_teams:
  - platform
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Audit the current EF Core migration and provider setup and decide the exact safe method to create `SqlServerInitialBaseline`.

## Context
SQL Server connectivity has already been manually smoke-tested, but no migrations have been applied and no schema has been created. The current blocker is that `scripts/sqlserver-script-migration.ps1` refuses to run until SQL Server baseline migration files exist under `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`.

This task is an audit and documentation step only unless a strictly documentation-only adjustment is required.

## Implementation steps

1. Inspect the existing migration folders and determine whether current migrations are provider-specific or provider-neutral.
2. Identify the `DbContext` used for migrations and the design-time factory behavior.
3. Confirm the output folder convention for SQL Server migrations.
4. Confirm the target folder is `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`.
5. Confirm the migration name is `SqlServerInitialBaseline`.
6. Document the safe generation approach and risks around provider mixing, accidental real database apply, default provider expectations, and generated SQL review.
7. Do not generate migrations, run `dotnet ef database update`, or connect to the real SQL Server in this task.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- docs/dev/sql-server-migration-strategy.md
- scripts/sqlserver-script-migration.ps1
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs

## Expected files to modify

- docs/dev/sql-server-migration-strategy.md

## Acceptance criteria

- The migration/provider setup is documented with the current folder layout and provider assumptions.
- The target SQL Server migration folder and migration name are explicitly recorded.
- Risks are documented without overclaiming that a migration or SQL script has been generated.
- No runtime behavior changes are introduced.
- No secrets, real usernames, real passwords, or full real connection strings are added.

## Constraints

- Keep default provider behavior unchanged: PostgreSQL remains the default provider.
- Do not remove or rewrite existing migrations.
- Do not run or document any automatic database apply command.
- Keep the change docs-only unless a documentation typo needs correction.

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

