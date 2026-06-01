# TASK-8L

---

id: TASK-8L
title: Add interception opportunity read model foundation
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8L - Interception opportunity read model foundation"
priority: high

---

## Goal

Add a lightweight, deterministic, read-only interception opportunity read model.

This phase must not add real interception execution, combat, damage, battle resolution, fleet retasking, persisted interception state, diplomacy, espionage, route graph/pathfinding, production endpoints, or final UI.

The goal is to expose safe metadata about potential interception opportunities derived from existing orbital transfers, fleet state, sensor profiles, and detection coverage.

## Context

The project now has:

* orbital transfers
* fleet operational overview
* sensor profiles
* detection coverage
* strategic map/fleet UI metadata

Future gameplay will need interception, but before that we need a conservative readiness model.

The read model should answer:

* Which active transfers are theoretically visible/detectable to the requesting civilization?
* Is there a friendly fleet context that could potentially intercept?
* What blocks interception readiness?
* What limitations apply?

This is only UI/dev readiness metadata. It is not authorization and it must not execute any command.

## Implementation steps

1. Inspect:

   * orbital transfer domain and lookup services
   * fleet operational overview
   * detection coverage service
   * sensor profile service
   * strategic map transfer overlays
2. Add application contracts, for example:

   * `InterceptionOpportunityStatus`
   * `InterceptionOpportunityBlockReason`
   * `InterceptionOpportunityDto`
   * `GetInterceptionOpportunitiesRequest`
   * `GetInterceptionOpportunitiesResult`
   * `IInterceptionOpportunityService`
3. Implement an infrastructure service:

   * scopes by requesting civilization
   * reads active transfers
   * derives opportunities using conservative metadata only
   * includes transfer id, orbital group id, origin/destination ids, status, estimated timing, and detection/readiness notes
   * excludes own transfers from hostile interception opportunities unless represented as non-hostile/self-observed metadata
   * does not reveal unknown/hidden details beyond current detection/visibility rules
   * does not mutate anything
4. Suggested conservative rules:

   * own active transfers can appear as `ObservedOwnTransfer`, not interceptable.
   * foreign active transfers should only appear if current detection/visibility metadata makes the relevant system/context known.
   * without a friendly fleet context, block with `NoFriendlyInterceptorContext`.
   * without detection context, block with `NotDetected`.
   * actual interception execution remains unsupported.
5. Add tests:

   * own active transfer appears as self-observed/non-interceptable metadata
   * foreign transfer without detection/visibility is not exposed or is blocked without details
   * detected/known foreign transfer produces an opportunity metadata item
   * no friendly fleet context blocks opportunity
   * service is read-only/no mutation
6. Update `ai/current-state.md` to document Phase 8L.

## Files to read first

* src/VoidEmpires.Domain/Fleets/OrbitalTransfer.cs
* src/VoidEmpires.Domain/Fleets/OrbitalGroup.cs
* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* src/VoidEmpires.Application/StrategicMap/*Detection*.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DetectionCoverageService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* tests/VoidEmpires.Tests/*OrbitalTransfer*.cs
* tests/VoidEmpires.Tests/*Detection*.cs
* tests/VoidEmpires.Tests/*Fleet*.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on conventions:

* src/VoidEmpires.Application/StrategicMap/InterceptionOpportunityStatus.cs
* src/VoidEmpires.Application/StrategicMap/InterceptionOpportunityBlockReason.cs
* src/VoidEmpires.Application/StrategicMap/GetInterceptionOpportunitiesRequest.cs
* src/VoidEmpires.Application/StrategicMap/GetInterceptionOpportunitiesResult.cs
* src/VoidEmpires.Application/StrategicMap/IInterceptionOpportunityService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/InterceptionOpportunityService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* tests/VoidEmpires.Tests/InterceptionOpportunityServiceTests.cs
* ai/current-state.md

If repository conventions suggest another namespace, follow current style and keep contracts lightweight.

## Acceptance criteria

* Read-only interception opportunity service exists.
* Interception opportunities are scoped by civilization.
* Own transfers are not represented as hostile/interceptable.
* Foreign transfer metadata is conservative and does not leak hidden data.
* Detection/visibility context is used only as metadata, not as command execution.
* No persisted interception state is added.
* No combat/interception command is executed.
* Tests cover service behavior and read-only guarantees.
* `ai/current-state.md` documents Phase 8L.

## Constraints

* Do not add interception execution.
* Do not add combat, damage, battle result, or interception resolution.
* Do not persist interception state.
* Do not reveal visibility through interception.
* Do not mutate exploration knowledge, resources, fleets, transfers, or missions.
* Do not add espionage or diplomacy.
* Do not add route graph/pathfinding.
* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not add migrations.
* Keep model deterministic and conservative.

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
   `feat(interception): add opportunity read model`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

