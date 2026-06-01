# TASK-8X

---

id: TASK-8X
title: Add alliance pact readiness domain foundation
status: pending
type: feature
team: backend
supporting_teams:
  - domain
  - infrastructure
  - application
  - tests
  - docs
roadmap_item: "Phase 8X - Alliance pact readiness domain foundation"
priority: high

---

## Goal

Add a minimal alliance pact readiness foundation.

This phase may add lightweight persistent pact metadata between alliances and a read-only query service. It must not add pact gameplay effects, shared visibility, shared sensors/detection, trade, war, diplomacy automation, combat, espionage, production endpoints, or final UI.

The purpose is to prepare stable metadata so future UI and diplomacy systems can reason about alliance-to-alliance relationships without granting gameplay effects yet.

## Context

The project now has:

* diplomatic contacts
* alliance domain/persistence foundation
* alliance membership readiness
* alliance readiness query service
* alliance readiness dev endpoint
* alliance readiness strategic map metadata

The next controlled step is pact readiness between alliances.

Current constraints must remain:

* alliance membership does not grant shared visibility
* alliance membership does not expose ally systems/fleets
* diplomacy contacts do not imply alliances
* pacts must not create gameplay permissions yet

Suggested domain objects:

* `AlliancePact`

Suggested enums:

* `AlliancePactType`
* `AlliancePactStatus`

Suggested minimal pact types:

* `NonAggression`
* `Cooperation`
* `TradeIntent`
* `MutualDefenseIntent`

These are names/readiness metadata only. They must not create actual trade, war, defense, permissions, or visibility behavior.

## Implementation steps

1. Inspect current alliance domain:

   * `src/VoidEmpires.Domain/Diplomacy/Alliance.cs`
   * `src/VoidEmpires.Domain/Diplomacy/AllianceMembership.cs`
   * alliance status/role/status enums
2. Add minimal domain model:

   * `AlliancePact`
3. Add enums:

   * `AlliancePactType`
   * `AlliancePactStatus`
4. Add validation:

   * ids cannot be empty
   * source alliance and target alliance cannot be the same
   * enum values must be valid
   * timestamps must be UTC
5. Add EF persistence:

   * DbSet registration in `VoidEmpiresDbContext`
   * entity configuration
   * deterministic indexes
   * uniqueness constraint for ordered pair + pact type if feasible
6. Add application contracts:

   * `AlliancePactReadinessDto`
   * `GetAlliancePactReadinessRequest`
   * `GetAlliancePactReadinessResult`
   * `IAlliancePactReadinessQueryService`
7. Add infrastructure query service:

   * validates non-empty civilization id
   * finds alliances where the requesting civilization is an active/member participant according to current alliance membership model
   * returns pact metadata involving those alliances
   * orders deterministically
   * read-only/no mutation
8. Add tests:

   * valid pact can be created
   * invalid self-pact is rejected
   * invalid timestamps/enums are rejected
   * EF persists/loads pacts
   * query service scopes by requesting civilization memberships
   * query service orders deterministically
   * query service is read-only/no mutation
9. Update `ai/current-state.md` to document Phase 8X.

## Files to read first

* src/VoidEmpires.Domain/Diplomacy/
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* src/VoidEmpires.Infrastructure/Persistence/Configurations/AllianceConfiguration.cs
* src/VoidEmpires.Infrastructure/Persistence/Configurations/AllianceMembershipConfiguration.cs
* src/VoidEmpires.Application/StrategicMap/GetAllianceReadinessResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/AllianceReadinessQueryService.cs
* tests/VoidEmpires.Tests/AllianceTests.cs
* tests/VoidEmpires.Tests/AlliancePersistenceTests.cs
* tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on conventions:

* src/VoidEmpires.Domain/Diplomacy/AlliancePact.cs
* src/VoidEmpires.Domain/Diplomacy/AlliancePactType.cs
* src/VoidEmpires.Domain/Diplomacy/AlliancePactStatus.cs
* src/VoidEmpires.Infrastructure/Persistence/Configurations/AlliancePactConfiguration.cs
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* src/VoidEmpires.Application/StrategicMap/GetAlliancePactReadinessResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/AlliancePactReadinessQueryService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* tests/VoidEmpires.Tests/AlliancePactTests.cs
* tests/VoidEmpires.Tests/AlliancePactPersistenceTests.cs
* tests/VoidEmpires.Tests/AlliancePactReadinessQueryServiceTests.cs
* ai/current-state.md

## Acceptance criteria

* Minimal alliance pact model exists.
* EF mapping and DbContext registration exist.
* Read-only alliance pact readiness query service exists.
* Query service is scoped by requesting civilizationâ€™s alliance memberships.
* Results are deterministic.
* No shared visibility, permissions, trade, war, defense, espionage, combat, or gameplay effects are granted.
* Tests cover domain, persistence, query behavior, and read-only guarantees.
* `ai/current-state.md` documents Phase 8X.

## Constraints

* Do not add pact gameplay effects.
* Do not add shared visibility.
* Do not alter map visibility.
* Do not expose ally systems, planets, fleets, transfers, sensors, detection, or interception data.
* Do not add trade behavior.
* Do not add war declarations.
* Do not add defense/intervention behavior.
* Do not add espionage.
* Do not add combat.
* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not apply migrations to a real database.
* Keep the model minimal and conservative.

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
   `feat(diplomacy): add alliance pact readiness foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

