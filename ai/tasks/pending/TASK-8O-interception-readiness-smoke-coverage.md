# TASK-8O

---

id: TASK-8O
title: Add interception readiness smoke coverage
status: pending
type: hardening
team: backend
supporting_teams:
  - tests
  - docs
  - application
  - infrastructure
  - web
roadmap_item: "Phase 8O - Interception readiness smoke coverage"
priority: high

---

## Goal

Add smoke coverage proving interception readiness metadata is coherent with transfers, detection, sensors, strategic map, and fleet UI state.

This phase should primarily add tests/docs. It must not introduce real interception gameplay.

## Context

The current stack has:

* orbital transfers
* fleet UI state
* strategic map transfer overlays
* sensor profiles
* detection coverage
* interception opportunities
* interception metadata in strategic map/fleet UI state
* dev endpoint/manifest metadata

The smoke test should confirm interception metadata is visible to dev tooling but does not alter gameplay state or reveal hidden data.

## Implementation steps

1. Inspect fleet lifecycle smoke tests.
2. Inspect detection readiness smoke tests.
3. Add or extend smoke coverage:

   * seed requesting civilization with owned planet/fleet
   * create or seed active transfer
   * seed foreign transfer if feasible without leaking hidden data
   * read detection coverage
   * read interception opportunities
   * read strategic map
   * read fleet UI state
   * assert interception metadata appears for own transfer as self-observed/non-hostile
   * assert hidden/foreign transfer data is conservative
   * assert action manifests expose interception readiness read action
   * assert no transfers/groups/resources/knowledge/missions are mutated by interception reads
4. Protect current limitations:

   * no interception execution
   * no combat/damage/resolution
   * no persisted interception state
   * no visibility reveal
   * no espionage/diplomacy
   * no route graph/pathfinding
   * no UI
5. Update docs only if gaps are found.
6. Update `ai/current-state.md` to document Phase 8O and expected final test baseline.

## Files to read first

* tests/VoidEmpires.Tests/FleetLifecycleSmokeTests.cs
* tests/VoidEmpires.Tests/DetectionReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/InterceptionOpportunityServiceTests.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs
* tests/VoidEmpires.Tests/DevInterceptionOpportunityEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* tests/VoidEmpires.Tests/InterceptionReadinessSmokeTests.cs
* tests/VoidEmpires.Tests/FleetLifecycleSmokeTests.cs if extending existing smoke is cleaner
* docs/dev/strategic-map-api-contract.md only if documentation mismatch is found
* docs/dev/fleet-api-contracts.md only if documentation mismatch is found
* ai/current-state.md

May also touch:

* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetActionManifestServiceTests.cs

## Acceptance criteria

* Smoke coverage validates interception metadata with transfers, detection, strategic map, and fleet UI state.
* Tests prove interception readiness does not execute interception or combat.
* Tests prove interception readiness does not mutate persisted state.
* Tests prove hidden/foreign data remains conservative.
* Tests prove action manifests expose interception readiness read metadata.
* Current limitations are protected.
* Docs updated if necessary.
* `ai/current-state.md` documents Phase 8O.

## Constraints

* Prefer tests/docs over production changes.
* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not add interception execution.
* Do not add combat, damage, battle result, or interception resolution.
* Do not persist interception state.
* Do not reveal visibility.
* Do not mutate exploration knowledge, resources, fleets, transfers, or missions.
* Do not add espionage or diplomacy.
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
   `test(interception): add readiness smoke coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

