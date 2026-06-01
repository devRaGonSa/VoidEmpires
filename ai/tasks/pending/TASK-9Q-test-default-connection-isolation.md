# TASK-9Q

---
id: TASK-9Q
title: Isolate endpoint tests from ambient DefaultConnection environment variables
status: pending
type: test
team: qa
supporting_teams:
  - backend
roadmap_item: "Test reliability hardening"
priority: medium
---

## Goal

Ensure endpoint tests that verify "persistence not configured" behavior do not depend on the host machine lacking a `ConnectionStrings__DefaultConnection` environment variable.

## Context

During `TASK-9P`, `dotnet test --no-build` initially failed on this machine because an ambient `ConnectionStrings__DefaultConnection` environment variable was present. Several endpoint tests currently use the default `WebApplicationFactory<Program>` without overriding the connection string, so those tests silently switch from the "unconfigured persistence" path to the configured path when a machine-level connection string exists.

The test suite passed only after clearing the environment variables for the test command process. That makes the suite environment-sensitive and should be fixed in repository code.

## Implementation steps

1. Inspect the endpoint tests that currently call `CreateClient()` on the default `WebApplicationFactory<Program>` when asserting service-unavailable behavior.
2. Introduce a shared testing pattern that forces `ConnectionStrings:DefaultConnection` to be empty for "unconfigured persistence" scenarios.
3. Update only the affected endpoint tests to use that shared pattern.
4. Keep configured-persistence tests unchanged unless they also depend on inherited environment state.
5. Run:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Files to read first

- `tests/VoidEmpires.Tests/HealthEndpointTests.cs`
- `tests/VoidEmpires.Tests/AuthEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevFleetOperationalOverviewEndpointTests.cs`
- `src/VoidEmpires.Web/Program.cs`
- `AGENTS.md`

## Expected files to modify

Expected:

- one shared test helper or fixture under `tests/VoidEmpires.Tests/`
- only the endpoint test files that currently rely on the default factory for unconfigured-persistence assertions

## Acceptance criteria

- Endpoint tests that assert service-unavailable or unconfigured-health behavior pass even when `ConnectionStrings__DefaultConnection` is set in the host environment.
- The fix is shared and minimal rather than duplicating ad-hoc overrides in many tests.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes without manually clearing environment variables in the shell.

## Constraints

- Do not modify application runtime behavior for production or development endpoints.
- Keep the fix inside test infrastructure unless a broader application change is clearly necessary.
- Do not modify unrelated tests.

## Validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if more endpoint test files are affected than can be safely updated in one pass.
