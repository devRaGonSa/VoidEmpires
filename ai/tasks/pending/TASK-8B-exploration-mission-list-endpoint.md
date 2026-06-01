# TASK-8B

---

id: TASK-8B
title: Add exploration mission list endpoint
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - web
  - tests
  - docs
roadmap_item: "Phase 8B - Exploration mission list endpoint"
priority: high

---

## Goal

Add a read-only exploration mission list service and Development-only endpoint so dev tooling can inspect planned/completed exploration missions for a civilization.

This endpoint is for development tooling only. It must not create, complete, cancel, or mutate missions.

## Context

The project now has:

* `ExplorationMission`
* create mission endpoint/service
* complete due missions endpoint/service
* knowledge creation on completion
* knowledge read endpoint from TASK-7Z

Dev tooling needs a direct read model for mission state.

Suggested endpoint:

* `GET /api/dev/strategic-map/exploration-missions?civilizationId={id}&status={optional}`

Status filter should be optional. Keep the contract simple.

## Implementation steps

1. Inspect `ExplorationMission` model and existing mission create/complete contracts.
2. Add application contracts:

   * `GetExplorationMissionsRequest`
   * `GetExplorationMissionsResult`
   * `ExplorationMissionDto`
   * `IExplorationMissionQueryService`
3. Implement infrastructure service:

   * validates non-empty civilization id
   * optional status filter
   * returns only requesting civilization missions
   * orders deterministically by requested/due timestamp then id
   * includes mission id, target ids, status, requested/due/completed timestamps
4. Add Development-only endpoint:

   * `GET /api/dev/strategic-map/exploration-missions?civilizationId={id}&status={optional}`
5. Endpoint behavior:

   * disabled dev routes return `404`
   * missing persistence returns `503`
   * missing/empty civilization id returns `400`
   * invalid status returns `400`
   * success returns `200`
6. Update strategic map action manifest:

   * add `exploration.mission.list`
7. Add tests:

   * service scopes by civilization
   * service filters by status
   * service orders deterministically
   * endpoint gating/status behavior
   * endpoint success
   * endpoint invalid status
   * read-only/no mutation
8. Update docs.
9. Update `ai/current-state.md` to document Phase 8B.

## Files to read first

* src/VoidEmpires.Domain/Exploration/
* src/VoidEmpires.Application/StrategicMap/
* src/VoidEmpires.Infrastructure/StrategicMap/
* src/VoidEmpires.Web/DevExplorationMissionEndpoints.cs
* tests/VoidEmpires.Tests/*ExplorationMission*.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Application/StrategicMap/GetExplorationMissionsRequest.cs
* src/VoidEmpires.Application/StrategicMap/GetExplorationMissionsResult.cs
* src/VoidEmpires.Application/StrategicMap/IExplorationMissionQueryService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/ExplorationMissionQueryService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevExplorationMissionEndpoints.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* tests/VoidEmpires.Tests/ExplorationMissionQueryServiceTests.cs
* tests/VoidEmpires.Tests/DevExplorationMissionEndpointTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

## Acceptance criteria

* Exploration mission query service exists.
* Dev-only mission list endpoint exists.
* Results are scoped by civilization.
* Optional status filter works.
* Endpoint follows current dev gating conventions.
* Mission list read is read-only.
* Action manifest includes list action.
* Tests cover service and endpoint behavior.
* Docs are updated.
* `ai/current-state.md` documents Phase 8B.

## Constraints

* Do not create, complete, cancel, or mutate missions.
* Do not reveal additional visibility.
* Do not add sensors/scanners.
* Do not add rewards.
* Do not mutate resources/fleets.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Keep payload lightweight and deterministic.

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
   `feat(exploration): add mission list endpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
