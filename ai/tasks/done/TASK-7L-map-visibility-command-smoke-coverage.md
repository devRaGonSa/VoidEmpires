# TASK-7L

---

id: TASK-7L
title: Add visibility and command readiness smoke coverage
status: done
type: hardening
team: backend
supporting_teams:

* tests
* infrastructure
* web
* docs
  roadmap_item: "Phase 7L - Visibility and command readiness smoke coverage"
  priority: high

---

## Goal

Add high-level smoke coverage proving that visibility and command availability remain coherent across the strategic map and related read surfaces.

This phase should add tests only unless a tiny test helper is needed. It must not add new gameplay behavior.

## Context

Phases 7I, 7J, and 7K add:

* map visibility read model
* visibility integrated into strategic map
* strategic map command availability metadata

These concepts need integrated coverage so future UI/gameplay phases do not accidentally leak hidden data, incorrectly mark ownership, or expose unavailable commands.

## Implementation steps

1. Inspect current strategic map readiness smoke tests.
2. Inspect map visibility service tests.
3. Inspect strategic map service tests.
4. Add or extend smoke tests to cover:

   * civilization A owned system/planet visible to civilization A
   * civilization B data not incorrectly marked as owned by civilization A
   * unknown/not-visible nodes have blocked commands
   * owned/visible nodes have expected view commands
   * command availability is read-only metadata
   * strategic map endpoint returns visibility and command availability coherently
   * read surfaces do not mutate stockpiles, groups, transfers, systems, planets, or ownership
5. Add assertions protecting current limitations:

   * no persisted fog-of-war state required
   * no exploration mission is created
   * no sensors/scanners are required
   * no combat/interception data is required
6. Update docs only if smoke coverage discovers a documentation mismatch.
7. Update `ai/current-state.md` to document Phase 7L and the new coverage.

## Files to read first

* tests/VoidEmpires.Tests/StrategicMapReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/MapVisibilityServiceTests.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapEndpointTests.cs
* src/VoidEmpires.Application/StrategicMap/
* src/VoidEmpires.Infrastructure/StrategicMap/
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on current test layout:

* tests/VoidEmpires.Tests/StrategicMapReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/VisibilityCommandReadinessSmokeTests.cs if a separate class is cleaner
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs if extending existing tests is cleaner
* tests/VoidEmpires.Tests/DevStrategicMapEndpointTests.cs if endpoint coverage needs updates
* docs/dev/strategic-map-api-contract.md only if documentation mismatch is found
* ai/current-state.md

## Acceptance criteria

* Smoke coverage validates visibility and command availability together.
* Tests confirm ownership/visibility are scoped correctly.
* Tests confirm blocked commands for unknown/not-visible nodes.
* Tests confirm available commands for owned/visible nodes.
* Tests confirm read surfaces do not mutate persisted state.
* Tests confirm current contracts do not require fog-of-war persistence, exploration missions, sensors, combat, or interception.
* No new gameplay behavior is introduced.
* `ai/current-state.md` documents Phase 7L.

## Constraints

* Prefer tests over production changes.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add persisted fog-of-war/exploration state.
* Do not add exploration missions.
* Do not add sensors/scanners.
* Do not add route graph/pathfinding.
* Do not add combat/interception.
* Do not add alliances or espionage.
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
3. Verify changed files are tests/docs/current-state unless prior tasks required contract/service changes.
4. Commit with a clear message, for example:
   `test(map): add visibility command readiness coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
