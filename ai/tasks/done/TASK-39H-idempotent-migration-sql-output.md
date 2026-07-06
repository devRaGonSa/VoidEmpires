# TASK-39H

---
id: TASK-39H
title: Verify idempotent SQL migration script output path
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Ensure the project can generate an idempotent SQL Server migration script for manual review, or document why generation remains blocked.

## Context

This task depends on whether the baseline migration exists after TASK-39G. Generated SQL should be reviewable and should not be applied automatically or committed as one-off output.

## Implementation steps

1. Read the migration strategy, dry-run docs, and migration generation helper.
2. If `SqlServerInitialBaseline` exists:
   - verify the helper can generate an idempotent SQL script to a local output path;
   - do not apply the generated script;
   - do not commit one-off generated SQL output unless it is an intentional template and documented as such.
3. If the baseline migration is deferred:
   - document that SQL script generation remains blocked until the baseline exists.
4. Update the runbook with the correct current path.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- scripts/sqlserver-script-migration.ps1
- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-migration-strategy.md
- docs/dev/sql-server-migration-dry-run.md

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-migration-dry-run.md
- scripts/sqlserver-script-migration.ps1

## Acceptance criteria

- If the baseline exists, idempotent SQL generation is verified without applying it.
- If the baseline is deferred, the runbook states the blocker clearly.
- No generated one-off SQL file is committed unless intentionally documented.
- No real SQL Server connection is required.

## Constraints

- Do not apply migrations.
- Do not connect to a real SQL Server.
- Do not commit secrets.
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
