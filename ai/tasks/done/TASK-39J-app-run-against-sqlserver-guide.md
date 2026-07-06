# TASK-39J

---
id: TASK-39J
title: Document local app run against SQL Server
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Document how to run `VoidEmpires.Web` locally against SQL Server after the schema exists.

## Context

The app should remain default-safe, but users need exact local PowerShell commands for running against `VoidEmpires_Dev` after manual database preparation.

## Implementation steps

1. Read Web appsettings, provider configuration, and SQL Server runbook.
2. Document PowerShell commands to:
   - set `VoidEmpires__Persistence__Provider` to `SqlServer`;
   - set `ConnectionStrings__DefaultConnection` with placeholders;
   - run `dotnet run --project .\src\VoidEmpires.Web`.
3. Include a warning that schema must exist before running the app against SQL Server.
4. Describe expected health or development diagnostics behavior without overclaiming production readiness.
5. Include commands to clear environment variables and return to default development mode.
6. Do not include real passwords.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Web/appsettings.json
- src/VoidEmpires.Web/appsettings.Development.json
- docs/dev/sql-server-runbook.md
- src/VoidEmpires.Infrastructure/

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/final-db-phase-readiness-report.md

## Acceptance criteria

- The runbook shows exact local commands for running the Web project against SQL Server.
- The guide warns that schema must exist first.
- The guide explains how to clear local env vars.
- No real credentials are committed.

## Constraints

- Do not apply migrations.
- Do not connect to a real SQL Server.
- Do not change app provider defaults.
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
