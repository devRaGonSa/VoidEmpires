# TASK-6T

---

id: TASK-6T
title: Add fleet operational overview read model
status: done
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* web
* tests
  roadmap_item: "Phase 6T - Fleet operational overview read model"
  priority: high

---

## Goal

Add a read-only fleet operational overview that returns the current orbital fleet state for a civilization.

This read model should help future UI and dev validation by consolidating:

* orbital groups
* current planet/origin planet
* group quantity/composition/type
* whether a group is currently in active transfer
* active transfer summary if applicable
* transfer timing/status information
* basic ability flags for available commands, such as can split, can merge, can transfer, can cancel

This phase must be read-only.

## Context

The backend now supports several fleet operations:

* transfer creation
* transfer completion
* transfer cancellation
* split
* merge
* cost charging
* active-transfer invariant hardening

At this point, it is becoming hard to validate fleet state through isolated endpoints only. A consolidated read model will be useful for dev endpoints and later UI.

This task must not add final UI and must not introduce combat or route graph logic.

## Implementation steps

1. Inspect existing orbital group query/read services and transfer lookup services.
2. Reuse existing DTO/query conventions where possible.
3. Add application contracts for fleet overview, for example:

   * `GetFleetOperationalOverviewRequest`
   * `GetFleetOperationalOverviewResult`
   * `FleetOperationalGroupDto`
   * `FleetOperationalTransferDto`
   * `FleetOperationalCommandAvailabilityDto`
   * `IFleetOperationalOverviewService`
4. Add EF-backed infrastructure implementation that:

   * filters by `CivilizationId`
   * returns orbital groups owned by the civilization
   * includes origin/current planet ids where available
   * includes group quantity/type/composition according to existing model
   * identifies whether each group has an active transfer
   * includes active transfer id, destination, departure/arrival timing and status where available
   * derives command availability flags from existing invariants:

     * `CanCreateTransfer`
     * `CanSplit`
     * `CanMerge`
     * `CanCancelTransfer`
   * does not mutate anything
5. Register the service in DI.
6. Add a Development-only endpoint, suggested route:

   * `GET /api/dev/fleets/overview?civilizationId={id}`
     or follow existing dev endpoint request style if the repo prefers POST.
7. Endpoint should follow existing dev endpoint conventions:

   * gated by current dev endpoint switch/environment mechanism
   * `503` when persistence is not configured if matching current convention
   * `400` for invalid request
   * `200` for success
8. Add tests:

   * service returns empty overview for civilization with no groups
   * service returns stationary group with correct command availability
   * service returns active-transfer group with active transfer summary
   * service marks active-transfer group as not splittable/mergeable/transferable
   * service marks active-transfer group as cancellable if cancellation exists
   * service excludes groups from other civilizations
   * endpoint gated outside Development/dev-enabled mode
   * endpoint invalid request
   * endpoint success
9. Update `ai/current-state.md` to document Phase 6T.

## Files to read first

* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* src/VoidEmpires.Web/DevOrbitalTransferLookupEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalTransferCreationEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalTransferCancelEndpoints.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* tests/VoidEmpires.Tests/*OrbitalTransfer*.cs
* tests/VoidEmpires.Tests/*OrbitalGroup*.cs
* tests/VoidEmpires.Tests/*Fleet*.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on existing repository conventions:

* src/VoidEmpires.Application/Fleets/GetFleetOperationalOverviewRequest.cs
* src/VoidEmpires.Application/Fleets/GetFleetOperationalOverviewResult.cs
* src/VoidEmpires.Application/Fleets/IFleetOperationalOverviewService.cs
* src/VoidEmpires.Infrastructure/Fleets/FleetOperationalOverviewService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevFleetOperationalOverviewEndpoints.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* tests/VoidEmpires.Tests/FleetOperationalOverviewServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetOperationalOverviewEndpointTests.cs
* ai/current-state.md

If existing conventions require different filenames, follow repository naming patterns.

## Acceptance criteria

* A read-only fleet operational overview service exists.
* The overview is scoped by civilization.
* Stationary groups are returned with correct state.
* Groups in active transfer are returned with active transfer summary.
* Command availability flags reflect current active-transfer invariants.
* No data is mutated.
* Dev endpoint exists and is gated consistently with existing dev endpoints.
* Tests cover service and endpoint behavior.
* `ai/current-state.md` documents Phase 6T.

## Constraints

* Read-only only.
* Do not create, cancel, complete, split, merge, or charge anything in this task.
* Do not add combat.
* Do not add interception.
* Do not add route graph logic.
* Do not add fuel inventory.
* Do not add alliances, espionage, or final UI.
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
   `feat(fleets): add fleet operational overview`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
