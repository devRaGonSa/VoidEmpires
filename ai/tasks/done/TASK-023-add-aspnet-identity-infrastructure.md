# TASK-023

---
id: TASK-023
title: Add ASP.NET Core Identity infrastructure
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2A - Identity and Brevo email foundation"
priority: high
---

## Goal
Add ASP.NET Core Identity persistence foundation using the existing PostgreSQL DbContext.

## Context
VoidEmpires needs user accounts. ASP.NET Core Identity should be integrated into Infrastructure and Web using EF Core and PostgreSQL, but without requiring tests to connect to the real NAS database.

## Implementation steps

1. Modify `src/VoidEmpires.Infrastructure`.
2. Modify `src/VoidEmpires.Web` as needed for service registration.
3. Modify tests as needed.
4. Add the required ASP.NET Core Identity EF Core packages.
5. Introduce an application user class such as `VoidEmpiresUser`.
6. Update `VoidEmpiresDbContext` to inherit from the appropriate Identity DbContext type if needed.
7. Configure Identity with conservative defaults, including unique email.
8. Prepare or enable confirmed email where practical without weakening security.
9. Add an Infrastructure service registration extension for Identity.
10. Register Identity in `Program.cs`.
11. Do not add Brevo integration in this task.
12. Do not add public registration endpoints yet unless minimal wiring requires it.
13. Do not add gameplay player or civilisation entities.
14. Do not connect to the real PostgreSQL database.
15. Do not commit secrets.

## Files to read first

- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/Program.cs`
- `tests/VoidEmpires.Tests/*`
- `ai/reports/identity-email-foundation.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- ASP.NET Core Identity is integrated into Infrastructure and Web.
- Identity uses the PostgreSQL-backed `DbContext` foundation.
- Existing endpoints and tests continue to work without a configured real database.
- Tests do not require Brevo.
- No gameplay entities are introduced.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- Tests do not require real PostgreSQL.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(identity): add ASP.NET Core Identity infrastructure`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
