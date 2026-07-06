# TASK-40C

---
id: TASK-40C
title: Document SQL Server baseline generation command
status: done
type: platform
team: docs
supporting_teams:
  - database
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Document the exact command used or intended to generate the `SqlServerInitialBaseline` migration.

## Context
Before generating migration files, the repository needs a reviewable command path that future agents can run consistently without applying changes to a real database.

## Implementation steps

1. Update `docs/dev/sql-server-migration-strategy.md` with the precise migration generation command.
2. Include any required provider environment variable or design-time provider selector command.
3. Include the exact output folder: `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`.
4. Include the exact migration name: `SqlServerInitialBaseline`.
5. Add an explicit warning not to run `dotnet ef database update`.
6. Confirm the command uses safe placeholder/local design-time connection metadata only.
7. Do not generate the migration in this task.

## Files to read first

- ai/architecture-index.md
- docs/dev/sql-server-migration-strategy.md
- docs/dev/sql-server-migration-dry-run.md
- src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs

## Expected files to modify

- docs/dev/sql-server-migration-strategy.md

## Acceptance criteria

- The migration generation command is exact enough to copy and run from the repository root.
- Provider selection, output folder, context, and project/startup project arguments are documented if required.
- The warning against automatic database apply is prominent.
- No secrets, real usernames, real passwords, or full real connection strings are documented.

## Constraints

- Do not run the generation command in this task.
- Do not change code unless the documentation reveals an obvious typo in existing command examples.
- Do not change provider defaults.

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

