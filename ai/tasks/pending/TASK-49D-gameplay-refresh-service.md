# TASK-49D

---
id: TASK-49D
title: Gameplay refresh service
status: pending
type: backend
team: backend
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Create or consolidate a backend GameplayRefreshService.

## Context
Normal read flows need a backend-authoritative refresh that accrues resources and completes due queues idempotently.

## Implementation steps

1. Inspect existing economy tick and queue completion services.
2. Add application contract and infrastructure implementation for gameplay refresh.
3. Wire it through DI.

## Files to read first

- ai/orchestrator/di-analysis.md
- src/VoidEmpires.Application/Economy/IPlanetEconomyTickService.cs
- src/VoidEmpires.Infrastructure/Economy/PlanetEconomyTickService.cs
- src/VoidEmpires.Infrastructure/Gameplay/GameplayQueueMaterializationService.cs
- src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs

## Expected files to modify

- src/VoidEmpires.Application/Gameplay/IGameplayRefreshService.cs
- src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs
- src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
- tests/VoidEmpires.Tests/GameplayRefreshServiceTests.cs

## Acceptance criteria

- Refresh is idempotent.
- Refresh can accrue resources and complete due construction, research, and supported production.

## Constraints

- Tests must not require SQL Server.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
