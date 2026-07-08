# TASK-47E

---
id: TASK-47E
title: Fix starting planet building capacity
status: pending
type: backend
team: backend
supporting_teams: []
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: high
---

## Goal
Ensure newly registered/home planets have valid building capacity/fields.

## Context
Basic construction must be playable on a newly registered home planet without missing-capacity blockers.

## Implementation steps

1. Fix the relevant bootstrap or capacity derivation path.
2. Preserve existing SQL Server/manual seed behavior.
3. Add tests that a newly registered home planet has capacity, used fields count, and can evaluate construction catalog without the missing-capacity blocker.

## Files to read first

- src/VoidEmpires.Infrastructure/Services/InitialPlayerWorldBootstrapService.cs
- src/VoidEmpires.Infrastructure/Services/DevConstructionUiStateService.cs
- src/VoidEmpires.Domain/Gameplay/Construction/PlanetBuildingCapacity.cs
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs
- tests/VoidEmpires.Tests/DevConstructionUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Services/InitialPlayerWorldBootstrapService.cs
- src/VoidEmpires.Infrastructure/Services/DevConstructionUiStateService.cs
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs
- tests/VoidEmpires.Tests/DevConstructionUiStateEndpointTests.cs

## Acceptance criteria

- New/home planets expose valid building capacity.
- Basic buildings are not blocked solely because capacity is missing.

## Constraints

- Do not bypass real costs or requirements.
- Do not require SQL Server for automated tests.

## Validation

- dotnet build --no-restore
- dotnet test --no-build

