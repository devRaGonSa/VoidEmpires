# TASK-43V-resource-capacity-backend-viewmodel

---
id: TASK-43V
title: Resource capacity backend viewmodel
status: pending
type: fullstack
team: platform
supporting_teams: [frontend]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Ensure backend/frontend can show resources versus storage capacity.

## Context
The resource bar should show real resource amounts and storage/capacity. If capacity is missing, add a safe computed or canonical backend model instead of arbitrary frontend values.

## Implementation steps

1. Inspect current planet UI-state and resource DTOs.
2. If capacities already exist, expose them cleanly to the frontend.
3. If capacities do not exist, add a safe computed/canonical capacity model based on existing storage/building state or baseline.
4. Update frontend types and resource display helpers.
5. Add tests covering resource amount and capacity in current planet state.
6. Build backend and frontend.

## Files to read first

- src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs
- src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs
- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Application/Planets/PlanetResourceDto.cs
- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs

## Acceptance criteria

- Planet state includes resource amount and capacity.
- Resource bar consumes backend capacity data.
- Tests cover amount and capacity.
- Frontend does not fake arbitrary capacities.

## Constraints

- Do not break SQL Server support.
- Do not require SQL Server for automated tests.
- Do not change resource semantics beyond display view-model needs.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
