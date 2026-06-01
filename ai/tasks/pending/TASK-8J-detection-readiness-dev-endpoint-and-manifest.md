# TASK-8J

---

id: TASK-8J
title: Add detection readiness dev endpoint and manifest metadata
status: pending
type: feature
team: backend
supporting_teams:
  - web
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8J - Detection readiness dev endpoint and manifest metadata"
priority: high

---

## Goal

Expose detection coverage through a Development-only read endpoint and strategic map action manifest metadata.

This is dev tooling only. It must not add production endpoints or gameplay effects.

## Context

Phase 8H adds a detection coverage service.
Phase 8I integrates detection metadata into strategic map.

Now dev tooling should be able to inspect detection readiness directly.

Suggested endpoint:

* `GET /api/dev/strategic-map/detection-coverage?civilizationId={id}`

Manifest action:

* `detection.coverage.read`

## Implementation steps

1. Read detection coverage contracts/service.
2. Add Development-only endpoint:

   * `GET /api/dev/strategic-map/detection-coverage?civilizationId={id}`
3. Endpoint behavior:

   * disabled dev routes return `404`
   * missing persistence returns `503`
   * missing/empty civilization id returns `400`
   * success returns `200`
4. Add action manifest metadata:

   * `detection.coverage.read`
   * method GET
   * route `/api/dev/strategic-map/detection-coverage`
   * read-only true
   * required field `civilizationId`
   * common status codes
   * notes that coverage is metadata only and does not reveal, detect, intercept, or trigger combat yet
5. Add tests:

   * endpoint gating
   * endpoint persistence missing
   * endpoint bad request
   * endpoint success
   * manifest includes detection coverage action
   * endpoint is read-only/no mutation
6. Update `docs/dev/strategic-map-api-contract.md`.
7. Update `ai/current-state.md` to document Phase 8J.

## Files to read first

* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Web/DevSensorProfileEndpoints.cs
* src/VoidEmpires.Web/DevStrategicMapEndpoints.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* src/VoidEmpires.Application/StrategicMap/GetDetectionCoverageResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DetectionCoverageService.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Web/DevDetectionCoverageEndpoints.cs
* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* tests/VoidEmpires.Tests/DevDetectionCoverageEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

## Acceptance criteria

* Detection coverage dev endpoint exists.
* Endpoint follows current dev gating conventions.
* Endpoint returns read-only detection coverage result.
* Action manifest includes detection coverage read action.
* Docs describe endpoint and limitations.
* Tests cover endpoint and manifest.
* `ai/current-state.md` documents Phase 8J.

## Constraints

* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not reveal visibility.
* Do not add real detection/scanner mechanics.
* Do not persist detection state.
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
   `feat(detection): add detection coverage dev endpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
