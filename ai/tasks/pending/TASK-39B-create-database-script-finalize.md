# TASK-39B

---
id: TASK-39B
title: Finalize manual SQL Server database creation script
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Finalize `scripts/sqlserver/create-database.sql` for safe, manual SSMS use when creating the recommended first SQL Server database, `VoidEmpires_Dev`.

## Context

The script must help the user create the database manually without embedding credentials or performing destructive actions. It must be reviewable and safe by default.

## Implementation steps

1. Read the SQL Server runbook and the existing create-database script.
2. Ensure the script recommends `VoidEmpires_Dev` as the first controlled validation database.
3. Ensure database creation is guarded with `IF DB_ID(...) IS NULL`.
4. Ensure the script does not create logins, users, passwords, or connection strings.
5. Add or refine comments explaining:
   - run manually in SSMS;
   - review before execution;
   - back up the existing database before modifying an existing one;
   - the script does not drop, truncate, or reset data.
6. Keep the script intentionally limited to safe database creation guidance.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- docs/dev/sql-server-runbook.md
- scripts/sqlserver/create-database.sql
- scripts/check-dev-qa-scripts.ps1

## Expected files to modify

- scripts/sqlserver/create-database.sql
- docs/dev/sql-server-runbook.md

## Acceptance criteria

- `scripts/sqlserver/create-database.sql` is suitable for manual SSMS review and execution.
- `VoidEmpires_Dev` is the recommended first database name.
- The script uses an `IF DB_ID(...) IS NULL` guard.
- No credentials or destructive SQL statements are added.

## Constraints

- Do not create SQL Server logins or passwords.
- Do not drop, truncate, reset, or migrate any database.
- Do not connect to a real SQL Server.
- Keep user-facing docs Spanish-first.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
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
