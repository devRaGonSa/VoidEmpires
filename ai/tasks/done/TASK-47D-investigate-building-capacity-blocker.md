# TASK-47D

---
id: TASK-47D
title: Investigate missing building capacity blocker
status: done
type: backend
team: backend
supporting_teams: [frontend]
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: high
---

## Goal
Investigate why starting planet buildings can be blocked by missing building capacity.

## Context
The card message "No se encontro la capacidad de edificios del planeta" is a functional defect for newly registered player construction.

## Implementation steps

1. Trace the source of the missing building capacity message.
2. Inspect backend/view-model logic for capacity, fields, used fields, max slots, bootstrap, and seed planets.
3. Determine whether the root cause is bootstrap data, DTO shape, catalog requirements, frontend null/zero handling, or view-model logic.
4. Document the root cause in task notes/current state.

## Files to read first

- src/VoidEmpires.Infrastructure/Services/InitialPlayerWorldBootstrapService.cs
- src/VoidEmpires.Infrastructure/Services/DevConstructionUiStateService.cs
- src/VoidEmpires.Domain/Gameplay/Construction/PlanetBuildingCapacity.cs
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs
- tests/VoidEmpires.Tests/DevConstructionUiStateEndpointTests.cs

## Expected files to modify

- ai/current-state.md
- ai/tasks/pending/TASK-47D-investigate-building-capacity-blocker.md

## Acceptance criteria

- The root cause is documented.
- No speculative workaround is accepted as the investigation result.

## Constraints

- Do not hide the blocker with copy-only changes.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
