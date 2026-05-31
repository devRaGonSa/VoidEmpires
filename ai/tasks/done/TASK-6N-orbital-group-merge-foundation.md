# TASK-6N

---

id: TASK-6N
title: Add orbital group merge foundation
status: pending
type: feature
team: backend
supporting_teams:

* domain
* application
* infrastructure
* web
* tests
  roadmap_item: "Phase 6N - Orbital group merge foundation"
  priority: high

---

## Goal

Add a safe foundation for merging two compatible `OrbitalGroup` records into one group.

This must support future fleet manipulation while staying backend-only and avoiding combat, route graph, fuel inventory, UI, or final gameplay complexity.

The merge operation must be explicit, validated, persistent, and covered by tests.

## Context

TASK-6M adds orbital group split. This task adds the complementary merge operation.

A merge should combine two compatible orbital groups owned by the same civilization and located at the same current planet.

This task must not create transfers, reserve groups, charge resources, or introduce combat behavior.

## Implementation steps

1. Inspect current `OrbitalGroup` model and the split service from TASK-6M.
2. Define compatibility rules for merge:

   * civilization id is required
   * target group id is required
   * source group id is required
   * target and source must be different groups
   * both groups must exist
   * both groups must belong to the civilization
   * both groups must be at the same current planet
   * both groups must have compatible asset type/composition according to current domain model
   * both groups must be available/stationed, not currently in transfer/reserved, if current status model supports that
3. Add application contracts, for example:

   * `MergeOrbitalGroupsRequest`
   * `MergeOrbitalGroupsResult`
   * `IOrbitalGroupMergeService`
4. Add EF-backed infrastructure implementation:

   * increase target group quantity/composition
   * remove source group or mark it empty/inactive according to current repository conventions
   * avoid leaving invalid zero-quantity active groups
5. Register the service in DI.
6. Add a Development-only endpoint, suggested route:

   * `POST /api/dev/fleets/orbital-groups/merge`
7. Endpoint should follow existing dev endpoint conventions:

   * gated by the current dev endpoint switch/environment mechanism
   * `503` when persistence is not configured if matching repository convention
   * `400` for invalid payload
   * `200` for successful merge
   * `404`/`409` for rejected service cases according to existing conventions
8. Add tests:

   * service rejects missing target group
   * service rejects missing source group
   * service rejects same group ids
   * service rejects civilization mismatch
   * service rejects different current planets
   * service rejects incompatible asset type/composition
   * service rejects unavailable/in-transfer groups if status supports it
   * service success increases target quantity/composition
   * service success removes or deactivates source group according to convention
   * endpoint gated outside Development/dev-enabled mode
   * endpoint invalid payload
   * endpoint success
9. Update `ai/current-state.md` to document Phase 6N after implementation.

## Files to read first

* src/VoidEmpires.Domain/Fleets/
* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* src/VoidEmpires.Infrastructure/VoidEmpiresDbContext.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* src/VoidEmpires.Web/DevOrbitalGroup*.cs
* tests/VoidEmpires.Tests/*OrbitalGroup*.cs
* tests/VoidEmpires.Tests/*OrbitalTransfer*.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on existing repository conventions:

* src/VoidEmpires.Application/Fleets/MergeOrbitalGroupsRequest.cs
* src/VoidEmpires.Application/Fleets/IOrbitalGroupMergeService.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalGroupMergeService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevOrbitalGroupMergeEndpoints.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* tests/VoidEmpires.Tests/OrbitalGroupMergeServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalGroupMergeEndpointTests.cs
* ai/current-state.md

If existing conventions require different filenames, follow existing repository naming patterns and keep the change minimal.

## Acceptance criteria

* Orbital group merge is implemented as a persistent backend operation.
* Target group quantity/composition increases.
* Source group is removed or deactivated according to existing repository conventions.
* Invalid merge requests are rejected.
* Civilization ownership is enforced.
* Same-current-planet requirement is enforced.
* Compatibility by asset type/composition is enforced.
* In-transfer/unavailable groups cannot be merged if current status model supports this.
* Dev endpoint exists and is gated consistently with existing dev endpoints.
* Tests cover service and endpoint behavior.
* `ai/current-state.md` documents Phase 6N.

## Constraints

* Do not modify split behavior except to fix direct integration issues with merge.
* Do not add combat.
* Do not add route graph logic.
* Do not add fuel inventory.
* Do not charge resources.
* Do not create or complete orbital transfers.
* Do not add migrations unless the existing model absolutely requires a schema change; if a migration appears necessary, stop and create a follow-up task explaining why.
* Do not add final UI.
* Keep implementation incremental and testable.

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
   `feat(fleets): add orbital group merge foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
