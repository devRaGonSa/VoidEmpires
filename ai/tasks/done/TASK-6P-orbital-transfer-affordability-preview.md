# TASK-6P

---

id: TASK-6P
title: Add orbital transfer affordability preview
status: pending
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* web
* tests
  roadmap_item: "Phase 6P - Orbital transfer affordability preview"
  priority: high

---

## Goal

Extend the existing orbital travel estimate preview so it can also report whether the estimated travel cost is affordable.

This task must remain read-only. It must not spend resources and must not modify transfer creation behavior.

## Context

Phase 6L added a read-only orbital travel estimate preview.
Phase 6O adds a reusable resource affordability/spend foundation.

This task connects the estimate preview with resource affordability checking, but only as a read-only preview.

The endpoint should help validate:

* estimated distance
* estimated duration
* estimated costs
* whether the civilization/planet can afford those costs
* which resources are insufficient

## Implementation steps

1. Read the Phase 6L estimate service and endpoint.
2. Read the Phase 6O resource spend/affordability service.
3. Extend the orbital travel estimate result contracts to include affordability data, for example:

   * `CanAfford`
   * `AffordableCosts`
   * `InsufficientResources`
   * or equivalent naming consistent with repository style
4. Update the infrastructure estimate service to call the affordability checker in read-only mode.
5. Ensure no resource balances are mutated.
6. Update the dev endpoint response to include affordability data.
7. Add tests:

   * estimate preview returns `CanAfford = true` when resources are sufficient
   * estimate preview returns `CanAfford = false` when one or more resources are insufficient
   * insufficient resource details identify ResourceType and required/available amounts if current conventions allow
   * endpoint returns affordability data
   * no resource mutation occurs after preview
8. Update `ai/current-state.md` to document Phase 6P after implementation.

## Files to read first

* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs
* src/VoidEmpires.Web/DevOrbitalTravelEstimateEndpoints.cs
* tests/VoidEmpires.Tests/OrbitalTravelEstimateServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalTravelEstimateEndpointTests.cs
* src/VoidEmpires.Application/Economy/
* src/VoidEmpires.Infrastructure/Economy/
* tests/VoidEmpires.Tests/ResourceSpendServiceTests.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on existing repository conventions:

* src/VoidEmpires.Application/Fleets/EstimateOrbitalTravelRequest.cs
* src/VoidEmpires.Application/Fleets/EstimateOrbitalTravelResult.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs
* src/VoidEmpires.Web/DevOrbitalTravelEstimateEndpoints.cs
* tests/VoidEmpires.Tests/OrbitalTravelEstimateServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalTravelEstimateEndpointTests.cs
* ai/current-state.md

If existing conventions require different filenames, follow existing repository naming patterns.

## Acceptance criteria

* Orbital travel estimate preview includes affordability data.
* Preview remains read-only.
* No resource balances are changed by preview.
* Insufficient resources are reported clearly.
* Endpoint response includes affordability data.
* Tests cover sufficient and insufficient resource cases.
* `ai/current-state.md` documents Phase 6P.

## Constraints

* Do not charge resources.
* Do not create transfers.
* Do not modify transfer creation behavior.
* Do not reserve orbital groups.
* Do not add migrations.
* Do not add final UI.
* Keep behavior read-only.

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
   `feat(fleets): add orbital transfer affordability preview`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
