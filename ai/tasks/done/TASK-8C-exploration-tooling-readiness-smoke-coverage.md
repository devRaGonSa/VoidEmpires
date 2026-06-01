# TASK-8C

---

id: TASK-8C
title: Add exploration tooling readiness smoke coverage
status: pending
type: hardening
team: backend
supporting_teams:
  - tests
  - docs
  - application
  - infrastructure
  - web
roadmap_item: "Phase 8C - Exploration tooling readiness smoke coverage"
priority: high

---

## Goal

Add smoke coverage proving the exploration dev tooling read surfaces are coherent together.

The smoke test should validate:

* strategic map
* exploration preview
* mission create
* mission complete due
* knowledge read
* mission list
* strategic map action manifest
* visibility after reveal

This phase should focus on tests and docs. It must not add new gameplay behavior.

## Context

The exploration reveal stack now has:

* exploration preview
* exploration mission lifecycle
* exploration knowledge
* visibility consumes knowledge
* strategic map reveal
* knowledge read endpoint
* mission list endpoint
* action manifest metadata

This task locks the dev tooling lifecycle and ensures the read surfaces remain consistent.

## Implementation steps

1. Inspect existing exploration lifecycle smoke coverage.
2. Add or extend smoke tests to cover:

   * seed owned civilization context and unknown target
   * preview target
   * create mission
   * read mission list and assert planned mission
   * complete due mission
   * read knowledge and assert system/planet knowledge
   * read mission list and assert completed mission
   * read map visibility and strategic map and assert target is revealed conservatively
   * read strategic map action manifest and assert exploration actions are present
   * assert no resources/fleets are mutated
3. Cover endpoint-level read tooling if current test helpers make it practical.

   * Service-level smoke is acceptable if endpoint tests already cover routing/status behavior.
4. Add assertions protecting current limitations:

   * no sensors/scanners
   * no rewards
   * no combat/interception
   * no espionage/diplomacy
   * no route graph/pathfinding
   * no final UI
5. Update docs only if gaps are found.
6. Update `ai/current-state.md` to document Phase 8C and final test baseline.

## Files to read first

* tests/VoidEmpires.Tests/ExplorationMissionLifecycleSmokeTests.cs
* tests/VoidEmpires.Tests/ExplorationKnowledgeQueryServiceTests.cs
* tests/VoidEmpires.Tests/ExplorationMissionQueryServiceTests.cs
* tests/VoidEmpires.Tests/StrategicMapReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* tests/VoidEmpires.Tests/ExplorationToolingReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/ExplorationMissionLifecycleSmokeTests.cs if extending existing test is cleaner
* docs/dev/strategic-map-api-contract.md only if documentation mismatch is found
* ai/current-state.md

May also touch:

* tests/VoidEmpires.Tests/DevExplorationKnowledgeEndpointTests.cs
* tests/VoidEmpires.Tests/DevExplorationMissionEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs

## Acceptance criteria

* Smoke coverage validates the exploration tooling lifecycle.
* Knowledge read and mission list read surfaces are covered.
* Strategic map action manifest includes exploration tooling actions.
* Visibility and strategic map remain coherent after reveal.
* Resources/fleets are not mutated.
* Current limitations are protected by tests.
* Docs are updated if necessary.
* `ai/current-state.md` documents Phase 8C.

## Constraints

* Prefer tests/docs over production changes.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add sensors/scanners.
* Do not add rewards.
* Do not mutate resources/fleets.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Keep tests deterministic and non-flaky.

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
   `test(exploration): add tooling readiness smoke coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
