# TASK-42AC-authenticated-playable-loop-smoke-tests

---
id: TASK-42AC
title: Authenticated playable loop smoke tests
status: pending
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Add backend smoke tests for the registration-to-world loop.

## Context
The backend must prove a registered user can authenticate, fetch current session data, and fetch planet UI state for the generated home planet without requiring SQL Server.

## Implementation steps

1. Review account endpoint tests and planet UI state endpoint tests.
2. Register a user through the account registration endpoint or service.
3. Login or assert a valid account/session response according to the chosen session approach.
4. Fetch `/api/accounts/me`.
5. Fetch planet UI state for the created home planet.
6. Assert no SQL Server dependency for normal test execution.

## Files to read first

- tests/VoidEmpires.Tests/AccountRegistrationEndpointTests.cs
- tests/VoidEmpires.Tests/AccountLoginEndpointTests.cs
- tests/VoidEmpires.Tests/AccountSessionEndpointTests.cs
- tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs
- src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs

## Expected files to modify

- tests/VoidEmpires.Tests/AuthenticatedPlayableLoopSmokeTests.cs

## Acceptance criteria

- Smoke test covers register, login/session, `/me`, and home planet UI-state fetch.
- Test does not require SQL Server.
- Test uses generated user data and no real credentials.

## Constraints

- Do not claim browser QA.
- Keep smoke test stable and focused.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
