# TASK-8M

---

id: TASK-8M
title: Integrate interception readiness into strategic map and fleet UI metadata
status: done
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8M - Interception readiness map and fleet UI integration"
priority: high

---

## Goal

Integrate read-only interception readiness metadata into strategic map/readiness surfaces and fleet UI state.

This phase must not add real interception commands, combat, damage, interception resolution, route graph/pathfinding, production endpoints, or final UI.

The goal is UI/dev readiness metadata only.

## Context

Phase 8L adds interception opportunity metadata.

Strategic map and fleet UI state should now be able to surface:

* active transfer interception readiness notes
* whether a transfer is self-observed, undetected, detected, or theoretically interceptable
* why interception is blocked
* the fact that actual interception execution is not implemented

This must not change existing transfer behavior.

## Implementation steps

1. Read Phase 8L interception opportunity contracts/service.
2. Inspect strategic map DTOs and service:

   * transfer overlays
   * detection metadata
   * command availability
3. Inspect fleet UI state DTOs and service:

   * groups
   * active transfer summaries
   * action hints
4. Add lightweight interception readiness metadata where appropriate:

   * top-level strategic map notes
   * per-transfer overlay interception readiness metadata
   * fleet UI action hints or transfer readiness notes
5. Ensure existing transfer create/cancel/complete behavior is unchanged.
6. Do not add a command execution endpoint.
7. Add tests:

   * strategic map transfer overlay includes interception readiness metadata
   * fleet UI state includes interception readiness/action hint metadata
   * own transfers are not hostile/interceptable
   * hidden foreign details are not leaked
   * read surfaces remain read-only
8. Update `docs/dev/strategic-map-api-contract.md` and `docs/dev/fleet-api-contracts.md`.
9. Update `ai/current-state.md` to document Phase 8M.

## Files to read first

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Application/Fleets/GetDevFleetUiStateResult.cs
* src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs
* src/VoidEmpires.Application/StrategicMap/*Interception*.cs
* src/VoidEmpires.Infrastructure/StrategicMap/InterceptionOpportunityService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Application/Fleets/GetDevFleetUiStateResult.cs
* src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md
* ai/current-state.md

May also modify:

* src/VoidEmpires.Application/StrategicMap/*Interception*.cs
* tests/VoidEmpires.Tests/InterceptionOpportunityServiceTests.cs

## Acceptance criteria

* Strategic map surfaces read-only interception readiness metadata.
* Fleet UI state surfaces read-only interception readiness/action hint metadata.
* Own transfers are not treated as hostile/interceptable.
* Hidden foreign data is not leaked.
* Existing transfer behavior is unchanged.
* No interception command execution is added.
* Tests cover integration behavior.
* Docs are updated.
* `ai/current-state.md` documents Phase 8M.

## Constraints

* Do not add interception execution.
* Do not add combat, damage, battle result, or interception resolution.
* Do not persist interception state.
* Do not reveal visibility through interception readiness.
* Do not mutate resources, fleets, transfers, exploration knowledge, or missions.
* Do not add espionage or diplomacy.
* Do not add route graph/pathfinding.
* Do not add production endpoints.
* Do not add final UI/frontend code.
* Keep payload lightweight.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Expected result:

* clean build
* 0 errors
* no new warnings
* all tests passing

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected files changed.
4. Commit with a clear message, for example:
   `feat(map): surface interception readiness metadata`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
