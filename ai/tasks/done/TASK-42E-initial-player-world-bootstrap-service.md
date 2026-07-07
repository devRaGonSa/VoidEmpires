# TASK-42E-initial-player-world-bootstrap-service

---
id: TASK-42E
title: Initial player world bootstrap service
status: done
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Implement the service that creates PlayerProfile, Civilization, starting planet ownership, initial resources, and base production after account creation.

## Context
Registration must create the player's initial persistent presence in the shared online universe. Multiple registered users must coexist without collisions.

## Implementation steps

1. Review existing PlayerProfile, Civilization, PlanetOwnership, planet resource stockpile, and production profile models.
2. Extend or add an application service for initial world bootstrap linked to an Identity user id.
3. Create PlayerProfile and Civilization with normalized names.
4. Assign or create a home planet through an allocation strategy without overwriting dev seed validation planets.
5. Create PlanetOwnership, resource stockpiles, and production profile using one baseline.
6. Prevent duplicate bootstrap for the same Identity user.
7. Add tests for success, duplicate bootstrap prevention, and persistence.

## Files to read first

- src/VoidEmpires.Domain/Players/PlayerProfile.cs
- src/VoidEmpires.Domain/Players/Civilization.cs
- src/VoidEmpires.Domain/Colonization/PlanetOwnership.cs
- src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs
- src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs
- src/VoidEmpires.Infrastructure/Players/StartingCivilizationService.cs
- tests/VoidEmpires.Tests/PlanetOwnershipDomainTests.cs

## Expected files to modify

- src/VoidEmpires.Application/Players/IInitialPlayerWorldBootstrapService.cs
- src/VoidEmpires.Application/Players/InitialPlayerWorldBootstrapRequest.cs
- src/VoidEmpires.Application/Players/InitialPlayerWorldBootstrapResult.cs
- src/VoidEmpires.Infrastructure/Players/InitialPlayerWorldBootstrapService.cs
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs

## Acceptance criteria

- A user id maps to one PlayerProfile and one initial Civilization.
- A home planet is assigned or generated and owned by that civilization.
- Starting resources and production are initialized.
- Duplicate bootstrap for the same user is rejected or returns the existing bootstrap safely.

## Constraints

- Do not add combat, fleet movement, market transactions, alliance mutations, or espionage execution.
- Do not require SQL Server for normal tests.
- Respect existing persistence conventions and DI wiring.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
