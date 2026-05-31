# TASK-6O

---

id: TASK-6O
title: Add resource affordability and spend foundation
status: pending
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* tests
  roadmap_item: "Phase 6O - Resource affordability and spend foundation"
  priority: high

---

## Goal

Add a reusable backend foundation for validating and spending resource costs from a civilization or planet economy store.

This foundation will be used by orbital transfer cost charging later, but this task must not modify orbital transfer creation behavior.

## Context

Phase 6K introduced orbital travel estimates with costs by `ResourceType`.
Phase 6L exposed read-only travel estimate preview.
Before real transfer creation can charge travel costs, the backend needs a reusable and tested economy operation for:

* checking if a set of costs is affordable
* spending a set of costs atomically inside the existing EF persistence boundary
* returning clear errors for insufficient resources

This task is economy-focused. It should not create or change orbital transfers.

## Implementation steps

1. Inspect current economy/resource entities, services, and tests.
2. Identify where resource balances are currently stored and how resource changes are persisted.
3. Add application contracts for resource affordability/spend, for example:

   * `ResourceCostDto` or reuse an existing DTO if one exists
   * `ResourceSpendRequest`
   * `ResourceSpendResult`
   * `IResourceSpendService`
4. Add infrastructure implementation using the existing EF persistence model.
5. The service must support:

   * checking all requested costs
   * rejecting negative costs
   * allowing zero-cost entries only if current conventions allow them; otherwise reject them
   * rejecting unknown/missing balances
   * rejecting insufficient resources
   * spending all requested costs atomically when affordable
6. Register the service in DI.
7. Add tests for:

   * successful affordability check
   * successful spend decreases balances
   * insufficient resource rejection
   * missing resource balance rejection
   * invalid negative cost rejection
   * multiple resource costs are handled atomically
   * no partial spend occurs when one resource is insufficient
8. Update `ai/current-state.md` to document Phase 6O after implementation.

## Files to read first

* src/VoidEmpires.Domain/Economy/
* src/VoidEmpires.Application/Economy/
* src/VoidEmpires.Infrastructure/Economy/
* src/VoidEmpires.Infrastructure/VoidEmpiresDbContext.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* tests/VoidEmpires.Tests/*Economy*.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on existing repository conventions:

* src/VoidEmpires.Application/Economy/ResourceSpendRequest.cs
* src/VoidEmpires.Application/Economy/ResourceSpendResult.cs
* src/VoidEmpires.Application/Economy/IResourceSpendService.cs
* src/VoidEmpires.Infrastructure/Economy/ResourceSpendService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* tests/VoidEmpires.Tests/ResourceSpendServiceTests.cs
* ai/current-state.md

If existing conventions require different filenames, follow existing repository naming patterns.

## Acceptance criteria

* A reusable resource affordability/spend service exists.
* It validates affordability for multiple resources.
* It spends resources atomically when affordable.
* It rejects insufficient resources without partial mutation.
* It rejects invalid costs.
* It is covered by focused tests.
* It is registered in DI.
* `ai/current-state.md` documents Phase 6O.

## Constraints

* Do not modify orbital transfer creation behavior in this task.
* Do not add orbital travel-specific behavior in this task.
* Do not add dev endpoints unless an existing economy convention makes that necessary.
* Do not add migrations unless absolutely required; if a migration appears necessary, stop and create a follow-up task.
* Do not add UI.
* Keep the change incremental and testable.

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
   `feat(economy): add resource spend foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
