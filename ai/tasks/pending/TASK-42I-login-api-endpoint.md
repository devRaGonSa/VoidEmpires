# TASK-42I-login-api-endpoint

---
id: TASK-42I
title: Login API endpoint
status: pending
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Expose a product login endpoint, for example `POST /api/accounts/login`.

## Context
Login should use the chosen ASP.NET Core Identity-backed auth mechanism and return safe current player information for the SPA.

## Implementation steps

1. Review Identity configuration and endpoint tests from registration work.
2. Define login request/response contracts if they do not already exist.
3. Implement login through `SignInManager`/`UserManager` or the documented chosen auth mechanism.
4. Return safe session/current player summary without passwords, tokens, or raw Identity internals.
5. Add tests for successful login, wrong password, unknown account, and safe error response.

## Files to read first

- src/VoidEmpires.Web/AccountEndpoints.cs
- src/VoidEmpires.Infrastructure/Identity/IdentityAccountRegistrationService.cs
- src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
- tests/VoidEmpires.Tests/AccountRegistrationEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Application/Identity/AccountLoginRequest.cs
- src/VoidEmpires.Application/Identity/AccountSessionResult.cs
- src/VoidEmpires.Web/AccountEndpoints.cs
- tests/VoidEmpires.Tests/AccountLoginEndpointTests.cs

## Acceptance criteria

- `POST /api/accounts/login` exists and uses real Identity-backed authentication.
- Success response includes safe current player/civilization/home planet summary when available.
- Failure responses are safe and do not reveal secrets.
- Tests cover success and failure.

## Constraints

- Do not implement fake auth.
- Do not store tokens in localStorage.
- Do not expose password or token secrets.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
