# TASK-7J

---

id: TASK-7J
title: Integrate visibility into strategic map
status: pending
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* web
* tests
* docs
  roadmap_item: "Phase 7J - Strategic map visibility integration"
  priority: high

---

## Goal

Integrate the Phase 7I map visibility read model into the existing strategic map read model and development endpoint.

The strategic map should expose visibility information for systems and planets so future UI can render owned/visible/unknown map nodes correctly.

This phase must remain read-only.

## Context

Phase 7E added the strategic map read model.
Phase 7F added the strategic map dev endpoint and docs.
Phase 7I adds map visibility foundation.

Now strategic map payloads should include visibility data, while preserving current behavior and scope.

The endpoint should continue to avoid heavy render data and should not add real exploration, fog-of-war persistence, sensors, espionage, diplomacy, or combat.

## Implementation steps

1. Read the Phase 7I map visibility implementation.
2. Inspect current strategic map contracts and service.
3. Extend strategic map DTOs with visibility fields, for example:

   * system visibility level/reason
   * planet visibility level/reason
   * booleans such as `IsOwned`, `IsVisible`, `IsKnown` only if consistent with repo style
4. Update `StrategicMapService` to use `IMapVisibilityService`.
5. Ensure visibility does not leak other-civilization ownership incorrectly.
6. Update the dev endpoint response if needed.
7. Update strategic map docs to describe visibility fields and limitations.
8. Add tests:

   * owned planet/system appears with owned/visible status
   * non-owned/non-visible data follows the selected unknown convention
   * endpoint returns visibility fields
   * visibility integration remains read-only
9. Update `docs/dev/strategic-map-api-contract.md`.
10. Update `ai/current-state.md` to document Phase 7J.

## Files to read first

* src/VoidEmpires.Application/StrategicMap/
* src/VoidEmpires.Infrastructure/StrategicMap/
* src/VoidEmpires.Web/DevStrategicMapEndpoints.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapEndpointTests.cs
* tests/VoidEmpires.Tests/MapVisibilityServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on repository conventions:

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Web/DevStrategicMapEndpoints.cs if response wrappers need updates
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapEndpointTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

If strategic map DTOs are split into multiple files, follow current naming style.

## Acceptance criteria

* Strategic map payload includes visibility information.
* Visibility is scoped by civilization.
* Owned data is marked correctly.
* Unknown/not-visible convention is represented clearly.
* Other-civilization ownership is not leaked incorrectly.
* Dev endpoint remains read-only and gated.
* Tests cover service and endpoint behavior.
* Docs are updated.
* `ai/current-state.md` documents Phase 7J.

## Constraints

* Read-only only.
* Do not add persisted fog-of-war/exploration state.
* Do not add exploration missions.
* Do not add sensors/scanners.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not return heavy render data.
* Do not add migrations.
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
   `feat(map): integrate visibility into strategic map`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
