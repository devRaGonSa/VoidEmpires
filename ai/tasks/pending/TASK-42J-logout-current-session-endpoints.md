# TASK-42J-logout-current-session-endpoints

---
id: TASK-42J
title: Logout and current session endpoints
status: pending
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Expose logout and current-session endpoints: `POST /api/accounts/logout` and `GET /api/accounts/me`.

## Context
The SPA needs backend source-of-truth session state. Anonymous behavior must be consistent and safe.

## Implementation steps

1. Review login endpoint and auth/session configuration.
2. Implement logout through the chosen Identity-backed auth mechanism.
3. Implement `/me` to return authenticated player profile, civilization, and home planet summary.
4. Return 401 or a consistent unauthenticated response for anonymous requests.
5. Add endpoint tests for authenticated `/me`, anonymous `/me`, logout success, and post-logout `/me`.

## Files to read first

- src/VoidEmpires.Web/AccountEndpoints.cs
- src/VoidEmpires.Application/Identity/AccountSessionResult.cs
- src/VoidEmpires.Infrastructure/Players/InitialPlayerWorldBootstrapService.cs
- tests/VoidEmpires.Tests/AccountLoginEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Application/Identity/CurrentAccountSession.cs
- src/VoidEmpires.Web/AccountEndpoints.cs
- tests/VoidEmpires.Tests/AccountSessionEndpointTests.cs

## Acceptance criteria

- `POST /api/accounts/logout` signs out the current session safely.
- `GET /api/accounts/me` returns safe authenticated account/player summary.
- Anonymous behavior is documented by tests.
- Raw ids are not required in player-facing UI copy, but safe ids may remain API fields.

## Constraints

- Do not expose passwords, password hashes, security stamps, or raw claims internals.
- Do not require browser tests.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
