# TASK-39G

---
id: TASK-39G
title: Optionally generate SQL Server baseline migration
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Generate the SQL Server baseline migration only if TASK-39F proves it is safe; otherwise close the task as a documented deferral.

## Context

This task depends on the decision recorded in TASK-39F. It must not apply migrations or connect to the real database.

## Implementation steps

1. Read the TASK-39F result in the migration strategy docs.
2. If TASK-39F says baseline generation is safe:
   - generate migration files for `SqlServerInitialBaseline` using the repository's established EF Core conventions;
   - ensure the migration targets SQL Server provider setup without changing the default provider;
   - verify build and tests.
3. If TASK-39F says baseline generation is unsafe:
   - do not generate migration files;
   - document the deferred state and exact manual command that should be used later once blockers are resolved.
4. Do not run `update-database`.
5. Do not connect to the real SQL Server.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- ai/orchestrator/di-analysis.md
- docs/dev/sql-server-migration-strategy.md
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Expected files to modify

- src/VoidEmpires.Infrastructure/
- docs/dev/sql-server-migration-strategy.md
- docs/dev/sql-server-migration-dry-run.md

## Acceptance criteria

- If safe, `SqlServerInitialBaseline` migration files exist and build/tests pass.
- If unsafe, no migration files are added and the deferral is clearly documented.
- No migration is applied to any SQL Server.
- No real connection string or password is introduced.

## Constraints

- Do not connect to a real SQL Server.
- Do not apply migrations.
- Do not require SQL Server for normal tests.
- Preserve default provider behavior.
- Keep user-facing docs Spanish-first.

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
