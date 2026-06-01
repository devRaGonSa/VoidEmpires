# TASK-7R

---

id: TASK-7R
title: Add exploration mission creation service and dev endpoint
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - web
  - tests
  - docs
roadmap_item: "Phase 7R - Exploration mission creation service and dev endpoint"
priority: high

---

## Goal

Add a minimal exploration mission creation service and Development-only endpoint.

This creates an exploration mission but does not complete it and does not reveal visibility.

The endpoint is dev-only. No production endpoint should be added.

## Context

Phase 7Q adds persistent exploration missions.
The project already has exploration preview metadata and strategic map command availability.

Now we need to create a mission from a preview-eligible unknown system/planet.

This must remain conservative:

- validate requesting civilization
- validate target system exists
- validate optional target planet exists and belongs to the target system
- allow creation only when exploration preview says the target is eligible
- set deterministic due time using a simple placeholder duration
- no resource costs
- no fleet assignment
- no sensors/scanners
- no visibility reveal
- no fog-of-war mutation

## Implementation steps

1. Read Phase 7Q mission model.
2. Read exploration preview service and map visibility service.
3. Add application contracts:
   - `CreateExplorationMissionRequest`
   - `CreateExplorationMissionResult`
   - `IExplorationMissionCreateService`
4. Implement infrastructure service:
   - validates ids
   - validates `RequestedAtUtc` is UTC
   - validates target system exists
   - validates optional target planet exists and belongs to target system
   - validates exploration preview eligibility for the target
   - creates and saves `ExplorationMission`
   - returns mission id, target ids, status, requested/due timestamps
5. Placeholder duration:
   - simple deterministic duration, for example 30 minutes for system-level preview and 45 minutes for planet-level preview
   - document in tests/comments
6. Add development-only endpoint:
   - suggested route: `POST /api/dev/strategic-map/exploration-missions/create`
7. Endpoint behavior:
   - mapped only in Development or `VoidEmpires:DevEndpoints:Enabled=true`
   - missing persistence returns `503`
   - invalid request returns `400`
   - rejected by current visibility/eligibility returns `409`
   - success returns `201 Created`
8. Update strategic map action manifest with:
   - `exploration.mission.create`
   - method POST
   - route
   - mutating/read-only flag false
   - required fields
   - success/error statuses
9. Add tests:
   - service creates mission for unknown system
   - service creates mission for unknown planet
   - service rejects visible/owned target
   - service rejects planet not in system
   - endpoint gating/status behavior
   - endpoint success
   - no visibility reveal occurs
10. Update `docs/dev/strategic-map-api-contract.md`.
11. Update `ai/current-state.md` to document Phase 7R.

## Files to read first

- `src/VoidEmpires.Domain/Exploration/`
- `src/VoidEmpires.Application/StrategicMap/`
- `src/VoidEmpires.Infrastructure/StrategicMap/`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `src/VoidEmpires.Web/DevExplorationActionPreviewEndpoints.cs`
- `src/VoidEmpires.Web/Program.cs`
- `tests/VoidEmpires.Tests/ExplorationActionPreviewServiceTests.cs`
- `tests/VoidEmpires.Tests/DevExplorationActionPreviewEndpointTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected, depending on conventions:

- `src/VoidEmpires.Application/StrategicMap/CreateExplorationMissionRequest.cs`
- `src/VoidEmpires.Application/StrategicMap/CreateExplorationMissionResult.cs`
- `src/VoidEmpires.Application/StrategicMap/IExplorationMissionCreateService.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/ExplorationMissionCreateService.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `src/VoidEmpires.Web/DevExplorationMissionEndpoints.cs`
- `src/VoidEmpires.Web/Program.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionCreateServiceTests.cs`
- `tests/VoidEmpires.Tests/DevExplorationMissionEndpointTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`

## Acceptance criteria

- Mission creation service exists.
- Dev-only create endpoint exists.
- Unknown/preview-eligible target can create a mission.
- Visible/owned target is rejected.
- Optional target planet must belong to target system.
- Mission due time is deterministic and UTC.
- No visibility is revealed.
- No fog-of-war/known-system state is created.
- No resource costs or fleet assignment are added.
- Action manifest documents the create action.
- Tests cover service and endpoint behavior.
- Docs are updated.
- `ai/current-state.md` documents Phase 7R.

## Constraints

- Do not add production endpoint.
- Do not reveal visibility.
- Do not complete missions.
- Do not add worker.
- Do not add sensors/scanners.
- Do not add resource costs.
- Do not assign fleets.
- Do not add espionage, diplomacy, combat, or interception.
- Do not add route graph/pathfinding.
- Do not add UI.
- Keep the create flow minimal and deterministic.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Expected result:

- clean build
- 0 errors
- no new warnings
- all tests passing

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected files changed.
4. Commit with a clear message, for example:
   `feat(exploration): add mission creation dev endpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
