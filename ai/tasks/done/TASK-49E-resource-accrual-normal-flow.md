# TASK-49E

---
id: TASK-49E
title: Resource accrual normal flow
status: completed
type: backend
team: backend
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Apply resource generation based on elapsed time and existing production rates.

## Context
Normal read refresh must use persisted backend state and respect storage capacity.

## Implementation steps

1. Inspect production profile and stockpile models.
2. Ensure gameplay refresh accrues resources over elapsed time.
3. Add tests for increase, cap, and no double accrual.

## Files to read first

- src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs
- src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs
- src/VoidEmpires.Infrastructure/Economy/PlanetEconomyTickService.cs
- tests/VoidEmpires.Tests/PlanetEconomyTickServiceTests.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs
- tests/VoidEmpires.Tests/GameplayRefreshServiceTests.cs

## Acceptance criteria

- Resources increase after elapsed time.
- Resources cap at capacity.
- Repeated refresh does not double accrue.

## Constraints

- No frontend timers.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
