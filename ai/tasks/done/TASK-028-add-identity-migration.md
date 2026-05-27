# TASK-028

---
id: TASK-028
title: Add Identity persistence migration
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2A - Identity and Brevo email foundation"
priority: medium
---

## Goal
Create an EF Core migration for ASP.NET Core Identity tables.

## Context
Identity infrastructure is now integrated with the `DbContext`. The repository needs a migration for the Identity schema against PostgreSQL. This migration must be generated without real NAS credentials and must not be applied to the real database.

## Implementation steps

1. Modify `src/VoidEmpires.Infrastructure` persistence migrations.
2. Modify a design-time factory if needed.
3. Add an Identity schema migration such as `AddIdentitySchema`.
4. Ensure migration order is valid if previous migrations exist.
5. Add a safe placeholder connection string to any design-time factory if required.
6. Do not include real hostnames, usernames, passwords, VPN details, or NAS details.
7. Do not apply the migration to the real NAS database.
8. Do not add gameplay entities.

## Files to read first

- `src/VoidEmpires.Infrastructure/*`
- `ai/current-state.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/*`
- `tests/VoidEmpires.Tests/*` if needed

## Acceptance criteria

- Migration files for Identity exist.
- The migration code contains no secrets.
- The migration order is valid.
- No real NAS database is touched.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- Migration files exist.
- Migration code contains no secrets.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(db): add identity schema migration`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
