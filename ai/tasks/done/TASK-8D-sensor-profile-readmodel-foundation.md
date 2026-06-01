# TASK-8D

---

id: TASK-8D
title: Add sensor profile read model foundation
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - domain
  - tests
  - docs
roadmap_item: "Phase 8D - Sensor profile read model foundation"
priority: high

---

## Goal

Add a lightweight, deterministic, read-only sensor profile foundation.

This phase must not add persisted sensor state, sensor range simulation, scanner mechanics, espionage, diplomacy, combat, interception, route graph/pathfinding, production endpoints, or final UI.

The goal is to expose stable sensor metadata that future exploration, detection, espionage, and interception systems can build on.

## Context

The project now has exploration missions, exploration knowledge, and map visibility reveal. Before adding real sensor mechanics, we need a conservative read-only model that describes potential sensor capability by existing game concepts.

Possible inputs:

* `SpaceAssetType`
* planet ownership/colonization status
* orbital group asset composition
* existing visibility/knowledge state

For now, sensor profile should be derived, deterministic, and non-persistent.

Examples of sensor metadata:

* sensor class: None, Basic, Orbital, DeepSpace
* detection band/range tier placeholder
* scan strength placeholder
* source kind: Planet, OrbitalGroup, ExplorationKnowledge, None
* notes/limitations

## Implementation steps

1. Inspect existing domain/application models:

   * `SpaceAssetType`
   * `OrbitalGroup`
   * `Planet`
   * `PlanetOwnership`
   * strategic map and visibility DTOs
2. Add application contracts for sensor profiles, for example:

   * `SensorProfileClass`
   * `SensorProfileSourceKind`
   * `SensorProfileDto`
   * `GetSensorProfilesRequest`
   * `GetSensorProfilesResult`
   * `ISensorProfileService`
3. Implement a deterministic service, likely under infrastructure:

   * scopes by `CivilizationId`
   * derives owned planet sensor profiles
   * derives stationed orbital group sensor profiles if current data supports it
   * uses safe placeholder values only
   * does not persist anything
   * does not reveal new visibility
4. Keep the mapping intentionally simple, for example:

   * owned colonized planet => Basic or Orbital profile
   * stationed scout craft group => Basic/Orbital profile
   * other asset types may have None/Basic placeholder profiles
5. Add tests:

   * empty civilization returns valid empty result
   * owned planet produces deterministic sensor profile
   * scout orbital group produces deterministic sensor profile
   * other civilization data is excluded
   * service is read-only/no mutation
6. Update `ai/current-state.md` to document Phase 8D.

## Files to read first

* src/VoidEmpires.Domain/Assets/
* src/VoidEmpires.Domain/Fleets/
* src/VoidEmpires.Domain/Galaxy/
* src/VoidEmpires.Domain/Colonization/
* src/VoidEmpires.Application/StrategicMap/
* src/VoidEmpires.Infrastructure/StrategicMap/
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* tests/VoidEmpires.Tests/*StrategicMap*.cs
* tests/VoidEmpires.Tests/*Fleet*.cs
* tests/VoidEmpires.Tests/*Exploration*.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on repository conventions:

* src/VoidEmpires.Application/StrategicMap/SensorProfileClass.cs
* src/VoidEmpires.Application/StrategicMap/SensorProfileSourceKind.cs
* src/VoidEmpires.Application/StrategicMap/GetSensorProfilesRequest.cs
* src/VoidEmpires.Application/StrategicMap/GetSensorProfilesResult.cs
* src/VoidEmpires.Application/StrategicMap/ISensorProfileService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/SensorProfileService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* tests/VoidEmpires.Tests/SensorProfileServiceTests.cs
* ai/current-state.md

If the repository has a better namespace for sensors, follow existing conventions and keep the contracts lightweight.

## Acceptance criteria

* Read-only sensor profile service exists.
* Sensor profiles are scoped by civilization.
* Owned planets and suitable orbital groups can produce deterministic profiles.
* Other-civilization data is excluded.
* No visibility is revealed.
* No persisted sensor state is added.
* No gameplay behavior changes are introduced.
* Tests cover service behavior and read-only guarantees.
* `ai/current-state.md` documents Phase 8D.

## Constraints

* Do not add persisted sensor/scanner state.
* Do not add real sensor range simulation.
* Do not reveal visibility.
* Do not create exploration knowledge.
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
   `feat(sensors): add sensor profile read model`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
