# TASK-8I

---

id: TASK-8I
title: Integrate detection coverage into strategic map metadata
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8I - Detection coverage strategic map integration"
priority: high

---

## Goal

Integrate read-only detection coverage metadata into strategic map/readiness surfaces.

This phase must not change visibility reveal rules, reveal unknown targets, add real detection mechanics, persist detection state, or introduce combat/interception behavior.

The goal is UI/dev readiness only.

## Context

Phase 8H adds detection coverage metadata derived from current sensor profiles and visible/known/owned context.

Strategic map should be able to expose detection notes so future UI prototypes can understand:

* which systems/planets have local detection coverage metadata
* which sensor sources contribute metadata
* current limitations
* that detection coverage does not reveal unknown systems/planets yet

## Implementation steps

1. Read Phase 8H detection coverage contracts/service.
2. Inspect strategic map DTOs and service:

   * `GetStrategicMapResult`
   * `StrategicMapSystemDto`
   * `StrategicMapPlanetDto`
   * `StrategicMapFleetPresenceDto`
   * existing sensor metadata integration
3. Add lightweight detection metadata to strategic map result where appropriate, for example:

   * top-level `DetectionNotes`
   * per-system detection coverage summaries
   * per-planet detection coverage summaries if clean
4. Ensure `StrategicMapService` uses `IDetectionCoverageService` or equivalent.
5. Do not use detection coverage to reveal visibility.
6. Do not change command validation.
7. Add tests:

   * owned/known strategic map item includes detection metadata
   * scout/fleet context contributes detection metadata if current service supports it
   * unknown nodes do not gain revealed detail from detection metadata
   * strategic map remains read-only
8. Update `docs/dev/strategic-map-api-contract.md`.
9. Update `ai/current-state.md` to document Phase 8I.

## Files to read first

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Application/StrategicMap/*Detection*.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DetectionCoverageService.cs
* src/VoidEmpires.Application/StrategicMap/*Sensor*.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DetectionCoverageServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

May also modify:

* src/VoidEmpires.Application/StrategicMap/*Detection*.cs
* tests/VoidEmpires.Tests/DetectionCoverageServiceTests.cs

## Acceptance criteria

* Strategic map exposes read-only detection metadata.
* Detection metadata does not reveal unknown data.
* Detection metadata does not change visibility or command validation.
* Docs explicitly state detection is metadata only at this stage.
* Tests cover integration behavior.
* `ai/current-state.md` documents Phase 8I.

## Constraints

* Do not reveal visibility through detection.
* Do not add real detection/scanner mechanics.
* Do not persist detection state.
* Do not mutate exploration knowledge.
* Do not mutate resources/fleets/missions.
* Do not add espionage, diplomacy, combat, or interception.
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
   `feat(map): surface detection coverage metadata`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
