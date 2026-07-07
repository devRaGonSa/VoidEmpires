# TASK-42U-multiple-users-registration-tests

---
id: TASK-42U
title: Multiple users registration tests
status: pending
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Add backend tests proving multiple registered users can coexist safely.

## Context
VoidEmpires is multiplayer. Two users registering into the same persistent database must receive distinct player profiles, civilizations, and non-conflicting home planet ownership while sharing equal starting baseline values.

## Implementation steps

1. Review registration endpoint/service tests added earlier in Block 42.
2. Add a test that registers two unique users through the highest practical layer.
3. Assert distinct PlayerProfiles, distinct Civilizations, and distinct HomePlanets or non-conflicting ownership.
4. Assert equal starting resource and production baseline values.
5. Add duplicate registration rejection coverage if not already present.

## Files to read first

- tests/VoidEmpires.Tests/AccountRegistrationEndpointTests.cs
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs
- tests/VoidEmpires.Tests/StartingHomeWorldBaselineTests.cs
- src/VoidEmpires.Infrastructure/Players/InitialPlayerWorldBootstrapService.cs

## Expected files to modify

- tests/VoidEmpires.Tests/MultipleUsersRegistrationTests.cs
- tests/VoidEmpires.Tests/AccountRegistrationEndpointTests.cs

## Acceptance criteria

- Two users register independently in one test database.
- PlayerProfile, Civilization, and home planet ownership do not collide.
- Starting resource and production baselines match.
- Duplicate registration is rejected safely.

## Constraints

- Normal tests must not require SQL Server.
- Avoid brittle assertions tied to random allocation order.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
