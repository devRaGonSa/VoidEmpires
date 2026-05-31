# TASK-7B

---

id: TASK-7B
title: Add orbital fuel readiness preview
status: pending
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* web
* tests
* docs
  roadmap_item: "Phase 7B - Orbital fuel readiness preview"
  priority: high

---

## Goal

Add a read-only fuel readiness preview for orbital travel.

This phase must not introduce real fuel inventory, fuel charging, fuel persistence, route graph pathfinding, combat, interception, alliances, espionage, or final UI.

The goal is to expose whether a group appears fuel-ready according to derived placeholder rules, so future fuel gameplay can be introduced without changing UI contracts later.

## Context

The project already estimates travel distance, duration, resource costs, affordability, and now route profile from TASK-7A.

The future game will likely need fuel/range constraints. However, real fuel stockpiles and refueling are not ready yet.

This task adds a read-only preview model:

* estimated fuel units required
* estimated range units available
* whether the route is fuel-ready
* reason if not fuel-ready
* fuel policy is explicitly placeholder/derived, not persisted inventory

This must be documented clearly to avoid confusing it with real fuel economy.

## Implementation steps

1. Read TASK-7A route profile implementation.
2. Inspect existing orbital travel estimate and affordability preview.
3. Add application contracts for fuel readiness, for example:

   * `OrbitalFuelReadinessDto`
   * `OrbitalFuelReadinessPolicy`
   * `IOrbitalFuelReadinessService`
4. Implement a deterministic placeholder policy based on:

   * `SpaceAssetType`
   * route distance/profile
   * group quantity if appropriate
5. Integrate fuel readiness into the existing travel estimate preview response if clean.
6. Keep the result read-only:

   * no fuel stockpile entity
   * no persisted fuel state
   * no transfer creation change
   * no resource spending
7. Add tests:

   * fuel readiness is deterministic
   * short route is ready for basic craft
   * very long route can be marked not ready according to policy
   * different asset types may have different range/fuel behavior if existing model supports it
   * travel estimate preview includes fuel readiness if integrated there
   * no stockpile/resource mutation occurs
8. Update `docs/dev/fleet-api-contracts.md` to document that fuel readiness is preview-only and placeholder.
9. Update `ai/current-state.md` to document Phase 7B.

## Files to read first

* src/VoidEmpires.Domain/Fleets/
* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* src/VoidEmpires.Web/DevOrbitalTravelEstimateEndpoints.cs
* tests/VoidEmpires.Tests/*OrbitalTravelEstimate*.cs
* tests/VoidEmpires.Tests/*OrbitalRoute*.cs
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on repository conventions:

* src/VoidEmpires.Application/Fleets/OrbitalFuelReadinessDto.cs
* src/VoidEmpires.Application/Fleets/IOrbitalFuelReadinessService.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalFuelReadinessService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Application/Fleets/EstimateOrbitalTravelResult.cs
* src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs
* src/VoidEmpires.Web/DevOrbitalTravelEstimateEndpoints.cs
* tests/VoidEmpires.Tests/OrbitalFuelReadinessServiceTests.cs
* tests/VoidEmpires.Tests/OrbitalTravelEstimateServiceTests.cs
* tests/VoidEmpires.Tests/DevOrbitalTravelEstimateEndpointTests.cs
* docs/dev/fleet-api-contracts.md
* ai/current-state.md

If existing conventions require different filenames, follow repository naming patterns.

## Acceptance criteria

* Fuel readiness preview exists and is read-only.
* Fuel readiness is deterministic.
* No fuel inventory is persisted.
* No resources are charged.
* Real transfer creation behavior is not changed.
* Travel estimate preview includes or can access fuel readiness data.
* Documentation clearly states this is placeholder fuel readiness, not real fuel economy.
* Tests cover ready/not-ready cases and no mutation behavior.
* `ai/current-state.md` documents Phase 7B.

## Constraints

* Do not add persisted fuel inventory.
* Do not add refueling.
* Do not change transfer creation behavior.
* Do not charge resources or fuel.
* Do not add route graph/pathfinding.
* Do not add combat, interception, alliances, espionage, or final UI.
* Do not add migrations unless absolutely required; if a migration appears necessary, stop and create a follow-up task.
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
   `feat(fleets): add orbital fuel readiness preview`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
