# TASK-49F

---
id: TASK-49F
title: Due construction completion normal flow
status: pending
type: backend
team: backend
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Complete due construction orders during normal refresh/read flow.

## Context
Due construction should advance building state without manual dev buttons.

## Implementation steps

1. Inspect construction completion service.
2. Ensure gameplay refresh calls it safely.
3. Add tests for building level, closed order, and next catalog unlock.

## Files to read first

- src/VoidEmpires.Infrastructure/Buildings/ConstructionOrderCompletionService.cs
- src/VoidEmpires.Infrastructure/Buildings/PlanetConstructionQueueService.cs
- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- tests/VoidEmpires.Tests/ConstructionOrderCompletionServiceTests.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs
- tests/VoidEmpires.Tests/GameplayRefreshServiceTests.cs

## Acceptance criteria

- Due construction orders complete during refresh.
- Catalog unlocks next actions correctly after read refresh.

## Constraints

- No manual/dev-only trigger required.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
