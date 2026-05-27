# TASK-019

---
id: TASK-019
title: Add persistence health status without real database dependency
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1B - Persistence foundation with PostgreSQL"
priority: medium
---

## Goal
Expose whether persistence is configured without requiring a real database check.

## Context
The current `/health` endpoint reports only the web service status. Since PostgreSQL configuration is optional and externally supplied, `/health` should expose whether persistence is configured, but it must not attempt to connect to the real NAS database in tests or default development mode.

## Implementation steps

1. Modify the `/health` response.
2. Modify tests accordingly.
3. Do not add a real database connectivity health check yet.
4. Do not require PostgreSQL to be available.
5. Do not expose connection string values.
6. Extend the payload with a deterministic field such as `persistence.configured` and `persistence.provider`.
7. Ensure `configured` is `true` only when `ConnectionStrings:DefaultConnection` is non-empty.
8. Use a fake non-secret connection string if testing the configured path.

## Files to read first

- `src/VoidEmpires.Web/Program.cs`
- `tests/VoidEmpires.Tests/*`
- `ai/task-template.md`
- `ai/current-state.md`

## Expected files to modify

- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- `/health` still returns status `ok` and service `VoidEmpires.Web`.
- The response includes a deterministic persistence status structure.
- Default empty configuration reports `configured=false`.
- Tests verify the default and configured cases without opening a database connection.
- No secrets are exposed.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- `/health` does not expose secrets.
- No test requires a live database.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat: expose persistence configuration health status`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
