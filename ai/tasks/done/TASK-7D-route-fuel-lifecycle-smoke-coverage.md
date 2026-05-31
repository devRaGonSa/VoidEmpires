# TASK-7D

---

id: TASK-7D
title: Add route and fuel lifecycle smoke coverage
status: pending
type: hardening
team: backend
supporting_teams:

* tests
* infrastructure
* web
* docs
  roadmap_item: "Phase 7D - Route/fuel lifecycle smoke coverage"
  priority: high

---

## Goal

Add smoke coverage proving that route profile and fuel readiness remain coherent across the current fleet lifecycle.

This phase should add tests only unless a tiny test helper is needed. It must not add new gameplay behavior.

## Context

The current backend fleet lifecycle already has smoke coverage for:

* seed resources and orbital groups
* preview travel estimate and affordability
* create transfer and charge resources
* reject split/merge while active transfer exists
* cancel transfer and release group
* split group
* merge group back
* create another transfer
* complete transfer
* read final fleet overview

Phase 7A/7B added read-only route profile and fuel readiness. Those concepts now need to be covered in the lifecycle so future changes do not break them.

## Implementation steps

1. Inspect current fleet lifecycle smoke tests.
2. Inspect route profile service/tests.
3. Inspect fuel readiness service/tests.
4. Extend existing lifecycle smoke coverage or add a focused new smoke test class.
5. Cover the following:

   * travel estimate includes route profile data.
   * travel estimate includes fuel readiness data.
   * fuel readiness is read-only and does not mutate stockpiles.
   * route/fuel preview before transfer creation matches the route/fuel metadata used in UI/read models where applicable.
   * transfer creation still charges normal resources but does not charge or persist fuel.
   * cancellation still does not refund resources or fuel because fuel is not real/persisted yet.
   * final overview/UI state remains coherent after completion.
6. Add assertions that explicitly protect current intentional limitations:

   * no fuel inventory entity is required.
   * no route graph/pathfinding is used by the smoke test.
   * fuel readiness is placeholder/read-only.
7. Update docs only if smoke coverage discovers an existing documentation mismatch.
8. Update `ai/current-state.md` to document Phase 7D and the new coverage.

## Files to read first

* tests/VoidEmpires.Tests/FleetLifecycleSmokeTests.cs
* tests/VoidEmpires.Tests/*OrbitalTravelEstimate*.cs
* tests/VoidEmpires.Tests/*OrbitalRoute*.cs
* tests/VoidEmpires.Tests/*Fuel*.cs
* tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetActionManifestServiceTests.cs
* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on current test layout:

* tests/VoidEmpires.Tests/FleetLifecycleSmokeTests.cs
* tests/VoidEmpires.Tests/RouteFuelLifecycleSmokeTests.cs if a separate class is cleaner
* tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs if Phase 7C needs smoke-like coverage there
* docs/dev/fleet-api-contracts.md only if documentation mismatch is found
* ai/current-state.md

If smoke coverage can be added cleanly to the existing lifecycle smoke test, prefer modifying the existing file.

## Acceptance criteria

* Smoke coverage validates route profile and fuel readiness in the fleet lifecycle.
* Tests confirm route/fuel preview is read-only.
* Tests confirm transfer creation still charges normal resources but does not persist/charge fuel.
* Tests confirm cancellation still follows the no-refund policy and does not involve real fuel.
* Tests confirm final overview or UI state remains coherent after route/fuel-aware lifecycle.
* No new gameplay behavior is introduced.
* `ai/current-state.md` documents Phase 7D.

## Constraints

* Prefer tests over production changes.
* Do not add route graph/pathfinding.
* Do not add persisted fuel inventory.
* Do not add refueling.
* Do not add combat, interception, alliances, espionage, or UI.
* Do not add migrations.
* Do not make tests flaky or time-dependent.
* Keep the change focused.

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
3. Verify changed files are tests/docs/current-state unless Phase 7C required small contract changes.
4. Commit with a clear message, for example:
   `test(fleets): add route fuel lifecycle coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
