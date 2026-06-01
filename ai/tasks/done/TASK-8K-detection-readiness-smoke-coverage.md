# TASK-8K

---

id: TASK-8K
title: Add detection readiness smoke coverage
status: pending
type: hardening
team: backend
supporting_teams:
  - tests
  - docs
  - application
  - infrastructure
  - web
roadmap_item: "Phase 8K - Detection readiness smoke coverage"
priority: high

---

## Goal

Add smoke coverage proving detection metadata is coherent with sensor profiles, strategic map, exploration knowledge, and visibility read surfaces.

This phase should primarily add tests/docs. It must not introduce real detection gameplay.

## Context

The current stack has:

* exploration knowledge
* visibility reveal
* strategic map
* exploration tooling
* sensor profiles
* sensor metadata in strategic map
* detection coverage
* detection metadata in strategic map
* detection dev endpoint/manifest

The smoke test should confirm detection metadata is visible to dev tooling but does not alter gameplay state or reveal hidden data.

## Implementation steps

1. Inspect sensor readiness smoke tests.
2. Add or extend smoke coverage:

   * seed owned civilization context
   * seed owned planet and scout orbital group if useful
   * seed unknown target and knowledge-revealed target
   * read sensor profiles
   * read detection coverage
   * read strategic map
   * assert detection metadata appears for owned/current/known context
   * assert unknown target is not revealed by detection
   * assert visibility remains driven by ownership/knowledge, not detection
   * assert action manifest includes detection coverage action
   * assert no resources/fleets/missions/knowledge are mutated by detection reads
3. Protect current limitations:

   * no real detection range
   * no scanner mechanics
   * no combat/interception
   * no espionage/diplomacy
   * no route graph/pathfinding
   * no UI
4. Update docs only if gaps are found.
5. Update `ai/current-state.md` to document Phase 8K and expected final test baseline.

## Files to read first

* tests/VoidEmpires.Tests/SensorReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/DetectionCoverageServiceTests.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevDetectionCoverageEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* tests/VoidEmpires.Tests/DetectionReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/SensorReadinessSmokeTests.cs if extending existing smoke is cleaner
* docs/dev/strategic-map-api-contract.md only if documentation mismatch is found
* ai/current-state.md

May also touch:

* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs

## Acceptance criteria

* Smoke coverage validates detection metadata with sensors, strategic map, and visibility.
* Tests prove detection does not reveal unknown targets.
* Tests prove detection does not mutate persisted state.
* Tests prove action manifest exposes detection coverage read action.
* Current limitations are protected.
* Docs updated if necessary.
* `ai/current-state.md` documents Phase 8K.

## Constraints

* Prefer tests/docs over production changes.
* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not reveal visibility.
* Do not add real detection/scanner mechanics.
* Do not persist detection state.
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
   `test(detection): add detection readiness smoke coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
