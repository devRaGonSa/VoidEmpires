# TASK-41AO

---
id: TASK-41AO
title: Secret scan still green
status: done
type: tooling
team: platform
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Ensure product copy changes did not introduce secrets or real connection strings.

## Context
Block 41 must preserve the repository secret-safety baseline while changing product-facing copy.

## Implementation steps

1. Inspect the repository secret scan script and recent product-copy changes.
2. Run the secret scan.
3. Confirm no real SQL password, resolved SQL Server connection string, or committed credential exists.
4. Fix any accidental secret-like committed text by replacing it with safe placeholders.
5. Do not change unrelated files.

## Files to read first

- scripts/check-repo-secret-scan.ps1
- docs/dev/sql-server-user-checklist.md
- ai/current-state.md

## Expected files to modify

- scripts/check-repo-secret-scan.ps1
- docs/dev/
- ai/current-state.md

## Acceptance criteria

- Repository secret scan passes.
- No real SQL password is committed.
- No resolved connection string with a real password is committed.

## Constraints

- Do not commit secrets.
- Do not apply migrations automatically.
- Do not require SQL Server for ordinary tests.

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
