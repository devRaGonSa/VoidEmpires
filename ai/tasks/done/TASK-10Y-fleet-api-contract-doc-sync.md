# TASK-10Y

---
id: TASK-10Y
title: Phase 10Y - Fleet API contract doc sync for split and merge statuses
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10Y"
priority: low
---

## Goal
Update the fleet development API contract documentation so split and merge match the current endpoint response behavior.

## Context
`TASK-10W` added explicit `status` values and `404` handling for missing split or merge groups, but `docs/dev/fleet-api-contracts.md` still describes the older payload and status-code behavior.

## Implementation steps

1. Inspect the split and merge endpoint implementations and the fleet API contract doc.
2. Update the documented common responses and split/merge sections to match the live endpoints.
3. Validate the solution and keep the task within the small-change budget.

## Files to read first

- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Web/DevOrbitalGroupSplitEndpoints.cs
- src/VoidEmpires.Web/DevOrbitalGroupMergeEndpoints.cs
- tests/VoidEmpires.Tests/DevOrbitalGroupSplitEndpointTests.cs
- tests/VoidEmpires.Tests/DevOrbitalGroupMergeEndpointTests.cs

## Expected files to modify

- docs/dev/fleet-api-contracts.md

## Acceptance criteria

- Split and merge docs mention the `status` field.
- Missing split or merge groups are documented as `404`.
- Validation commands pass.
- No build artifacts are committed.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- repository-relevant build steps succeed, if applicable
- repository-relevant tests succeed, if applicable
- no new warnings or obvious regressions are introduced

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
