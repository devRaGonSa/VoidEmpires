# TASK-9X

---
id: TASK-9X
title: Dev endpoint config opt-in coverage
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Dev endpoint config opt-in coverage"
priority: medium
---

## Goal

Add test coverage proving development endpoints are available outside the Development environment when `VoidEmpires:DevEndpoints:Enabled=true`.

## Context

`Program.cs` allows development endpoints when either the environment is `Development` or the config flag is enabled. Current endpoint tests cover the default production denial path, but they do not verify the explicit configuration opt-in path.

## Implementation steps

1. Review the current development endpoint gating in `Program.cs`.
2. Review existing endpoint tests that verify default production denial.
3. Add a focused `WebApplicationFactory` test that runs in `Production` with `VoidEmpires:DevEndpoints:Enabled=true`.
4. Assert that a read-only dev endpoint becomes reachable and still returns its normal downstream status when persistence is disabled.
5. Keep the change test-only.

## Files to read first

- `src/VoidEmpires.Web/Program.cs`
- `tests/VoidEmpires.Tests/DevPlanetVisualStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevelopmentCorsEndpointTests.cs`
- `tests/VoidEmpires.Tests/TestWebApplicationFactoryExtensions.cs`

## Expected files to modify

- `tests/VoidEmpires.Tests/DevPlanetVisualStateEndpointTests.cs`

## Acceptance criteria

- There is automated coverage for the `VoidEmpires:DevEndpoints:Enabled=true` path outside `Development`.
- The test uses existing repo endpoint-testing patterns.
- Validation commands pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

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
