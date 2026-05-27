# TASK-017

---
id: TASK-017
title: Add EF Core Npgsql infrastructure packages
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1B - Persistence foundation with PostgreSQL"
priority: high
---

## Goal
Add the minimal package dependencies needed for PostgreSQL persistence in the Infrastructure project.

## Context
VoidEmpires will use EF Core with PostgreSQL through Npgsql. This task adds packages only; it should not introduce real database connectivity or game entities.

## Implementation steps

1. Modify `src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj`.
2. Modify `src/VoidEmpires.Web/VoidEmpires.Web.csproj` only if a design-time or runtime package reference is required there.
3. Add `Microsoft.EntityFrameworkCore`.
4. Add `Npgsql.EntityFrameworkCore.PostgreSQL`.
5. Add `Microsoft.EntityFrameworkCore.Design` where needed for future migrations.
6. Keep versions compatible with .NET 8.
7. Prefer explicit package versions.
8. Avoid unnecessary packages.
9. Do not add `DbContext` yet unless strictly required by package setup.
10. Do not add migrations.
11. Do not add game entities.
12. Do not add real connection strings.

## Files to read first

- `src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj`
- `src/VoidEmpires.Web/VoidEmpires.Web.csproj` if present
- `ai/repo-context.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj`
- `src/VoidEmpires.Web/VoidEmpires.Web.csproj` if needed

## Acceptance criteria

- The Infrastructure project includes the minimal EF Core and Npgsql package references needed for future PostgreSQL persistence.
- Any Web project package changes are limited to what is strictly required.
- No real database connectivity is introduced.
- No tests require PostgreSQL.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- No real database connection is attempted by tests.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore: add EF Core PostgreSQL infrastructure packages`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
