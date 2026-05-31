# TASK-6U

---

id: TASK-6U
title: Add fleet lifecycle smoke coverage
status: pending
type: hardening
team: backend
supporting_teams:

* tests
* infrastructure
* web
  roadmap_item: "Phase 6U - Fleet lifecycle smoke coverage"
  priority: high

---

## Goal

Add high-level smoke coverage for the current fleet lifecycle so future phases cannot easily break the integrated behavior.

This task should validate the current backend fleet loop across existing services/endpoints, without adding new gameplay features.

The smoke coverage should prove that the following lifecycle is coherent:

1. seed resources and orbital groups
2. preview travel estimate and affordability
3. create transfer and charge resources
4. reject split/merge while the group is in active transfer
5. cancel transfer and release group
6. split group
7. merge group back
8. create another transfer
9. complete transfer
10. read final fleet overview from TASK-6T

## Context

The project now has many individually tested operations, but the integration between them is more important from now on.

This task should add broad smoke tests using existing service or endpoint test patterns. It should not create new production behavior unless a tiny helper is needed to make tests maintainable.

The existing `scripts/run-integration-tests.ps1` is currently a placeholder. This task may either:

* leave it as placeholder and add normal xUnit smoke tests, or
* update it only if the repository already has a clear test command convention.

Prefer xUnit smoke tests inside `tests/VoidEmpires.Tests` unless the repo clearly supports script-based integration tests.

## Implementation steps

1. Inspect existing test helpers for EF/in-memory SQLite/web application factory/dev endpoints.
2. Add a high-level smoke test class, for example:

   * `FleetLifecycleSmokeTests`
     or endpoint-specific equivalent.
3. Cover the lifecycle using existing services or dev endpoints:

   * create initial civilization/resource/orbital group test state
   * call travel estimate preview
   * assert affordability data
   * create transfer
   * assert resource balances decreased
   * assert split/merge reject active-transfer group
   * cancel transfer
   * assert group is released according to convention
   * split group
   * merge group back
   * create another transfer
   * complete transfer
   * assert final location/state
   * call fleet overview from TASK-6T and verify final state appears
4. Keep the smoke test deterministic and fast.
5. Avoid duplicating all low-level assertions already covered elsewhere.
6. If tests require small test helper extraction, keep it under `tests/VoidEmpires.Tests`.
7. Update `ai/current-state.md` to document Phase 6U and the smoke coverage.

## Files to read first

* tests/VoidEmpires.Tests/*OrbitalTransfer*.cs
* tests/VoidEmpires.Tests/*OrbitalGroupSplit*.cs
* tests/VoidEmpires.Tests/*OrbitalGroupMerge*.cs
* tests/VoidEmpires.Tests/*OrbitalTravelEstimate*.cs
* tests/VoidEmpires.Tests/*FleetOperationalOverview*.cs
* tests/VoidEmpires.Tests/*ResourceSpend*.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* scripts/run-integration-tests.ps1
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on existing repository conventions:

* tests/VoidEmpires.Tests/FleetLifecycleSmokeTests.cs
* tests/VoidEmpires.Tests/TestHelpers/* if helper extraction is clearly beneficial
* scripts/run-integration-tests.ps1 only if the repo has a clear existing convention for enabling it
* ai/current-state.md

If existing conventions require different filenames, follow repository naming patterns.

## Acceptance criteria

* A high-level fleet lifecycle smoke test exists.
* The smoke test covers preview, create, charge, active-transfer rejections, cancel, split, merge, create again, complete, and overview.
* The smoke test is deterministic and fast.
* The smoke test does not require external services.
* No production gameplay behavior is added.
* `ai/current-state.md` documents Phase 6U.

## Constraints

* Prefer tests over production changes.
* Do not add combat.
* Do not add interception.
* Do not add route graph logic.
* Do not add fuel inventory.
* Do not add alliances, espionage, or UI.
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
3. Verify only expected files changed.
4. Commit with a clear message, for example:
   `test(fleets): add fleet lifecycle smoke coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
