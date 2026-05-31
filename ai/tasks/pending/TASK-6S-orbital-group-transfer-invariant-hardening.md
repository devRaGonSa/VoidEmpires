# TASK-6S

---

id: TASK-6S
title: Harden orbital group invariants while transfers are active
status: pending
type: hardening
team: backend
supporting_teams:

* application
* infrastructure
* tests
  roadmap_item: "Phase 6S - Orbital group transfer invariant hardening"
  priority: high

---

## Goal

Harden backend invariants so an orbital group that is currently involved in an active transfer cannot be modified through incompatible operations.

This task must protect transfer consistency after split, merge, estimate, cost charging, cancellation, and completion foundations.

## Context

The project now supports:

* creating orbital transfers
* charging resources for transfers
* completing transfers
* splitting orbital groups
* merging orbital groups
* estimating travel and affordability
* cancelling transfers from TASK-6R

Before more gameplay systems are added, active-transfer invariants must be explicit and covered by tests.

For this phase, active transfer means any transfer that has not completed and has not been cancelled/inactivated according to current conventions.

## Implementation steps

1. Inspect current services for:

   * orbital transfer creation
   * orbital transfer completion
   * orbital transfer cancellation from TASK-6R
   * orbital group split
   * orbital group merge
   * orbital travel estimate preview
2. Identify the existing way to determine whether an orbital group is currently in active transfer.
3. Add or reuse a small query/helper inside infrastructure to check active transfer participation if needed.
4. Ensure these operations reject groups in active transfer where appropriate:

   * split source group
   * merge source group
   * merge target group
   * creating a second transfer for the same group
   * travel estimate preview if current conventions require a stationary group for preview
5. Ensure completion and cancellation are still allowed for their own active transfer.
6. Add tests:

   * cannot split a group with an active transfer
   * cannot merge a source group with an active transfer
   * cannot merge into a target group with an active transfer
   * cannot create a second transfer for a group with an active transfer
   * completion still works for an active transfer
   * cancellation still works for an active transfer
   * preview behavior is explicit: either rejected for active transfer or documented as allowed, with tests matching the chosen convention
7. Update endpoint tests if dev endpoints expose affected operations.
8. Update `ai/current-state.md` to document Phase 6S and the active-transfer invariant.

## Files to read first

* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* src/VoidEmpires.Web/DevOrbitalTransferCreationEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalTransferCompletionEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalTransferCancelEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalGroupSplitEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalGroupMergeEndpoints.cs
* tests/VoidEmpires.Tests/*OrbitalTransfer*.cs
* tests/VoidEmpires.Tests/*OrbitalGroupSplit*.cs
* tests/VoidEmpires.Tests/*OrbitalGroupMerge*.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on existing repository conventions:

* src/VoidEmpires.Infrastructure/Fleets/OrbitalTransferCreationService.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalGroupSplitService.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalGroupMergeService.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs
* tests/VoidEmpires.Tests/OrbitalTransferCreationServiceTests.cs
* tests/VoidEmpires.Tests/OrbitalTransferCancelServiceTests.cs
* tests/VoidEmpires.Tests/OrbitalTransferCompletionServiceTests.cs
* tests/VoidEmpires.Tests/OrbitalGroupSplitServiceTests.cs
* tests/VoidEmpires.Tests/OrbitalGroupMergeServiceTests.cs
* tests/VoidEmpires.Tests/OrbitalTravelEstimateServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalTransferEndpointTests.cs
* ai/current-state.md

If a shared helper is useful, add it near existing infrastructure fleet services and keep it small.

## Acceptance criteria

* Active-transfer group invariants are explicit.
* Split rejects active-transfer groups.
* Merge rejects active-transfer source and target groups.
* Transfer creation rejects a second active transfer for the same group.
* Completion still works for its active transfer.
* Cancellation still works for its active transfer.
* Preview behavior for active-transfer groups is explicit and tested.
* Tests cover service behavior and affected endpoint behavior.
* `ai/current-state.md` documents Phase 6S.

## Constraints

* Do not add combat.
* Do not add interception.
* Do not add route graph logic.
* Do not add fuel inventory.
* Do not add alliances, espionage, or UI.
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
   `test(fleets): harden active transfer invariants`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
