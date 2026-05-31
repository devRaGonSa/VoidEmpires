# TASK-6Q

---

id: TASK-6Q
title: Charge orbital transfer costs when creating transfers
status: pending
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* web
* tests
  roadmap_item: "Phase 6Q - Charge orbital transfer costs on create"
  priority: high

---

## Goal

Update real orbital transfer creation so it charges the estimated travel costs when a transfer is successfully created.

This is the first phase that turns orbital travel costs from read-only estimates into real economy behavior.

## Context

Phase 6K added domain travel cost estimates.
Phase 6L added read-only travel estimate preview.
Phase 6O added resource affordability/spend foundation.
Phase 6P added read-only affordability preview.

Now transfer creation should:

* estimate the cost
* validate affordability
* spend the required resources atomically
* create the transfer only if the spend succeeds
* leave the system unchanged if the spend fails

## Implementation steps

1. Read the current orbital transfer creation service and tests.
2. Read the Phase 6O resource spend service.
3. Read the Phase 6K/6L orbital travel estimate service.
4. Update orbital transfer creation flow so that:

   * it estimates travel cost for the requested transfer
   * it checks/spends the required resources before or within the same persistence transaction as transfer creation
   * insufficient resources reject transfer creation
   * when rejected, no transfer is created and the orbital group is not reserved
   * when successful, resource balances decrease and transfer creation continues as before
5. Ensure atomic behavior:

   * no partial spend without transfer
   * no transfer without spend
   * no group reservation when spend fails
6. Update dev transfer creation endpoint response if needed to include cost/insufficient-resource errors.
7. Add tests:

   * transfer creation succeeds and charges resources
   * transfer creation fails when resources are insufficient
   * failed transfer does not create `OrbitalTransfer`
   * failed transfer does not reserve or mutate `OrbitalGroup`
   * failed transfer does not partially spend resources
   * successful transfer still behaves like before for timing/status/origin/destination
   * endpoint returns the expected error for insufficient resources
8. Update `ai/current-state.md` to document Phase 6Q after implementation.

## Files to read first

* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* src/VoidEmpires.Web/DevOrbitalTransferCreationEndpoints.cs
* tests/VoidEmpires.Tests/*OrbitalTransfer*.cs
* tests/VoidEmpires.Tests/*OrbitalTravelEstimate*.cs
* src/VoidEmpires.Application/Economy/
* src/VoidEmpires.Infrastructure/Economy/
* tests/VoidEmpires.Tests/ResourceSpendServiceTests.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on existing repository conventions:

* src/VoidEmpires.Infrastructure/Fleets/OrbitalTransferCreationService.cs
* src/VoidEmpires.Web/DevOrbitalTransferCreationEndpoints.cs
* tests/VoidEmpires.Tests/OrbitalTransferCreationServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalTransferEndpointTests.cs
* ai/current-state.md

Additional application contract files may be modified if existing result/error contracts need to expose insufficient resource details.

## Acceptance criteria

* Real orbital transfer creation charges estimated travel costs.
* Transfer creation rejects insufficient resources.
* Failed transfer creation does not create transfer records.
* Failed transfer creation does not reserve or mutate the orbital group.
* Failed transfer creation does not partially spend resources.
* Successful transfer creation still works as before, plus resource deduction.
* Tests cover success, insufficient resources, and atomic failure behavior.
* `ai/current-state.md` documents Phase 6Q.

## Constraints

* Do not change the estimator constants unless needed to fix a direct issue.
* Do not add route graph logic.
* Do not add fuel inventory.
* Do not add combat, interception, alliances, espionage, or UI.
* Do not add migrations unless absolutely required; if a migration appears necessary, stop and create a follow-up task.
* Keep the behavior deterministic and testable.

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
   `feat(fleets): charge orbital transfer costs`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
