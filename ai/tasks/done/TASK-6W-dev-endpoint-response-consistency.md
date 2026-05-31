# TASK-6W

---

id: TASK-6W
title: Normalize dev endpoint response consistency
status: pending
type: hardening
team: backend
supporting_teams:

* web
* tests
* docs
  roadmap_item: "Phase 6W - Dev endpoint response consistency"
  priority: medium

---

## Goal

Review and normalize current Development-only fleet endpoint response shapes and status-code behavior where inconsistencies are small and safe to fix.

This task should make dev endpoints easier for frontend tooling and future UI prototypes to consume.

## Context

The project has accumulated several dev endpoints incrementally. They work and are tested, but response conventions may differ slightly across endpoints.

This task should not redesign the API. It should only make small consistency improvements where safe.

Examples of consistency to inspect:

* invalid request returns `400`
* missing persistence returns `503`
* missing entity returns `404`
* state/invariant conflict returns `409`
* successful read-only endpoint returns `200`
* successful mutating command returns existing convention, preferably `200` unless nearby endpoints use `201`
* error response has predictable `error`/`message` fields if current repo has a convention
* route naming follows existing grouping

## Implementation steps

1. Inspect all fleet-related dev endpoints and endpoint tests.
2. Compare response shapes/status codes for:

   * estimate preview
   * transfer create
   * transfer complete
   * transfer cancel
   * transfer lookup/read
   * group split
   * group merge
   * fleet overview
3. Make only small safe consistency fixes.
4. Do not change service behavior unless needed to expose consistent endpoint responses.
5. Update endpoint tests for any normalized status/shape.
6. Update `docs/dev/fleet-api-contracts.md` from TASK-6V if the task changes documented response behavior.
7. Update `ai/current-state.md` to document Phase 6W.

## Files to read first

* src/VoidEmpires.Web/DevEndpointMappings.cs
* src/VoidEmpires.Web/DevOrbital*.cs
* src/VoidEmpires.Web/DevFleet*.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on findings:

* src/VoidEmpires.Web/DevOrbitalTransferCreationEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalTransferCompletionEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalTransferCancelEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalTransferLookupEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalTravelEstimateEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalGroupSplitEndpoints.cs
* src/VoidEmpires.Web/DevOrbitalGroupMergeEndpoints.cs
* src/VoidEmpires.Web/DevFleetOperationalOverviewEndpoints.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* docs/dev/fleet-api-contracts.md
* ai/current-state.md

Only modify files that actually need consistency fixes.

## Acceptance criteria

* Fleet dev endpoint response behavior is reviewed.
* Any small safe inconsistencies are normalized.
* Endpoint tests reflect normalized behavior.
* Documentation stays aligned with behavior.
* No gameplay behavior changes are introduced.
* `ai/current-state.md` documents Phase 6W.

## Constraints

* Do not redesign API.
* Do not create production endpoints.
* Do not modify domain rules unless directly required by endpoint consistency.
* Do not add combat, interception, route graph logic, fuel inventory, alliances, espionage, or UI.
* Do not add migrations.
* Keep changes minimal and test-backed.
* If the review finds no safe inconsistency, update docs/current-state with that finding and keep code unchanged.

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
3. Verify changed files are docs/web/tests/current-state only as needed.
4. Commit with a clear message, for example:
   `chore(dev): normalize fleet endpoint responses`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
