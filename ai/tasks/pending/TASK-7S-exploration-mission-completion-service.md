# TASK-7S

---

id: TASK-7S
title: Add exploration mission completion service
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 7S - Exploration mission completion service"
priority: high

---

## Goal

Add a minimal exploration mission completion service.

This service should mark due exploration missions as completed. It must not reveal real fog-of-war or create a complex visibility/knowledge model yet.

## Context

Phase 7Q adds the mission persistence foundation.
Phase 7R adds creation.

Now we need the first completion path:

- find due planned missions
- mark them completed
- return completed ids
- do not reveal map state yet
- do not create known-system/sensor/fog-of-war persistence

This keeps the mission lifecycle testable without introducing the full exploration visibility model.

## Implementation steps

1. Read Phase 7Q and 7R implementation.
2. Add application contracts:
   - `CompleteDueExplorationMissionsRequest`
   - `CompleteDueExplorationMissionsResult`
   - `IExplorationMissionCompletionService`
3. Implement infrastructure service:
   - validates `NowUtc` is UTC
   - finds planned missions with `DueAtUtc <= NowUtc`
   - completes them
   - saves changes
   - returns completed count and mission ids
4. Add development-only endpoint if consistent with current dev patterns:
   - suggested route: `POST /api/dev/strategic-map/exploration-missions/complete-due`
5. Endpoint behavior:
   - missing persistence returns `503`
   - invalid/non-UTC now returns `400`
   - success returns `200`
6. Add action manifest metadata:
   - `exploration.mission.completeDue`
7. Add tests:
   - completes due planned mission
   - does not complete future planned mission
   - rejects non-UTC timestamp
   - does not reveal visibility or create fog-of-war state
   - endpoint behavior if endpoint added
8. Update docs.
9. Update `ai/current-state.md` to document Phase 7S.

## Files to read first

- `src/VoidEmpires.Domain/Exploration/`
- `src/VoidEmpires.Application/StrategicMap/`
- `src/VoidEmpires.Infrastructure/StrategicMap/`
- `src/VoidEmpires.Web/DevExplorationMissionEndpoints.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionCreateServiceTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected:

- `src/VoidEmpires.Application/StrategicMap/CompleteDueExplorationMissionsRequest.cs`
- `src/VoidEmpires.Application/StrategicMap/CompleteDueExplorationMissionsResult.cs`
- `src/VoidEmpires.Application/StrategicMap/IExplorationMissionCompletionService.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/ExplorationMissionCompletionService.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `src/VoidEmpires.Web/DevExplorationMissionEndpoints.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionCompletionServiceTests.cs`
- `tests/VoidEmpires.Tests/DevExplorationMissionEndpointTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`

## Acceptance criteria

- Completion service exists.
- Due planned missions are completed.
- Future missions are left unchanged.
- Non-UTC request is rejected.
- Completion does not reveal map visibility.
- Completion does not create fog-of-war/known-system/sensor state.
- Dev endpoint exists if consistent with current patterns.
- Action manifest includes completion action if endpoint exists.
- Tests cover service and endpoint behavior.
- Docs are updated.
- `ai/current-state.md` documents Phase 7S.

## Constraints

- Do not reveal visibility.
- Do not create known-system/fog-of-war/sensor persistence.
- Do not add resource rewards.
- Do not add combat/interception.
- Do not add espionage or diplomacy.
- Do not add route graph/pathfinding.
- Do not add final UI.
- Do not add background worker unless explicitly needed; a service/endpoint is enough for this phase.
- Keep completion minimal and deterministic.

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
   `feat(exploration): add mission completion service`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
