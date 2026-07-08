# TASK-47F

---
id: TASK-47F
title: Verify basic construction availability
status: pending
type: backend
team: backend
supporting_teams: [frontend]
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: high
---

## Goal
Ensure basic construction options are buildable when the player has enough resources and capacity.

## Context
Starting construction must keep real requirements while allowing core economy/energy buildings to start from a valid home planet.

## Implementation steps

1. Verify the starting catalog state for metal, crystal, gas, and energy buildings.
2. Add or adjust tests for buildability from starting state.
3. Keep advanced buildings blocked when real requirements are missing.
4. Ensure blocked card copy shows real requirements only when blocked.

## Files to read first

- src/VoidEmpires.Infrastructure/Services/DevConstructionUiStateService.cs
- src/VoidEmpires.Domain/Gameplay/Construction/BuildingCatalog.cs
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs
- tests/VoidEmpires.Tests/DevConstructionUiStateEndpointTests.cs
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx

## Expected files to modify

- src/VoidEmpires.Infrastructure/Services/DevConstructionUiStateService.cs
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs
- tests/VoidEmpires.Tests/DevConstructionUiStateEndpointTests.cs
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx

## Acceptance criteria

- At least the basic resource/energy construction options are available with sufficient resources/capacity.
- Advanced requirements remain enforced.

## Constraints

- Do not bypass real costs/resources.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend

