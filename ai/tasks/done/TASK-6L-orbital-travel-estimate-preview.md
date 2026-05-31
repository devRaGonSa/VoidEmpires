# TASK-6L

---
id: TASK-6L
title: Add read-only orbital travel estimate preview service and dev endpoint
status: done
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - web
  - tests
roadmap_item: "Phase 6L - Orbital travel estimate preview"
priority: high
---

## Goal

Expose a read-only orbital travel estimate preview through the application/infrastructure/web layers.

The preview must calculate distance, estimated duration, and estimated resource costs for an orbital group traveling from its current planet to a requested destination planet.

This phase must not create an OrbitalTransfer, reserve an OrbitalGroup, charge resources, mutate stockpiles, or persist any travel estimate.

## Context

Phase 6K already added the pure domain estimator:

* `OrbitalTravelEstimate`
* `OrbitalTravelResourceCost`
* `OrbitalTravelEstimator.Estimate(...)`

Now Phase 6L should add the read path around it:

* application request/result contracts
* application service interface
* infrastructure implementation that reads persisted orbital group and destination data
* Development-only HTTP endpoint for preview validation
* tests for service and endpoint behavior

This is a preview only. It is not the real transfer creation flow.

## Implementation steps

1. Read the current Phase 6K domain estimator and tests.
2. Inspect existing fleet/orbital group services and dev endpoints to follow naming and response conventions.
3. Add application contracts for estimating orbital travel preview, for example:

   * `EstimateOrbitalTravelRequest`
   * `EstimateOrbitalTravelResult`
   * `OrbitalTravelCostComponentDto`
   * `IOrbitalTravelEstimateService`
4. Add an infrastructure implementation that:

   * receives `CivilizationId`, `OrbitalGroupId`, and `DestinationPlanetId`
   * loads the orbital group
   * verifies the orbital group belongs to the civilization
   * verifies the orbital group has a current planet
   * verifies the destination planet exists
   * rejects same current/destination planet using the existing estimator validation behavior or explicit validation
   * derives the relevant `SpaceAssetType` from the orbital group or its grouped assets according to current domain structure
   * calls `OrbitalTravelEstimator.Estimate(...)`
   * returns a read-only result with distance, costs, and errors
5. Register the service in the existing persistence/application dependency injection location following repository conventions.
6. Add a Development-only endpoint, suggested route:

   * `POST /api/dev/fleets/orbital-travel/estimate`
7. The endpoint should:

   * be available only through the same dev endpoint gating pattern used by existing dev endpoints
   * return `404` outside Development/dev-enabled mode if that is the current convention
   * return `503` when persistence is not configured if matching existing endpoint behavior
   * return `400` for invalid request payload
   * return `200` for successful estimate
   * return `404` or `409` for rejected domain/service cases following existing repository convention
8. Add focused tests:

   * service success returns duration, distance, and costs
   * service rejects missing orbital group
   * service rejects civilization mismatch
   * service rejects missing destination planet
   * service rejects same destination as current planet
   * endpoint is not exposed outside Development/dev-enabled mode
   * endpoint returns 503 without persistence if that is the current convention
   * endpoint returns 400 for invalid payload
   * endpoint returns 200 for valid preview
9. Update `ai/current-state.md` after implementation to document Phase 6L and the fact that estimates are read-only preview only.

## Files to read first

* src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimator.cs
* src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimate.cs
* src/VoidEmpires.Domain/Fleets/OrbitalTravelResourceCost.cs
* tests/VoidEmpires.Tests/OrbitalTravelEstimatorTests.cs
* existing application fleet/orbital transfer contracts under src/VoidEmpires.Application/Fleets/
* existing infrastructure fleet/orbital transfer services under src/VoidEmpires.Infrastructure/Fleets/
* existing dev fleet/orbital transfer endpoints under src/VoidEmpires.Web/
* existing endpoint tests under tests/VoidEmpires.Tests/
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on existing repository conventions:

* src/VoidEmpires.Application/Fleets/EstimateOrbitalTravelRequest.cs
* src/VoidEmpires.Application/Fleets/IOrbitalTravelEstimateService.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevOrbitalTravelEstimateEndpoints.cs
* src/VoidEmpires.Web/Program.cs
* tests/VoidEmpires.Tests/OrbitalTravelEstimateServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalTravelEstimateEndpointTests.cs
* ai/current-state.md

If existing conventions require different filenames, follow existing repository naming patterns and keep the change minimal.

## Acceptance criteria

* A read-only application service exists for orbital travel estimate previews.
* The service uses the Phase 6K domain estimator.
* The service reads current orbital group state and destination planet state from persistence.
* The service validates civilization ownership/access.
* The service returns estimated distance, duration, and resource costs.
* The dev endpoint exposes preview only.
* No resources are charged.
* No stockpile is mutated.
* No `OrbitalTransfer` is created.
* No orbital group status is changed.
* No migrations are added.
* No production gameplay endpoint is added.
* Tests cover service behavior and endpoint behavior.
* `ai/current-state.md` documents Phase 6L as read-only preview.

## Constraints

* Do not modify actual orbital transfer creation behavior.
* Do not reserve orbital groups.
* Do not charge resources.
* Do not mutate database state except normal test setup.
* Do not add migrations.
* Do not add final UI.
* Do not add sandbox rendering changes.
* Do not add route graph, fuel inventory, combat, interception, alliances, espionage, or movement simulation.
* Keep implementation incremental and testable.
* Prefer modifying fewer than 5 files where practical, but endpoint + service + tests may require more.
* If the change grows too large, split into a follow-up task rather than exceeding the task budget silently.

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
   `feat(fleets): add orbital travel estimate preview`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

