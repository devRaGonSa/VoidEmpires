# TASK-39M

---
id: TASK-39M
title: Create user-facing SQL Server checklist
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Create a concise Spanish checklist for the user to manually execute the controlled SQL Server creation and validation flow.

## Context

The runbook may be detailed; the user also needs a short checklist at `docs/dev/sql-server-user-checklist.md`.

## Implementation steps

1. Read the SQL Server runbook and security notes.
2. Add `docs/dev/sql-server-user-checklist.md` in Spanish.
3. Include checkbox items for:
   - create `VoidEmpires_Dev` in SSMS;
   - configure the connection string locally;
   - run connection smoke;
   - generate migration script;
   - review migration script;
   - optionally apply manually;
   - run optional SQL smoke;
   - run the app;
   - clear environment variables.
4. Use placeholders only for usernames/passwords.
5. Keep the checklist concise and aligned with the runbook.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- docs/dev/sql-server-runbook.md
- docs/dev/final-db-security-notes.md
- docs/dev/sql-server-migration-strategy.md

## Expected files to modify

- docs/dev/sql-server-user-checklist.md
- docs/dev/sql-server-runbook.md

## Acceptance criteria

- `docs/dev/sql-server-user-checklist.md` exists.
- The checklist is Spanish-first and uses Markdown checkboxes.
- The checklist includes all required manual steps.
- No real password or full real connection string is committed.

## Constraints

- Do not connect to SQL Server.
- Do not apply migrations.
- Do not include secrets.
- Keep the checklist short and operational.

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
