# TASK-8G

---

id: TASK-8G
title: Add sensor readiness smoke coverage
status: pending
type: hardening
team: backend
supporting_teams:
  - tests
  - docs
  - application
  - infrastructure
  - web
roadmap_item: "Phase 8G - Sensor readiness smoke coverage"
priority: high

---

## Goal

Add smoke coverage proving sensor metadata is coherent with strategic map, exploration knowledge, and visibility read surfaces.

This phase should primarily add tests/docs. It must not introduce real sensor gameplay.

## Context

The current stack has:

* exploration knowledge
* visibility reveal
* strategic map
* exploration tooling
* sensor profiles
* sensor metadata in strategic map
* sensor profile dev endpoint/manifest

The smoke test should confirm sensor metadata is visible to dev tooling but does not alter gameplay state or reveal data.

## Implementation steps

1. Inspect exploration tooling smoke tests.
2. Add or extend smoke coverage:

   * seed owned civilization context
   * seed owned planet and scout orbital group if useful
   * seed unknown target and knowledge-revealed target
   * read sensor profiles
   * read strategic map
   * assert sensor metadata appears for owned/current context
   * assert unknown target is not revealed by sensors
   * assert visibility remains driven by ownership/knowledge, not sensors
   * assert action manifest includes sensor profile action
   * assert no resources/fleets/missions/knowledge are mutated by sensor reads
3. Protect current limitations:

   * no real sensor range
   * no scanner mechanics
   * no combat/interception
   * no espionage/diplomacy
   * no route graph/pathfinding
   * no UI
4. Update docs only if gaps are found.
5. Update `ai/current-state.md` to document Phase 8G and expected final test baseline.

## Files to read first

* tests/VoidEmpires.Tests/ExplorationToolingReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/SensorProfileServiceTests.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevSensorProfileEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* tests/VoidEmpires.Tests/SensorReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/ExplorationToolingReadinessSmokeTests.cs if extending existing smoke is cleaner
* docs/dev/strategic-map-api-contract.md only if documentation mismatch is found
* ai/current-state.md

May also touch:

* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs

## Acceptance criteria

* Smoke coverage validates sensor metadata with strategic map and visibility.
* Tests prove sensors do not reveal unknown targets.
* Tests prove sensors do not mutate persisted state.
* Tests prove action manifest exposes sensor profile read action.
* Current limitations are protected.
* Docs updated if necessary.
* `ai/current-state.md` documents Phase 8G.

## Constraints

* Prefer tests/docs over production changes.
* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not reveal visibility.
* Do not add real sensor range/scanner mechanics.
* Do not mutate exploration knowledge, resources, fleets, or missions.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Keep tests deterministic.

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
3. Verify changed files are expected tests/docs/current-state unless small fixes were necessary.
4. Commit with a clear message, for example:
   `test(sensors): add sensor readiness smoke coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
