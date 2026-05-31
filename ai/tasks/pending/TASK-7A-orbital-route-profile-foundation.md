# TASK-7A

---

id: TASK-7A
title: Add orbital route profile foundation
status: pending
type: feature
team: backend
supporting_teams:

* domain
* application
* infrastructure
* web
* tests
* docs
  roadmap_item: "Phase 7A - Orbital route profile foundation"
  priority: high

---

## Goal

Add a lightweight, read-only orbital route profile foundation that classifies a potential orbital trip before transfer creation.

This phase must not add a full route graph, pathfinding, fuel inventory, combat, interception, alliances, espionage, or final UI.

The goal is to give future movement systems a stable contract for route difficulty and route metadata while keeping current transfer creation behavior safe and deterministic.

## Context

The project already has orbital travel estimates based on abstract distance, duration, resource costs, and affordability.

The next step is to enrich movement with a route profile that can later support deeper gameplay. For now, route profile must be derived and read-only.

Examples of route profile data:

* route class, such as LocalOrbit, InnerSystem, OuterSystem, LongRange
* distance band
* risk band placeholder
* fuel multiplier placeholder
* travel complexity notes
* whether the route is currently supported by the backend

This is not pathfinding. This is not a route graph. This is not combat/interception.

## Implementation steps

1. Inspect existing orbital travel estimator, estimate service, and tests.
2. Add domain/application contracts for route profile, for example:

   * `OrbitalRouteProfile`
   * `OrbitalRouteClass`
   * `OrbitalRouteRiskBand`
   * `OrbitalRouteProfileDto`
   * `IOrbitalRouteProfileService`
3. Implement route classification using existing abstract distance units:

   * short/local distances classify as a simpler route
   * larger distances classify as more complex route bands
   * choose deterministic thresholds and document them in code/tests
4. Keep the route profile read-only and deterministic.
5. Expose route profile through the existing orbital travel estimate result if it fits cleanly, or add a dedicated read-only service if that is cleaner.
6. Add a Development-only endpoint only if useful and repository-consistent, suggested route:

   * `POST /api/dev/fleets/orbital-route/profile`
7. Endpoint should:

   * follow current dev gating conventions
   * return `404` outside Development/dev-enabled mode
   * return `503` when persistence is not configured if persistence is needed
   * return `400` for invalid request
   * return `200` for success
8. Add focused tests:

   * route class is deterministic for known distances
   * route class changes across threshold boundaries
   * invalid same-planet route is rejected if using group/current/destination input
   * service/endpoint returns supported route profile
   * endpoint is gated outside Development/dev-enabled mode if endpoint is added
9. Update `docs/dev/fleet-api-contracts.md` if an endpoint or estimate response shape changes.
10. Update `ai/current-state.md` to document Phase 7A.

## Files to read first

* src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimator.cs
* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* src/VoidEmpires.Web/DevOrbitalTravelEstimateEndpoints.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* tests/VoidEmpires.Tests/*OrbitalTravelEstimate*.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on repository conventions:

* src/VoidEmpires.Domain/Fleets/OrbitalRouteClass.cs
* src/VoidEmpires.Domain/Fleets/OrbitalRouteRiskBand.cs
* src/VoidEmpires.Domain/Fleets/OrbitalRouteProfile.cs
* src/VoidEmpires.Application/Fleets/OrbitalRouteProfileDto.cs
* src/VoidEmpires.Application/Fleets/IOrbitalRouteProfileService.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalRouteProfileService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevOrbitalRouteProfileEndpoints.cs
* src/VoidEmpires.Web/Program.cs or DevEndpointMappings.cs, following existing endpoint conventions
* tests/VoidEmpires.Tests/OrbitalRouteProfileServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalRouteProfileEndpointTests.cs
* docs/dev/fleet-api-contracts.md
* ai/current-state.md

If existing conventions require different filenames, follow repository naming patterns.

## Acceptance criteria

* Route profile is read-only and deterministic.
* Route profile classifies trips by distance band.
* No full route graph or pathfinding is introduced.
* No transfer creation behavior changes are introduced in this task.
* Route profile is covered by tests.
* If a dev endpoint is added, endpoint gating and status behavior match existing conventions.
* Documentation is updated if public/dev response contracts change.
* `ai/current-state.md` documents Phase 7A.

## Constraints

* Do not add full route graph logic.
* Do not add pathfinding.
* Do not add fuel inventory.
* Do not charge additional resources.
* Do not modify real transfer creation behavior.
* Do not add combat, interception, alliances, espionage, or final UI.
* Do not add migrations unless absolutely required; if a migration appears necessary, stop and create a follow-up task.
* Keep behavior deterministic and testable.

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
   `feat(fleets): add orbital route profile foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
