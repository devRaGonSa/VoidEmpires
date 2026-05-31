# TASK-7I

---

id: TASK-7I
title: Add map visibility read model foundation
status: done
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* tests
* docs
  roadmap_item: "Phase 7I - Map visibility read model foundation"
  priority: high

---

## Goal

Add a lightweight, read-only map visibility read model for civilization-scoped strategic map work.

This must establish how the backend represents whether systems and planets are owned, known, visible, or unknown to a civilization.

This phase must remain read-only. It must not add real exploration missions, sensors, fog-of-war persistence, espionage, diplomacy, combat, interception, route graph/pathfinding, or final UI.

## Context

The project already has a strategic map read model and dev endpoint. It currently returns owned/relevant persisted map data. Future UI needs a stable way to distinguish:

* owned systems/planets
* currently visible systems/planets
* known but not owned systems/planets
* unknown/unavailable systems/planets
* why a map node is visible or hidden

There may not yet be a persisted exploration/fog-of-war model. If none exists, this task should implement a derived read-only visibility model based on current ownership and persisted data, then document the limitation clearly.

## Implementation steps

1. Inspect current galaxy/system/planet persistence models.
2. Inspect ownership/civilization models.
3. Inspect strategic map service from Phase 7E.
4. Add application contracts for visibility, for example:

   * `MapVisibilityLevel`
   * `MapVisibilityReason`
   * `MapSystemVisibilityDto`
   * `MapPlanetVisibilityDto`
   * `GetMapVisibilityRequest`
   * `GetMapVisibilityResult`
   * `IMapVisibilityService`
5. Add infrastructure implementation that:

   * scopes by `CivilizationId`
   * marks owned planets/systems as owned/visible
   * marks systems containing owned planets as visible
   * marks other persisted systems as unknown or not visible unless current model has explicit known/visibility support
   * does not invent exploration data
   * does not persist anything
6. If the existing model already has knowledge/visibility concepts, reuse them.
7. Add tests:

   * owned planet is visible/owned
   * system containing owned planet is visible
   * other civilization's owned planet is not exposed as owned
   * unknown/not-visible nodes are represented according to selected convention
   * service is read-only and does not mutate persisted state
8. Update `ai/current-state.md` to document Phase 7I and current visibility limitations.

## Files to read first

* src/VoidEmpires.Domain/Galaxy/
* src/VoidEmpires.Domain/Players/
* src/VoidEmpires.Domain/Colonization/
* src/VoidEmpires.Application/StrategicMap/
* src/VoidEmpires.Infrastructure/StrategicMap/
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* tests/VoidEmpires.Tests/*StrategicMap*.cs
* tests/VoidEmpires.Tests/*Galaxy*.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on repository conventions:

* src/VoidEmpires.Application/StrategicMap/MapVisibilityLevel.cs
* src/VoidEmpires.Application/StrategicMap/MapVisibilityReason.cs
* src/VoidEmpires.Application/StrategicMap/GetMapVisibilityRequest.cs
* src/VoidEmpires.Application/StrategicMap/GetMapVisibilityResult.cs
* src/VoidEmpires.Application/StrategicMap/IMapVisibilityService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/MapVisibilityService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* tests/VoidEmpires.Tests/MapVisibilityServiceTests.cs
* ai/current-state.md

If current conventions place visibility under a different namespace, follow repository style.

## Acceptance criteria

* A read-only map visibility service exists.
* Visibility is scoped by civilization.
* Owned planets/systems are marked visible/owned.
* Other-civilization data is not incorrectly marked as owned.
* The current unknown/not-visible convention is explicit and tested.
* No exploration/fog-of-war state is persisted.
* No gameplay behavior changes are introduced.
* `ai/current-state.md` documents Phase 7I.

## Constraints

* Read-only only.
* Do not add persisted fog-of-war/exploration state.
* Do not add exploration missions.
* Do not add sensors/scanners.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add migrations unless absolutely required; if a migration appears necessary, stop and create a follow-up task.
* Keep contracts lightweight and deterministic.

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
   `feat(map): add visibility read model foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
