# TASK-6M

---

id: TASK-6M
title: Add orbital group split foundation
status: pending
type: feature
team: backend
supporting_teams:

* domain
* application
* infrastructure
* web
* tests
  roadmap_item: "Phase 6M - Orbital group split foundation"
  priority: high

---

## Goal

Add a safe foundation for splitting part of an existing `OrbitalGroup` into a new `OrbitalGroup`.

This must allow future fleet manipulation without introducing combat, route graphs, fuel inventory, UI, or final gameplay complexity.

The split operation must be explicit, validated, persistent, and covered by tests.

## Context

VoidEmpires already has orbital groups, ownership, origin/current planet separation, transfer foundations, and travel estimate preview.

Before combat or advanced fleet movement, the player needs basic fleet composition manipulation:

* keep part of a group in place
* create a smaller group from an existing group
* preserve civilization ownership
* preserve origin/current planet rules
* avoid splitting groups that are not available for local manipulation

This task introduces split only. Merge is handled by TASK-6N.

## Implementation steps

1. Inspect current `OrbitalGroup` domain model, persistence configuration, services, and tests.
2. Identify how an orbital group stores composition/quantity/type today.
3. Add domain-level validation/rules needed for split:

   * source group id is required
   * civilization id is required
   * split quantity must be positive
   * split quantity must be lower than source quantity
   * source group must belong to the civilization
   * source group must be available/stationed, not currently in transfer/reserved, if current status model supports that
   * new group keeps the same `CivilizationId`
   * new group keeps the same `OriginPlanetId`
   * new group keeps the same `CurrentPlanetId`
   * source group quantity is decreased
4. Add application contracts, for example:

   * `SplitOrbitalGroupRequest`
   * `SplitOrbitalGroupResult`
   * `IOrbitalGroupSplitService`
5. Add EF-backed infrastructure service implementation.
6. Register the service in DI following existing conventions.
7. Add a Development-only endpoint, suggested route:

   * `POST /api/dev/fleets/orbital-groups/split`
8. Endpoint should follow existing dev endpoint conventions:

   * gated by the current dev endpoint switch/environment mechanism
   * `503` when persistence is not configured if matching repository convention
   * `400` for invalid payload
   * `200` or `201` for successful split according to nearby endpoint convention
   * `404`/`409` for rejected service cases according to existing conventions
9. Add tests:

   * domain/service rejects invalid quantity
   * service rejects missing source group
   * service rejects civilization mismatch
   * service rejects splitting full quantity
   * service rejects unavailable/in-transfer group if status supports it
   * service success decreases source quantity
   * service success creates a new group with same civilization/origin/current planet
   * endpoint gated outside Development/dev-enabled mode
   * endpoint invalid payload
   * endpoint success
10. Update `ai/current-state.md` to document Phase 6M after implementation.

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

* src/VoidEmpires.Application/Fleets/SplitOrbitalGroupRequest.cs
* src/VoidEmpires.Application/Fleets/IOrbitalGroupSplitService.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalGroupSplitService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevOrbitalGroupSplitEndpoints.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* tests/VoidEmpires.Tests/OrbitalGroupSplitServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalGroupSplitEndpointTests.cs
* ai/current-state.md

If existing conventions require different filenames, follow existing repository naming patterns and keep the change minimal.

## Acceptance criteria

* Orbital group split is implemented as a persistent backend operation.
* Source group quantity decreases.
* New group is created with correct civilization, origin planet, current planet, asset type/composition, and status.
* Invalid split requests are rejected.
* Civilization ownership is enforced.
* In-transfer/unavailable groups cannot be split if current status model supports this.
* Dev endpoint exists and is gated consistently with existing dev endpoints.
* Tests cover service and endpoint behavior.
* `ai/current-state.md` documents Phase 6M.

## Constraints

* Do not implement merge in this task.
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
   `feat(fleets): add orbital group split foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
