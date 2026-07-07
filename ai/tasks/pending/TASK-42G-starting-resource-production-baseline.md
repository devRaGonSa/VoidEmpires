# TASK-42G-starting-resource-production-baseline

---
id: TASK-42G
title: Starting resource and production baseline
status: pending
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Centralize and test the starting resources and base production profile for new users' first planets.

## Context
Every registered user's first/home planet must start equal. Existing seed profiles must remain intact unless the registration bootstrap explicitly uses the new baseline.

## Implementation steps

1. Review economy domain types, catalog resource keys, and existing seed data usage.
2. Define one authoritative baseline object or policy for first planet resources and base production.
3. Update bootstrap to use the baseline for resource stockpiles and production profiles.
4. Keep development seed profiles unchanged unless they intentionally call the baseline.
5. Add tests comparing two registered users' starting resources and production.

## Files to read first

- src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs
- src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs
- src/VoidEmpires.Domain/Economy/ResourceType.cs
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- src/VoidEmpires.Infrastructure/Players/InitialPlayerWorldBootstrapService.cs

## Expected files to modify

- src/VoidEmpires.Application/Players/StartingHomeWorldBaseline.cs
- src/VoidEmpires.Infrastructure/Players/InitialPlayerWorldBootstrapService.cs
- tests/VoidEmpires.Tests/StartingHomeWorldBaselineTests.cs
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs

## Acceptance criteria

- One authoritative baseline defines first planet resource and production values.
- Two independently registered users start with identical baseline values.
- Seed validation flows are not broken.
- Tests document the equality guarantee.

## Constraints

- Do not change queue/resource semantics beyond initial player bootstrap.
- Keep any balance values clearly named and easy to revisit.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
