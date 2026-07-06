# TASK-39D

---
id: TASK-39D
title: Improve SQL Server connection smoke usability
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Make `scripts/sqlserver-connection-smoke.ps1` easy and safe to run against `VoidEmpires_Dev`.

## Context

The connection smoke must validate connectivity with `SELECT 1` only, must not print passwords, and must provide actionable failures for common SQL Server setup problems.

## Implementation steps

1. Read the current smoke script and runbook.
2. Ensure the script accepts either `-ConnectionString` or an environment variable.
3. Ensure the script performs only a `SELECT 1` query and does not inspect or mutate schema/data.
4. Ensure the script never prints passwords or the full raw connection string.
5. Improve error messages where needed for:
   - server unreachable;
   - login failed;
   - database missing;
   - encryption or certificate issue.
6. Update docs with an exact command that uses a local PowerShell variable, not a committed connection string.
7. Keep the smoke test opt-in and outside normal `dotnet test`.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- scripts/sqlserver-connection-smoke.ps1
- docs/dev/sql-server-runbook.md
- docs/dev/final-db-security-notes.md
- scripts/check-dev-qa-scripts.ps1

## Expected files to modify

- scripts/sqlserver-connection-smoke.ps1
- docs/dev/sql-server-runbook.md

## Acceptance criteria

- The smoke script accepts `-ConnectionString` or a documented environment variable.
- The script runs `SELECT 1` only.
- Passwords and full real connection strings are not printed.
- Common connection failures produce actionable guidance.

## Constraints

- Do not apply migrations.
- Do not inspect or mutate gameplay data.
- Do not require SQL Server for normal test runs.
- Do not commit real credentials.
- Keep user-facing docs Spanish-first.

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
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
