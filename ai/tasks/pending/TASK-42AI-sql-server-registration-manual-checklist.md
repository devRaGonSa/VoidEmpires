# TASK-42AI-sql-server-registration-manual-checklist

---
id: TASK-42AI
title: SQL Server registration manual checklist
status: pending
type: docs
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: medium
---

## Goal
Add a manual SQL Server validation checklist for registration.

## Context
This is a checklist only. Do not claim the manual validation was performed unless it actually is performed outside this task.

## Implementation steps

1. Review SQL Server runbook and registration readiness docs.
2. Add checklist steps to register a user in a SQL Server-backed environment.
3. Include checks for AspNetUsers, PlayerProfile, Civilization, PlanetOwnership, starting resources, and no plaintext password.
4. Include reminders to avoid exposing secrets and to record evidence separately.
5. Explicitly state that the checklist is prepared but not executed by this docs task.

## Files to read first

- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-user-checklist.md
- docs/dev/user-account-auth-readiness.md
- docs/dev/final-db-security-notes.md

## Expected files to modify

- docs/dev/sql-server-user-checklist.md
- docs/dev/sql-server-runbook.md

## Acceptance criteria

- Manual checklist covers required database rows and no-plaintext-password verification.
- Checklist does not include real credentials or connection strings.
- Docs do not claim manual QA was performed.

## Constraints

- Documentation only.
- Do not modify migrations or scripts.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
