# TASK-39I

---
id: TASK-39I
title: Document optional real SQL Server smoke test flow
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Document the exact optional real SQL Server smoke test flow and make clear that ordinary `dotnet test` does not require SQL Server.

## Context

Real SQL Server smoke testing must remain opt-in and secret-safe. It should perform `SELECT 1` only.

## Implementation steps

1. Read the current SQL Server runbook, tests, and smoke-related scripts.
2. Document these environment variables:
   - `VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=true`
   - `VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING=...`
3. Explain that the optional smoke performs `SELECT 1` only.
4. Explain that normal `dotnet test` does not run the optional SQL Server smoke.
5. Explain how to clear the environment variables after use.
6. Do not include real passwords or full real connection strings.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- docs/dev/sql-server-runbook.md
- scripts/sqlserver-connection-smoke.ps1
- tests/VoidEmpires.Tests/
- docs/dev/final-db-security-notes.md

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/final-db-security-notes.md

## Acceptance criteria

- The optional smoke flow is documented with placeholder-only values.
- Docs state that the smoke performs `SELECT 1` only.
- Docs state that normal tests do not require SQL Server.
- Docs include cleanup commands for environment variables.

## Constraints

- Do not connect to the real SQL Server.
- Do not add required SQL Server tests to the normal test path.
- Do not commit real credentials.
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
