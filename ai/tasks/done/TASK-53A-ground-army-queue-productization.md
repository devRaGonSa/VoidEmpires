# TASK-53A

---
id: TASK-53A
title: Ground Army queue productization
status: done
type: product
team: gameplay
supporting_teams: [platform]
roadmap_item: "Block 53"
priority: high
---

## Goal
Make Ground Army production use the authoritative asset-production queue with ground-only active rows, correct locking, and due-order materialization.

## Context
The Ground Army read model must expose only open planetary ground-unit orders and must preserve independence from defense production while reusing the existing queue and materialization services.

## Implementation steps

1. Inspect the Ground Army read model, asset queue service, materializer, endpoint, and focused tests.
2. Correct filtering, locking, enqueue, and due-order refresh behavior without adding schema.
3. Add focused backend regression coverage for totals, resource spending, queue isolation, and materialization.

## Files to read first

- `src/VoidEmpires.Infrastructure/Planets/DevGroundArmyUiStateService.cs`
- `src/VoidEmpires.Infrastructure/Assets/AssetProductionQueueService.cs`
- `src/VoidEmpires.Infrastructure/Gameplay/GameplayQueueMaterializationService.cs`
- `src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs`
- `tests/VoidEmpires.Tests/DevGroundArmyUiStateEndpointTests.cs`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Planets/DevGroundArmyUiStateService.cs`
- `src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs`
- `tests/VoidEmpires.Tests/DevGroundArmyUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/AssetProductionOrderTests.cs`

## Acceptance criteria

- Only pending or active ground-unit orders appear in Ground Army UI state.
- Ground-unit orders lock only the ground catalog; defense orders remain independent.
- Quantity enqueue uses authoritative costs, duration, population, ownership, and requirements.
- Due orders materialize before the active UI-state response and increase stock exactly once.

## Constraints

- Reuse existing asset production and materialization services.
- No schema, combat, invasion, or unrelated UI changes.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

Commit this task separately on the Block 53 branch; push after the complete Block 53 cycle validates.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
