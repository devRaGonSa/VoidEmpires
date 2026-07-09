# TASK-49I

---
id: TASK-49I
title: Independent module queues
status: done
type: backend
team: backend
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Fix queue blockers so modules block only their own queue or real requirements.

## Context
Shipyard and defenses must not depend on construction queue availability unless their enabling building is being upgraded.

## Implementation steps

1. Inspect queue blocker logic for construction, research, shipyard, and defenses.
2. Remove defense/shipyard blockers based only on construction queue occupancy.
3. Add tests for independent queues.

## Files to read first

- src/VoidEmpires.Infrastructure/Assets/DevShipyardUiStateService.cs
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs
- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- src/VoidEmpires.Infrastructure/Assets/AssetProductionQueueService.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Assets/DevShipyardUiStateService.cs
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs
- tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Acceptance criteria

- Construction blocks construction only.
- Research blocks research only.
- Shipyard and defense production are independent from construction queue occupancy.
- Enabling building upgrade can still block relevant production.

## Constraints

- No combat or movement.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
