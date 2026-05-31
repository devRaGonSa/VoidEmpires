# TASK-7E

---

id: TASK-7E
title: Add strategic map read model
status: in-progress
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* tests
* docs
  roadmap_item: "Phase 7E - Strategic map read model"
  priority: high

---

## Goal

Add a read-only strategic map read model that summarizes the current known galaxy/system state for a civilization.

This read model should prepare future map UI work by consolidating map-relevant state without introducing final UI, route graph pathfinding, combat, interception, alliances, espionage, or heavy render data.

## Context

The project already has:

* galaxy/system/planet persistence
* planet and system visual state services
* orbital group markers and transfer overlays
* fleet UI state
* route/fuel readiness previews
* development API documentation

The future strategic map needs a backend read model that can answer:

* which solar systems are relevant
* which planets are visible/owned/known
* ownership summary
* planet visual/colonization summary
* orbital presence summary
* active transfer/route overlay summary
* route/fuel capability notes
* map node coordinates/layout hints when available

This must remain a read model. It must not create gameplay behavior.

## Implementation steps

1. Inspect existing galaxy/system/planet entities and visual state services.
2. Inspect system visual state DTOs and services for coordinates, orbit layout hints, orbital group markers, and transfer overlays.
3. Inspect fleet UI state and action manifest contracts.
4. Add application contracts for strategic map read model, for example:

   * `GetStrategicMapRequest`
   * `GetStrategicMapResult`
   * `StrategicMapSystemDto`
   * `StrategicMapPlanetDto`
   * `StrategicMapFleetPresenceDto`
   * `StrategicMapTransferOverlayDto`
   * `StrategicMapRouteFuelNoteDto`
   * `IStrategicMapService`
5. Add infrastructure implementation that:

   * filters/scopes by `CivilizationId`
   * returns owned systems/planets and relevant nearby/known systems if current model supports known/visibility; otherwise document that Phase 7E returns owned/relevant persisted systems only
   * includes planet ids, names, type, ownership/civilization id, colonization status, and layout hints if available
   * includes aggregated orbital presence per planet/system
   * includes active transfer summaries/overlays
   * includes route/fuel capability notes as read-only metadata, not concrete estimates unless destination context exists
   * does not mutate anything
6. Prefer reusing existing visual state service output rather than duplicating layout calculations.
7. Add tests:

   * empty civilization returns empty or valid minimal map result
   * owned planet/system appears in map result
   * other civilization data is excluded unless current visibility model explicitly includes it
   * orbital presence summary appears for owned/current planet groups
   * active transfer overlay appears for active transfers
   * result is read-only and does not mutate persisted state
8. Update `ai/current-state.md` to document Phase 7E.

## Files to read first

* src/VoidEmpires.Application/Visuals/
* src/VoidEmpires.Infrastructure/Visuals/
* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* src/VoidEmpires.Domain/Galaxy/
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* tests/VoidEmpires.Tests/*VisualState*.cs
* tests/VoidEmpires.Tests/*Fleet*.cs
* tests/VoidEmpires.Tests/*Galaxy*.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on repository conventions:

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapRequest.cs
* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Application/StrategicMap/IStrategicMapService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* ai/current-state.md

If the repository has an existing namespace better suited than `StrategicMap`, follow existing conventions and keep the change minimal.

## Acceptance criteria

* A read-only strategic map service exists.
* The service is scoped by civilization.
* Owned/relevant systems and planets are returned.
* Planet visual/layout summary is included where available.
* Orbital presence summary is included.
* Active transfer summaries or overlays are included.
* Route/fuel metadata is represented as capability notes, not fabricated concrete estimates.
* No data is mutated.
* Tests cover service behavior.
* `ai/current-state.md` documents Phase 7E.

## Constraints

* Read-only only.
* Do not add final UI/frontend code.
* Do not add production gameplay endpoints.
* Do not add route graph/pathfinding.
* Do not add combat/interception.
* Do not add alliances or espionage.
* Do not add fuel inventory/refueling.
* Do not add migrations unless absolutely required; if a migration appears necessary, stop and create a follow-up task.
* Do not return heavy render data, meshes, textures, binary assets, or shader data.
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
   `feat(map): add strategic map read model`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
