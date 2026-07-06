# TASK-39A

---
id: TASK-39A
title: Controlled real SQL Server database runbook audit
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Audit the current SQL Server runbook and scripts, then document the exact safe path for manually creating and validating `VoidEmpires_Dev`.

## Context

This is the first task in Block 39A-39P. The repository is prepared for SQL Server, but real database creation must remain controlled, manual, non-destructive, and free of committed secrets.

## Implementation steps

1. Read the SQL Server runbook, migration strategy, security notes, and current SQL Server helper scripts.
2. Confirm current scripts are non-destructive by default and do not drop, truncate, reset, or automatically apply migrations to a real SQL Server.
3. Confirm `VoidEmpires_Dev` is used only as a safe example or recommended first validation target.
4. Confirm no real SQL Server username, password, or full real connection string is present in docs, scripts, or appsettings.
5. Update documentation only if needed to make the intended user flow explicit:
   - create the database manually in SSMS;
   - configure the local connection string outside the repo;
   - run `SELECT 1` smoke validation;
   - generate the migration script;
   - review the generated script;
   - apply manually only if approved;
   - run the optional SQL Server smoke;
   - run the app against SQL Server.
6. Do not change runtime behavior unless a documentation-only correction requires a nearby script comment.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- docs/dev/sql-server-runbook.md
- docs/dev/final-db-security-notes.md
- scripts/sqlserver/create-database.sql
- scripts/sqlserver-connection-smoke.ps1

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/final-db-security-notes.md
- ai/current-state.md

## Acceptance criteria

- The runbook describes the safe manual flow for `VoidEmpires_Dev`.
- Existing scripts are confirmed or documented as non-destructive by default.
- No real SQL Server username, password, or full real connection string is committed.
- No gameplay, auth, migration-apply, or database mutation behavior changes are introduced.

## Constraints

- Do not connect to the real SQL Server.
- Do not apply migrations.
- Do not add secrets or real credentials.
- Keep user-facing docs Spanish-first.
- Preserve the default persistence provider behavior.
- Keep the change under the normal task budget unless a follow-up task is created.

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
