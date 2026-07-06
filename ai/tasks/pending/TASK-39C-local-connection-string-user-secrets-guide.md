# TASK-39C

---
id: TASK-39C
title: Document local SQL Server connection string setup
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Document exact local commands for configuring SQL Server access without committing secrets.

## Context

The user needs a safe local setup path for SQL Server Authentication while keeping usernames, passwords, and full real connection strings out of the repository and chat logs.

## Implementation steps

1. Read the current appsettings files, SQL Server runbook, and security notes.
2. Document the PowerShell environment variable method:
   - `$env:VoidEmpires__Persistence__Provider="SqlServer"`
   - `$env:ConnectionStrings__DefaultConnection="Server=192.168.178.28,1433;Database=VoidEmpires_Dev;User Id=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"`
3. Document the .NET user-secrets method if the project supports it for `src/VoidEmpires.Web`.
4. Make clear that `YOUR_USER` and `YOUR_PASSWORD` must be replaced locally only.
5. Warn not to commit secrets and not to paste real passwords into chat, docs, scripts, terminal logs, or tickets.
6. Avoid behavior changes unless a missing configuration convention must be documented.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- docs/dev/sql-server-runbook.md
- docs/dev/final-db-security-notes.md
- src/VoidEmpires.Web/appsettings.json
- src/VoidEmpires.Web/appsettings.Development.json

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/final-db-security-notes.md

## Acceptance criteria

- The runbook includes safe PowerShell environment variable commands.
- The runbook includes user-secrets guidance if supported by the project.
- All credentials are placeholders only.
- No code behavior changes are made unless necessary and documented.

## Constraints

- Do not commit a real SQL Server username, password, or full real connection string.
- Do not require SQL Server for normal build or tests.
- Keep user-facing docs Spanish-first.
- Preserve default provider behavior unless explicitly overridden locally.

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
