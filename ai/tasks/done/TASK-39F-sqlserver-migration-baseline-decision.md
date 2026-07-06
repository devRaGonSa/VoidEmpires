# TASK-39F

---
id: TASK-39F
title: Decide SQL Server baseline migration readiness
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Decide and document whether the repository is ready to generate a SQL Server baseline migration named `SqlServerInitialBaseline`.

## Context

Before creating migration files, the provider setup and existing migrations must be inspected. If generation is unsafe, the decision must be documented and deferred with exact blockers.

## Implementation steps

1. Read existing EF Core migrations, provider configuration, and SQL Server migration strategy.
2. Inspect how PostgreSQL and explicit SQL Server provider selection are wired.
3. Determine whether generating `SqlServerInitialBaseline` is safe now.
4. If safe, document the named baseline migration plan and the manual command path.
5. If unsafe, document exact blockers and keep generation deferred.
6. Do not generate, apply, or run a migration in this task.
7. Do not connect to the real SQL Server.
8. Update `docs/dev/sql-server-migration-strategy.md` with the decision.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- ai/orchestrator/di-analysis.md
- docs/dev/sql-server-migration-strategy.md
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Expected files to modify

- docs/dev/sql-server-migration-strategy.md
- docs/dev/sql-server-migration-dry-run.md

## Acceptance criteria

- The repo has a documented yes/no decision for `SqlServerInitialBaseline`.
- If safe, the plan names `SqlServerInitialBaseline` and explains the generation path.
- If unsafe, blockers are specific and actionable.
- No migration is generated or applied in this task.

## Constraints

- Do not connect to a real SQL Server.
- Do not apply migrations.
- Do not change gameplay semantics or provider defaults.
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
