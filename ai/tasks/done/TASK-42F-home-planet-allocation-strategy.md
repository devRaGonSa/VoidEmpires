# TASK-42F-home-planet-allocation-strategy

---
id: TASK-42F
title: Home planet allocation strategy
status: done
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Implement or refine a safe home planet allocation strategy for new registered users.

## Context
If a galaxy/system/planet already exists, registration should allocate an available valid home world. If none exists, bootstrap a safe default structure. Tests must be deterministic without forbidding future randomness.

## Implementation steps

1. Review galaxy, solar system, planet, and ownership persistence configuration.
2. Design an allocator that finds unowned valid home-world candidates first.
3. If no candidate exists, create a default galaxy/system/planet using deterministic naming and coordinates that avoid collisions.
4. Ensure the allocator cannot assign the same starting planet to two users.
5. Document the allocation algorithm in dev docs.
6. Add tests for empty universe, existing available planet, and occupied planet collision avoidance.

## Files to read first

- src/VoidEmpires.Domain/Galaxy/Galaxy.cs
- src/VoidEmpires.Domain/Galaxy/SolarSystem.cs
- src/VoidEmpires.Domain/Galaxy/Planet.cs
- src/VoidEmpires.Infrastructure/Persistence/Configurations/PlanetConfiguration.cs
- src/VoidEmpires.Infrastructure/Persistence/Configurations/PlanetOwnershipConfiguration.cs
- src/VoidEmpires.Infrastructure/GalaxyGeneration/GalaxyGenerationService.cs

## Expected files to modify

- src/VoidEmpires.Application/Players/IHomePlanetAllocator.cs
- src/VoidEmpires.Infrastructure/Players/HomePlanetAllocator.cs
- src/VoidEmpires.Infrastructure/Players/InitialPlayerWorldBootstrapService.cs
- docs/dev/user-account-auth-readiness.md
- tests/VoidEmpires.Tests/HomePlanetAllocatorTests.cs

## Acceptance criteria

- Empty database registration can create a safe default home world.
- Existing available planets can be allocated without duplication.
- Already owned planets are never assigned to another new user.
- Allocation behavior is documented and deterministic enough for tests.

## Constraints

- Do not overwrite development seed planets unless an internal operator flow explicitly does so.
- Keep the algorithm small and reviewable.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
