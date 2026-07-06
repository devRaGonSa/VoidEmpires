# TASK-40B

---
id: TASK-40B
title: Support safe design-time SQL Server migration generation
status: done
type: platform
team: database
supporting_teams:
  - backend
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Ensure EF Core design-time migration generation can target SQL Server without requiring or using a real SQL Server connection.

## Context
The SQL Server baseline migration must be generated for review, but generation must not depend on the user's real database, username, password, or connection string. Runtime defaults must remain unchanged.

## Implementation steps

1. Review the design-time `VoidEmpiresDbContextFactory` and provider selection conventions.
2. Ensure a safe design-time configuration can select the SQL Server provider for migration generation.
3. Use only a placeholder or local-only connection string for design-time metadata.
4. Confirm migration generation does not print or require any password.
5. Preserve runtime default behavior and PostgreSQL test behavior.
6. Add or update focused tests for design-time provider selection if feasible without making tests depend on SQL Server.
7. Do not run `dotnet ef database update` or apply migrations.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- ai/orchestrator/di-analysis.md
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
- tests/VoidEmpires.Tests/TestWebApplicationFactoryExtensions.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs
- tests/VoidEmpires.Tests/
- docs/dev/sql-server-migration-strategy.md

## Acceptance criteria

- EF design-time creation can select SQL Server using safe explicit configuration.
- Design-time SQL Server selection does not require the real database server to be reachable.
- No real SQL Server credentials or full connection strings are committed or printed.
- Runtime defaults and ordinary tests continue to use the existing default provider behavior.
- Any tests added are provider-selection tests only and do not connect to SQL Server.

## Constraints

- Default provider remains `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Explicit SQL Server provider remains supported.
- Do not change gameplay semantics or add gameplay features.
- Do not connect to the real SQL Server from tests.

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

