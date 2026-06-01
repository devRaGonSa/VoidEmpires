# TASK-8U

---

id: TASK-8U
title: Integrate alliance readiness into strategic map and diplomacy metadata
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8U - Alliance readiness strategic map integration"
priority: high

---

## Goal

Integrate read-only alliance readiness metadata into strategic map and diplomacy/contact metadata.

This phase must not grant shared visibility, shared sensor/detection coverage, shared fleet data, permissions, pacts, trade, war, espionage, combat, production endpoints, or final UI.

The goal is UI/dev readiness metadata only.

## Context

Phase 8T adds minimal alliance and membership readiness.

Strategic map currently contains:

* diplomacy notes
* diplomatic contacts
* visibility derived from ownership/exploration knowledge
* sensors/detection/interception metadata

Now strategic map should be able to expose:

* top-level alliance readiness notes
* requesting civilizationâ€™s own alliance memberships
* explicit limitations that alliance readiness does not affect visibility or authorization

Diplomatic contacts should remain separate from alliance memberships:

* a contact is not an alliance
* an alliance membership is not a contact permission
* neither grants shared visibility in this phase

## Implementation steps

1. Read Phase 8T alliance readiness contracts/service.
2. Inspect:

   * `GetStrategicMapResult`
   * `StrategicMapService`
   * `GetDiplomaticContactsResult`
   * `DiplomaticContactQueryService`
3. Add strategic map metadata:

   * top-level `AllianceNotes`
   * top-level `AllianceMemberships` or `AllianceReadiness`
4. Use `IAllianceReadinessQueryService` or equivalent from `StrategicMapService`.
5. Ensure no visibility changes:

   * do not alter `MapVisibilityService`
   * do not add alliance systems to strategic map relevance
   * do not expose ally-owned planets/fleets
   * do not expose ally sensor/detection/interception data
6. Add tests:

   * strategic map returns own alliance membership metadata
   * strategic map still does not reveal ally/foreign systems through alliance membership
   * diplomatic contacts remain independent from alliance membership
   * read remains no-mutation
7. Update `docs/dev/strategic-map-api-contract.md`.
8. Update `ai/current-state.md` to document Phase 8U.

## Files to read first

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Application/StrategicMap/GetAllianceReadinessResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/AllianceReadinessQueryService.cs
* src/VoidEmpires.Application/StrategicMap/GetDiplomaticContactsResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DiplomaticContactQueryService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/MapVisibilityService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs
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

* src/VoidEmpires.Application/StrategicMap/GetAllianceReadinessResult.cs
* tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs

## Acceptance criteria

* Strategic map exposes read-only alliance readiness metadata.
* Alliance membership metadata is scoped to the requesting civilization.
* Alliance readiness does not alter visibility.
* Alliance readiness does not add strategic map relevance.
* Ally-owned or foreign systems/fleets are not exposed.
* Diplomatic contacts remain independent from alliance membership.
* Tests cover integration behavior.
* Docs are updated.
* `ai/current-state.md` documents Phase 8U.

## Constraints

* Do not add shared visibility.
* Do not alter `MapVisibilityService`.
* Do not expose ally-owned systems, planets, fleets, transfers, sensors, detection, or interception data.
* Do not add alliance permissions.
* Do not add alliance pacts or treaties.
* Do not add trade.
* Do not add war declarations.
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
   `feat(map): surface alliance readiness metadata`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

