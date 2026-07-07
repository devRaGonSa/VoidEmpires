# TASK-42H-registration-api-endpoint

---
id: TASK-42H
title: Registration API endpoint
status: done
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Expose a safe registration endpoint, for example `POST /api/accounts/register`.

## Context
The endpoint should orchestrate Identity account creation and initial player world bootstrap, returning safe JSON for the SPA.

## Implementation steps

1. Review endpoint mapping conventions in `VoidEmpires.Web`.
2. Add an accounts endpoint mapping class or extend an existing auth endpoint group.
3. Accept the registration request contract.
4. Use the Identity registration service and bootstrap service transactionally where possible.
5. Return 200 or 201 for success, 400 for validation errors, and 409 or 400 for duplicate account/civilization conflicts.
6. Never include password, confirmPassword, or raw Identity errors in the response.
7. Add WebApplicationFactory tests for success, validation failure, and duplicate account.

## Files to read first

- src/VoidEmpires.Web/Program.cs
- src/VoidEmpires.Web/DevEndpointMappings.cs
- tests/VoidEmpires.Tests/AuthEndpointTests.cs
- src/VoidEmpires.Application/Identity/AccountRegistrationRequest.cs
- src/VoidEmpires.Application/Players/InitialPlayerWorldBootstrapResult.cs

## Expected files to modify

- src/VoidEmpires.Web/AccountEndpoints.cs
- src/VoidEmpires.Web/Program.cs
- tests/VoidEmpires.Tests/AccountRegistrationEndpointTests.cs

## Acceptance criteria

- `POST /api/accounts/register` accepts the request contract and returns safe JSON.
- Successful registration creates account, profile, civilization, home planet, ownership, resources, and production.
- Validation and duplicate errors return safe structured responses.
- Normal tests remain provider-independent.

## Constraints

- Do not log passwords.
- Do not require email verification.
- Do not expose raw Identity internals.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
