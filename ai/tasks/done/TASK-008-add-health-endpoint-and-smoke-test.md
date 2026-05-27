# TASK-008

---
id: TASK-008
title: Add health endpoint and smoke test
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1 - Technical foundation"
priority: high
---

## Goal
Add a minimal health endpoint to verify the web project can start and respond.

## Context
Before adding gameplay features, the web entrypoint needs a simple deterministic endpoint for local, CI, and future deployment verification.

## Implementation steps

1. Modify only `VoidEmpires.Web` and the test project as needed.
2. Add a `GET /health` endpoint.
3. Return HTTP 200 with a simple deterministic payload such as `{ "status": "ok", "service": "VoidEmpires.Web" }`.
4. Add a smoke or integration test that verifies `/health` returns success and includes the expected status and service values.
5. Use `WebApplicationFactory` or the appropriate ASP.NET Core testing pattern.
6. Add only the minimal packages required for integration testing if extra packages are needed.
7. Do not add database dependencies, authentication, or gameplay features.

## Files to read first

- `src/VoidEmpires.Web/Program.cs` or equivalent web entrypoint
- `tests/VoidEmpires.Tests/*`
- `ai/reports/solution-bootstrap-plan.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- `GET /health` returns HTTP 200.
- The response is deterministic and includes the expected status and service data.
- The smoke test validates the endpoint successfully.
- The change stays isolated to the web project and tests.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build` succeeds.
- `dotnet test` succeeds.
- The `/health` test passes.
- No unrelated files are modified.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `test: add health endpoint smoke coverage`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
