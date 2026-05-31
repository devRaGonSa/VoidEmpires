# TASK-6R

---

id: TASK-6R
title: Add orbital transfer cancellation foundation
status: pending
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* web
* tests
  roadmap_item: "Phase 6R - Orbital transfer cancellation foundation"
  priority: high

---

## Goal

Add a safe backend foundation for cancelling an active orbital transfer before it completes.

This task must release the orbital group from the transfer lifecycle safely, without adding combat, interception, route graph logic, fuel inventory, UI, or final gameplay complexity.

Cancellation must be explicit, validated, persistent, and covered by tests.

## Context

The project now charges resources when creating an orbital transfer.

Before adding more advanced movement/combat systems, the transfer lifecycle needs a safe cancellation path. This gives us a controlled way to abort invalid or undesired transfers during development and future gameplay.

Cancellation policy for this phase:

* Cancel only transfers that have not completed.
* Do not refund resources in this phase.
* Do not create a return trip.
* Do not simulate travel progress.
* Release the orbital group according to current domain conventions.
* Keep the group at its current/origin planet according to the current persisted transfer model. If the current model does not support mid-flight location, use the safest existing convention and document it in `ai/current-state.md`.
* If the current transfer model does not have a cancellable/cancelled status and adding one would require a migration, stop and create a follow-up task explaining the required schema change instead of forcing a large migration into this phase.

## Implementation steps

1. Inspect current `OrbitalTransfer` domain model, statuses, creation service, completion worker/service, and tests.
2. Determine whether the existing model supports a cancelled state without schema changes.
3. Add application contracts, for example:

   * `CancelOrbitalTransferRequest`
   * `CancelOrbitalTransferResult`
   * `IOrbitalTransferCancelService`
4. Add EF-backed infrastructure implementation that:

   * receives `CivilizationId` and `OrbitalTransferId`
   * loads the transfer and related orbital group
   * verifies the transfer exists
   * verifies the transfer belongs to the civilization
   * rejects completed transfers
   * rejects already cancelled transfers if cancelled state exists
   * cancels the transfer or marks it inactive according to existing conventions
   * releases the orbital group from transfer/reserved state according to existing conventions
   * does not refund resources
5. Register the service in DI.
6. Add a Development-only endpoint, suggested route:

   * `POST /api/dev/fleets/orbital-transfers/cancel`
7. Endpoint should follow existing dev endpoint conventions:

   * gated by the current dev endpoint switch/environment mechanism
   * `503` when persistence is not configured if matching repository convention
   * `400` for invalid payload
   * `200` for successful cancellation
   * `404`/`409` for rejected service cases according to current conventions
8. Add tests:

   * service rejects missing transfer
   * service rejects civilization mismatch
   * service rejects completed transfer
   * service rejects already cancelled transfer if applicable
   * service success marks transfer cancelled/inactive according to convention
   * service success releases orbital group according to convention
   * service success does not refund charged resources
   * endpoint gated outside Development/dev-enabled mode
   * endpoint invalid payload
   * endpoint success
9. Update `ai/current-state.md` to document Phase 6R and the explicit no-refund cancellation policy.

## Files to read first

* src/VoidEmpires.Domain/Fleets/
* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* src/VoidEmpires.Infrastructure/VoidEmpiresDbContext.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevOrbitalTransferCreationEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalTransferCompletionEndpoints.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* tests/VoidEmpires.Tests/*OrbitalTransfer*.cs
* tests/VoidEmpires.Tests/*ResourceSpend*.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on existing repository conventions:

* src/VoidEmpires.Application/Fleets/CancelOrbitalTransferRequest.cs
* src/VoidEmpires.Application/Fleets/CancelOrbitalTransferResult.cs
* src/VoidEmpires.Application/Fleets/IOrbitalTransferCancelService.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalTransferCancelService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevOrbitalTransferCancelEndpoints.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* tests/VoidEmpires.Tests/OrbitalTransferCancelServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalTransferCancelEndpointTests.cs
* ai/current-state.md

If existing conventions require different filenames, follow existing repository naming patterns.

## Acceptance criteria

* Orbital transfer cancellation is implemented as a backend operation or, if blocked by schema limitations, a clear follow-up task is created explaining the blocker.
* Existing completed transfers cannot be cancelled.
* Civilization ownership is enforced.
* Successful cancellation releases the orbital group according to current conventions.
* Cancellation does not refund resources in this phase.
* Dev endpoint exists and is gated consistently with existing dev endpoints.
* Tests cover service and endpoint behavior.
* `ai/current-state.md` documents Phase 6R and the no-refund policy.

## Constraints

* Do not add refunds in this phase.
* Do not add return travel.
* Do not add route graph logic.
* Do not add fuel inventory.
* Do not add combat, interception, alliances, espionage, or UI.
* Do not add migrations unless absolutely required; if a migration appears necessary, stop and create a follow-up task.
* Do not alter cost estimator constants.
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
   `feat(fleets): add orbital transfer cancellation foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
