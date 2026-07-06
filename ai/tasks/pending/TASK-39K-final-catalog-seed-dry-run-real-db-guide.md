# TASK-39K

---
id: TASK-39K
title: Document final catalog seed dry-run for SQL Server context
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: medium
---

## Goal

Document how to dry-run the final catalog seed in the SQL Server context without deleting user gameplay data or enabling destructive apply behavior.

## Context

The seed helper must remain safe while the first real SQL Server database flow is validated. Real apply behavior remains deferred unless already safely implemented.

## Implementation steps

1. Read the final catalog seed script and SQL Server runbook.
2. Confirm and document that `scripts/sqlserver-final-catalog-seed.ps1` defaults to dry-run.
3. Document that `-Apply -ConfirmMutation` currently safely fails if real apply remains deferred.
4. Explain that no user gameplay data should be deleted.
5. Do not implement destructive apply behavior.
6. Do not include real passwords or real full connection strings.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- scripts/sqlserver-final-catalog-seed.ps1
- docs/dev/sql-server-runbook.md
- docs/dev/final-db-security-notes.md
- scripts/check-dev-qa-scripts.ps1

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/final-db-phase-readiness-report.md
- scripts/sqlserver-final-catalog-seed.ps1

## Acceptance criteria

- The dry-run behavior is documented accurately.
- Any apply path remains non-destructive or safely deferred.
- Docs state that user gameplay data must not be deleted.
- No real credentials are committed.

## Constraints

- Do not implement destructive apply.
- Do not delete, truncate, or reset real SQL Server data.
- Do not connect to the real SQL Server unless the task explicitly validates only local script behavior without real credentials.
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
