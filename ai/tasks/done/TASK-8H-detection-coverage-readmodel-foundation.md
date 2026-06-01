# TASK-8H

---

id: TASK-8H
title: Add detection coverage read model foundation
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8H - Detection coverage read model foundation"
priority: high

---

## Goal

Add a lightweight, deterministic, read-only detection coverage read model derived from current sensor profiles and known/visible map state.

This phase must not add real detection gameplay. It must not reveal visibility automatically, persist detection state, add scanner mechanics, add espionage, add diplomacy, add combat, add interception, add route graph/pathfinding, add production endpoints, or add final UI.

The goal is to prepare stable metadata for future detection/interception systems.

## Context

The project now has read-only sensor profiles. Sensor profiles currently describe possible capability metadata but do not reveal anything or detect anything.

Detection coverage should now expose safe metadata such as:

* source kind: planet, orbital group, knowledge, none
* source id
* source system/planet context
* detection class/tier
* covered system ids if safely known/relevant
* confidence/coverage placeholder
* limitations/notes

This must remain conservative:

* use only data already visible/known/owned for the requesting civilization
* do not leak unknown or foreign-owned details
* do not create knowledge
* do not alter `MapVisibilityService`
* do not mutate persisted state

## Implementation steps

1. Inspect sensor profile contracts and service from Phase 8D.
2. Inspect map visibility service and strategic map service.
3. Add application contracts, for example:

   * `DetectionCoverageClass`
   * `DetectionCoverageSourceKind`
   * `DetectionCoverageDto`
   * `GetDetectionCoverageRequest`
   * `GetDetectionCoverageResult`
   * `IDetectionCoverageService`
4. Implement an infrastructure service:

   * scopes by `CivilizationId`
   * uses `ISensorProfileService` and/or the same source data
   * returns deterministic detection coverage metadata
   * only derives coverage from owned/known/visible context
   * does not reveal hidden systems/planets
   * does not persist anything
5. Suggested conservative placeholder rules:

   * owned colonized planet sensor profile => local system coverage metadata
   * scout orbital group sensor profile => current system coverage metadata
   * DeepSpace/Orbital sensor classes may still be placeholder notes, not actual range simulation
   * unknown targets remain unknown unless already revealed by exploration knowledge/ownership
6. Add tests:

   * empty civilization returns valid empty result
   * owned planet produces local detection coverage metadata
   * scout orbital group produces detection coverage metadata
   * other civilization data is excluded
   * unknown targets are not revealed by detection coverage
   * service is read-only/no mutation
7. Update `ai/current-state.md` to document Phase 8H.

## Files to read first

* src/VoidEmpires.Application/StrategicMap/*Sensor*.cs
* src/VoidEmpires.Infrastructure/StrategicMap/SensorProfileService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/MapVisibilityService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* tests/VoidEmpires.Tests/SensorProfileServiceTests.cs
* tests/VoidEmpires.Tests/SensorReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/MapVisibilityServiceTests.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on conventions:

* src/VoidEmpires.Application/StrategicMap/DetectionCoverageClass.cs
* src/VoidEmpires.Application/StrategicMap/DetectionCoverageSourceKind.cs
* src/VoidEmpires.Application/StrategicMap/GetDetectionCoverageRequest.cs
* src/VoidEmpires.Application/StrategicMap/GetDetectionCoverageResult.cs
* src/VoidEmpires.Application/StrategicMap/IDetectionCoverageService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DetectionCoverageService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* tests/VoidEmpires.Tests/DetectionCoverageServiceTests.cs
* ai/current-state.md

If repository conventions suggest another namespace, follow existing style and keep contracts lightweight.

## Acceptance criteria

* Read-only detection coverage service exists.
* Detection coverage is scoped by civilization.
* Owned planets and suitable orbital groups can produce deterministic coverage metadata.
* Other-civilization data is excluded.
* Unknown targets are not revealed by coverage.
* No persisted detection state is added.
* No gameplay behavior changes are introduced.
* Tests cover service behavior and read-only guarantees.
* `ai/current-state.md` documents Phase 8H.

## Constraints

* Do not persist detection state.
* Do not reveal visibility through detection.
* Do not mutate exploration knowledge.
* Do not add real sensor range simulation.
* Do not add scanner mechanics.
* Do not mutate resources/fleets/missions.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add migrations.
* Keep the model deterministic and conservative.

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
   `feat(detection): add coverage read model`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
