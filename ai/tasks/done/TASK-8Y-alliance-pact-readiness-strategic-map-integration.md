# TASK-8Y

---

id: TASK-8Y
title: Integrate alliance pact readiness into strategic map metadata
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8Y - Alliance pact readiness strategic map integration"
priority: high

---

## Goal

Integrate read-only alliance pact readiness metadata into strategic map and alliance readiness surfaces.

This phase must not grant shared visibility, permissions, trade, war, defense/intervention behavior, shared sensors/detection, espionage, combat, production endpoints, or final UI.

The goal is UI/dev readiness metadata only.

## Context

Phase 8X adds alliance pact readiness metadata.

Strategic map currently contains:

* diplomacy notes
* diplomatic contacts
* alliance readiness/membership metadata
* visibility derived from ownership/exploration knowledge
* sensors/detection/interception metadata

Now strategic map should be able to expose:

* top-level alliance pact readiness notes
* pact metadata involving alliances where the requesting civilization is a member
* explicit limitations that pact readiness does not affect visibility, command authorization, trade, war, defense, or combat

Alliance pacts should remain separate from gameplay permissions:

* a pact is not shared visibility
* a pact is not an authorization layer
* a pact does not expose ally-owned systems or fleets
* a pact does not trigger defense or war behavior

## Implementation steps

1. Read Phase 8X alliance pact readiness contracts/service.
2. Inspect:

   * `GetStrategicMapResult`
   * `StrategicMapService`
   * `GetAllianceReadinessResult`
   * `AllianceReadinessQueryService`
3. Add strategic map metadata:

   * top-level `AlliancePactNotes`
   * top-level `AlliancePacts` or `AlliancePactReadiness`
4. Use `IAlliancePactReadinessQueryService` or equivalent from `StrategicMapService`.
5. Ensure no visibility changes:

   * do not alter `MapVisibilityService`
   * do not add alliance pact systems to strategic map relevance
   * do not expose ally-owned planets/fleets
   * do not expose ally sensor/detection/interception data
6. Add tests:

   * strategic map returns pact metadata for requesterâ€™s alliance memberships
   * strategic map does not reveal ally/foreign systems through pact metadata
   * alliance pacts remain independent from diplomatic contacts
   * read remains no-mutation
7. Update `docs/dev/strategic-map-api-contract.md`.
8. Update `ai/current-state.md` to document Phase 8Y.

## Files to read first

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Application/StrategicMap/GetAllianceReadinessResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/AllianceReadinessQueryService.cs
* src/VoidEmpires.Application/StrategicMap/GetAlliancePactReadinessResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/AlliancePactReadinessQueryService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/MapVisibilityService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs
* tests/VoidEmpires.Tests/AlliancePactReadinessQueryServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

May also modify:

* src/VoidEmpires.Application/StrategicMap/GetAlliancePactReadinessResult.cs
* tests/VoidEmpires.Tests/AlliancePactReadinessQueryServiceTests.cs

## Acceptance criteria

* Strategic map exposes read-only alliance pact readiness metadata.
* Pact metadata is scoped to alliances where the requesting civilization is a member.
* Pact readiness does not alter visibility.
* Pact readiness does not add strategic map relevance.
* Ally-owned or foreign systems/fleets are not exposed.
* Diplomatic contacts remain independent from pact readiness.
* Tests cover integration behavior.
* Docs are updated.
* `ai/current-state.md` documents Phase 8Y.

## Constraints

* Do not add shared visibility.
* Do not alter `MapVisibilityService`.
* Do not expose ally-owned systems, planets, fleets, transfers, sensors, detection, or interception data.
* Do not add pact permissions.
* Do not add trade behavior.
* Do not add war declarations.
* Do not add defense/intervention behavior.
* Do not add espionage.
* Do not add combat.
* Do not add production endpoints.
* Do not add final UI/frontend code.
* Keep metadata lightweight and read-only.

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
   `feat(map): surface alliance pact readiness metadata`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

