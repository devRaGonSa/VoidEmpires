# TASK-018

---
id: TASK-018
title: Add application DbContext skeleton
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1B - Persistence foundation with PostgreSQL"
priority: high
---

## Goal
Introduce the initial EF Core `DbContext` skeleton without gameplay entities or migrations.

## Context
VoidEmpires needs a persistence boundary in the Infrastructure layer. The initial `DbContext` should exist but should not yet model gameplay concepts. This keeps the architecture ready for future domain entities while avoiding premature design.

## Implementation steps

1. Add a `DbContext` class under `src/VoidEmpires.Infrastructure/Persistence`.
2. Add a dependency injection registration extension under Infrastructure.
3. Wire the registration from `VoidEmpires.Web` using the configured connection string.
4. Do not add gameplay entities.
5. Do not add migrations.
6. Do not require a real database connection for normal startup unless the app is configured to use persistence.
7. Do not break the existing `/` and `/health` tests.
8. If the connection string is empty, do not register PostgreSQL in a way that breaks startup or tests.
9. If the connection string is present, register the `DbContext` with `UseNpgsql`.

## Files to read first

- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/Program.cs`
- `tests/VoidEmpires.Tests/*`
- `ai/reports/solution-bootstrap-plan.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- A `VoidEmpiresDbContext`-style class exists in the Infrastructure layer.
- Infrastructure exposes a registration extension method for persistence services.
- The web app calls the Infrastructure registration.
- Startup and existing tests continue to pass with an empty connection string.
- No gameplay entities or migrations are introduced.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- Existing endpoint tests still pass.
- Tests do not require PostgreSQL.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat: add PostgreSQL DbContext skeleton`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
