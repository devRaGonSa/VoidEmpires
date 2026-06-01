# TASK-8T

---

id: TASK-8T
title: Add alliance readiness domain foundation
status: pending
type: feature
team: backend
supporting_teams:
  - domain
  - infrastructure
  - application
  - tests
  - docs
roadmap_item: "Phase 8T - Alliance readiness domain foundation"
priority: high

---

## Goal

Add a minimal alliance readiness foundation.

This phase may add lightweight persistent alliance and membership entities, plus read-only query contracts/services. It must not add full alliance gameplay, invitations, permissions, shared visibility, pact systems, diplomacy automation, war declarations, trade, espionage, combat, alliance chat, production endpoints, or final UI.

The purpose is to prepare stable metadata so future UI and gameplay systems can reason about alliance membership without granting gameplay effects yet.

## Context

The project now has diplomatic contacts as read-only readiness metadata. The next controlled step is an alliance readiness foundation.

Current diplomacy/contact constraints must remain:

* diplomatic contacts do not create alliances
* diplomacy contacts do not grant shared visibility
* diplomacy contacts do not create pacts, trade, espionage, war, or combat behavior

Alliance readiness should be similarly conservative:

* an alliance can exist as metadata
* a civilization can be a member
* membership can have role/status metadata
* no gameplay permissions are granted
* no shared visibility is granted
* no war/trade/pact rules are added

Suggested domain objects:

* `Alliance`
* `AllianceMembership`

Suggested enums:

* `AllianceStatus`
* `AllianceMembershipStatus`
* `AllianceMembershipRole`

Keep the model minimal and deterministic.

## Implementation steps

1. Inspect current diplomacy domain:

   * `src/VoidEmpires.Domain/Diplomacy/DiplomaticContact.cs`
   * `src/VoidEmpires.Domain/Diplomacy/DiplomaticContactStatus.cs`
2. Add minimal domain models:

   * `Alliance`
   * `AllianceMembership`
3. Add validation:

   * IDs cannot be empty
   * names/tags cannot be empty
   * timestamps must be UTC
   * membership cannot reference empty alliance/civilization ids
   * enum values must be valid
4. Add EF persistence:

   * DbSet registrations in `VoidEmpiresDbContext`
   * entity configurations
   * deterministic indexes
   * uniqueness constraints for alliance tag and membership pair if feasible
5. Add application contracts:

   * `AllianceReadinessDto`
   * `AllianceMembershipDto`
   * `GetAllianceReadinessRequest`
   * `GetAllianceReadinessResult`
   * `IAllianceReadinessQueryService`
6. Add infrastructure query service:

   * validates non-empty civilization id
   * returns alliance membership rows for the requesting civilization
   * includes minimal alliance metadata for the requesting civilizationâ€™s own memberships
   * scopes by civilization
   * orders deterministically
   * read-only/no mutation
7. Add tests:

   * valid alliance can be created
   * invalid alliance/membership input is rejected
   * EF persists/loads alliance and memberships
   * query service scopes by civilization
   * query service orders deterministically
   * query service is read-only/no mutation
8. Update `ai/current-state.md` to document Phase 8T.

## Files to read first

* src/VoidEmpires.Domain/Diplomacy/
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* src/VoidEmpires.Infrastructure/Persistence/Configurations/DiplomaticContactConfiguration.cs
* src/VoidEmpires.Application/StrategicMap/GetDiplomaticContactsResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DiplomaticContactQueryService.cs
* tests/VoidEmpires.Tests/DiplomaticContactTests.cs
* tests/VoidEmpires.Tests/DiplomaticContactQueryServiceTests.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on conventions:

* src/VoidEmpires.Domain/Diplomacy/Alliance.cs
* src/VoidEmpires.Domain/Diplomacy/AllianceMembership.cs
* src/VoidEmpires.Domain/Diplomacy/AllianceStatus.cs
* src/VoidEmpires.Domain/Diplomacy/AllianceMembershipStatus.cs
* src/VoidEmpires.Domain/Diplomacy/AllianceMembershipRole.cs
* src/VoidEmpires.Infrastructure/Persistence/Configurations/AllianceConfiguration.cs
* src/VoidEmpires.Infrastructure/Persistence/Configurations/AllianceMembershipConfiguration.cs
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* src/VoidEmpires.Application/StrategicMap/GetAllianceReadinessResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/AllianceReadinessQueryService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* tests/VoidEmpires.Tests/AllianceTests.cs
* tests/VoidEmpires.Tests/AlliancePersistenceTests.cs
* tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs
* ai/current-state.md

## Acceptance criteria

* Minimal alliance and alliance membership models exist.
* EF mapping and DbContext registrations exist.
* Read-only alliance readiness query service exists.
* Query service is scoped by requesting civilization.
* Results are deterministic.
* No shared visibility or gameplay permission is granted.
* No alliance invitations, pacts, trade, war, espionage, combat, or final UI are added.
* Tests cover domain, persistence, query behavior, and read-only guarantees.
* `ai/current-state.md` documents Phase 8T.

## Constraints

* Do not add full alliance gameplay.
* Do not add alliance invitations.
* Do not add alliance pacts or treaties.
* Do not add shared visibility.
* Do not alter map visibility.
* Do not mutate diplomatic contacts.
* Do not mutate resources, fleets, transfers, missions, or knowledge.
* Do not add trade.
* Do not add war declarations.
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
   `feat(diplomacy): add alliance readiness foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

