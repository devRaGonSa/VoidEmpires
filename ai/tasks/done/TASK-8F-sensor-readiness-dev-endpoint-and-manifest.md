# TASK-8F

---

id: TASK-8F
title: Add sensor readiness dev endpoint and manifest metadata
status: pending
type: feature
team: backend
supporting_teams:
  - web
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8F - Sensor readiness dev endpoint and manifest metadata"
priority: high

---

## Goal

Expose sensor profiles through a Development-only read endpoint and strategic map action manifest metadata.

This is dev tooling only. It must not add production endpoints or gameplay effects.

## Context

Phase 8D adds a sensor profile service.
Phase 8E integrates sensor metadata into strategic map.

Now dev tooling should be able to inspect sensor readiness directly.

Suggested endpoint:

* `GET /api/dev/strategic-map/sensor-profiles?civilizationId={id}`

Manifest action:

* `sensor.profile.read`

## Implementation steps

1. Read sensor profile contracts/service.
2. Add Development-only endpoint:

   * `GET /api/dev/strategic-map/sensor-profiles?civilizationId={id}`
3. Endpoint behavior:

   * disabled dev routes return `404`
   * missing persistence returns `503`
   * missing/empty civilization id returns `400`
   * success returns `200`
4. Add action manifest metadata:

   * `sensor.profile.read`
   * method GET
   * route `/api/dev/strategic-map/sensor-profiles`
   * read-only true
   * required field `civilizationId`
   * common status codes
   * notes that profiles are metadata only and do not reveal or detect anything yet
5. Add tests:

   * endpoint gating
   * endpoint persistence missing
   * endpoint bad request
   * endpoint success
   * manifest includes sensor profile action
   * endpoint is read-only/no mutation
6. Update `docs/dev/strategic-map-api-contract.md`.
7. Update `ai/current-state.md` to document Phase 8F.

## Files to read first

* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Web/DevExplorationKnowledgeEndpoints.cs
* src/VoidEmpires.Web/DevStrategicMapEndpoints.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* src/VoidEmpires.Application/StrategicMap/GetSensorProfilesResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/SensorProfileService.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Web/DevSensorProfileEndpoints.cs
* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* tests/VoidEmpires.Tests/DevSensorProfileEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

## Acceptance criteria

* Sensor profile dev endpoint exists.
* Endpoint follows current dev gating conventions.
* Endpoint returns read-only sensor profile result.
* Action manifest includes sensor profile read action.
* Docs describe endpoint and limitations.
* Tests cover endpoint and manifest.
* `ai/current-state.md` documents Phase 8F.

## Constraints

* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not reveal visibility.
* Do not add scanner mechanics.
* Do not mutate exploration knowledge.
* Do not mutate resources/fleets/missions.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Keep endpoint read-only.

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
   `feat(sensors): add sensor profile dev endpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
