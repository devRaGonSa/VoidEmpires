# TASK-40D

---
id: TASK-40D
title: Generate SQL Server initial baseline migration
status: pending
type: platform
team: database
supporting_teams:
  - backend
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Generate the SQL Server initial EF Core migration files named `SqlServerInitialBaseline`.

## Context
The migration script helper is currently blocked because SQL Server migration files do not exist under the provider-specific folder. This task creates the migration files only; it must not apply them.

## Implementation steps

1. Run the documented command from `docs/dev/sql-server-migration-strategy.md`.
2. Generate the migration named `SqlServerInitialBaseline`.
3. Ensure the output folder is `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`.
4. Do not connect to the real SQL Server.
5. Do not run `dotnet ef database update`.
6. Inspect the generated files for provider correctness, absence of secrets, absence of real connection strings, and lack of destructive data operations outside initial schema creation.
7. If EF migration generation fails, document the exact reason, create a narrowed follow-up task, and do not fake migration files.

## Files to read first

- ai/architecture-index.md
- docs/dev/sql-server-migration-strategy.md
- src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs
- src/VoidEmpires.Infrastructure/Persistence/Migrations/VoidEmpiresDbContextModelSnapshot.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/
- docs/dev/sql-server-migration-strategy.md
- ai/tasks/pending/

## Acceptance criteria

- `SqlServerInitialBaseline` migration files exist under the SQL Server migration folder, or the failure is honestly documented with a narrowed follow-up task.
- Generated files are provider-correct for SQL Server.
- Generated files contain no secrets, real usernames, passwords, or full real connection strings.
- No migration is applied automatically.

## Constraints

- Do not remove existing PostgreSQL/default migrations unless explicitly proven safe and necessary.
- Do not modify gameplay behavior.
- Do not add seed data containing credentials or production auth.
- Keep generated artifacts reviewable.

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

