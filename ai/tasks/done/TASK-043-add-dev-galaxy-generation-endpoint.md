# TASK-043

---
id: TASK-043
title: Add development galaxy generation endpoint
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2C.2 - Persisted galaxy generation foundation"
priority: medium
---

## Goal
Expose a controlled development endpoint to generate and persist a test galaxy.

## Context
The backend should have a simple API-first way to generate a galaxy in development and testing. This endpoint is not a public gameplay endpoint and must not be enabled accidentally in production.

## Implementation steps

1. Modify `src/VoidEmpires.Web`.
2. Modify tests.
3. Do not add UI.
4. Do not add player or civilisation ownership.
5. Do not add economy, fleets, combat, alliances, espionage, construction, or research.
6. Do not call a real PostgreSQL database in tests.
7. Add a minimal endpoint such as `POST /api/dev/galaxies/generate`.
8. Accept a request body with name, seed, solar system count, minimum planets per system, and maximum planets per system.
9. Return success, galaxy ID, galaxy name, solar system count, planet count, and errors.
10. Disable the endpoint outside the Development environment unless an explicit safe configuration flag enables it.
11. If disabled, return 404 or 403 deterministically.
12. Do not expose stack traces or connection strings.
13. If persistence is not configured, return 503 Service Unavailable.
14. If the request is invalid, return 400.
15. If generation succeeds, return 201 Created or 200 OK.
16. If the galaxy name is duplicated, return 409 Conflict or a deterministic 400-level error.

## Files to read first

- `src/VoidEmpires.Web/*`
- `src/VoidEmpires.Application/*`
- `tests/VoidEmpires.Tests/*`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- The development galaxy generation endpoint exists.
- The endpoint is disabled outside Development unless explicitly enabled.
- The endpoint returns 503 when persistence is not configured.
- Invalid requests return 400.
- Successful requests can be tested with a fake `IGalaxyGenerationService`.
- Tests do not require real PostgreSQL.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- All tests pass.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(api): add development galaxy generation endpoint`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
