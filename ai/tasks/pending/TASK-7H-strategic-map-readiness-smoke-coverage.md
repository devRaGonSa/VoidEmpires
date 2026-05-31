# TASK-7H

---

id: TASK-7H
title: Add strategic map readiness smoke coverage
status: pending
type: hardening
team: backend
supporting_teams:

* tests
* infrastructure
* web
* docs
  roadmap_item: "Phase 7H - Strategic map readiness smoke coverage"
  priority: high

---

## Goal

Add smoke coverage proving that strategic map, visual state, fleet UI state, and action manifests remain coherent together.

This phase should add tests only unless a tiny test helper is needed. It must not add new gameplay behavior.

## Context

The project now has:

* system visual state
* fleet UI state
* fleet action manifest
* strategic map read model
* strategic map dev endpoint
* strategic map action manifest from TASK-7G

These are all read surfaces for future UI/dev tooling. They need integrated smoke coverage so later changes do not break the contract alignment.

The smoke coverage should prove that:

* strategic map returns owned/relevant systems and planets
* visual state still returns planet/system visual layout
* fleet UI state still returns orbital groups and action hints
* strategic map action manifest references available map/visual/fleet read actions
* no read surface mutates persisted state
* no heavy render data is returned

## Implementation steps

1. Inspect existing fleet lifecycle smoke tests.
2. Inspect strategic map service/endpoint tests.
3. Inspect system visual state tests.
4. Inspect fleet UI state tests.
5. Inspect action manifest tests.
6. Add a high-level smoke test class, for example:

   * `StrategicMapReadinessSmokeTests`
7. Cover an integrated read flow:

   * seed civilization, system, planets, ownership, stockpile, orbital group, and active transfer if current test helpers support it
   * call strategic map service
   * call system visual state service or endpoint test helper
   * call fleet UI state service
   * call strategic map action manifest
   * assert each read model returns coherent ids for the same civilization/system/planet/fleet state
   * assert action manifests reference expected routes/action keys
   * assert stockpiles, groups, and transfer states are unchanged after reads
8. Add assertions that protect current intentional limitations:

   * no route graph/pathfinding data is required
   * no mesh/texture/binary/shader payload appears in strategic map contracts
   * no combat/interception data is required yet
9. Update docs only if smoke coverage discovers a documentation mismatch.
10. Update `ai/current-state.md` to document Phase 7H and the new smoke coverage.

## Files to read first

* tests/VoidEmpires.Tests/FleetLifecycleSmokeTests.cs
* tests/VoidEmpires.Tests/*StrategicMap*.cs
* tests/VoidEmpires.Tests/*VisualState*.cs
* tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* src/VoidEmpires.Application/StrategicMap/
* src/VoidEmpires.Infrastructure/StrategicMap/
* src/VoidEmpires.Application/Visuals/
* src/VoidEmpires.Infrastructure/Visuals/
* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on current test layout:

* tests/VoidEmpires.Tests/StrategicMapReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs if extending existing tests is cleaner
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs if TASK-7G needs additional coverage
* tests/VoidEmpires.Tests/TestHelpers/* if a tiny test helper extraction is clearly useful
* docs/dev/strategic-map-api-contract.md only if documentation mismatch is found
* ai/current-state.md

If smoke coverage can be added cleanly to existing strategic map tests, prefer modifying the existing file.

## Acceptance criteria

* Smoke coverage validates strategic map, visual state, fleet UI state, and strategic action manifest together.
* Tests confirm the read surfaces are coherent for the same seeded civilization/system/planet/fleet state.
* Tests confirm read surfaces do not mutate stockpiles, groups, or transfers.
* Tests confirm current contracts do not require heavy render data.
* Tests confirm current contracts do not require route graph/pathfinding or combat/interception.
* No new gameplay behavior is introduced.
* `ai/current-state.md` documents Phase 7H.

## Constraints

* Prefer tests over production changes.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add route graph/pathfinding.
* Do not add combat/interception.
* Do not add alliances or espionage.
* Do not add fuel inventory/refueling.
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
3. Verify changed files are tests/docs/current-state unless TASK-7G required small contract changes.
4. Commit with a clear message, for example:
   `test(map): add strategic map readiness smoke coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
