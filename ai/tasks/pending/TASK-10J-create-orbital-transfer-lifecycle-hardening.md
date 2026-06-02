# TASK-10J

---
id: TASK-10J
title: Phase 10J - Create orbital transfer lifecycle hardening
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10J"
priority: medium
---

## Goal
Harden the create orbital transfer command lifecycle so it consistently reserves a group, charges resources, and creates a transfer only when valid.

## Context
Focus on the create transfer service and endpoint, resource cost logic, group status transitions, the active transfer model, and the tests that cover transfer creation behavior.

## Implementation steps

1. Inspect the create transfer service or endpoint, resource cost logic, group transitions, active transfer model, and transfer creation tests.
2. Tighten or add tests for success, reservation/in-transfer status changes, expected resource deductions, and all listed rejection cases.
3. Preserve the existing API contract unless a clear bug is found.

## Files to read first

- `src/VoidEmpires.Web`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Infrastructure`
- `tests/VoidEmpires.Tests`

## Expected files to modify

- `src/VoidEmpires.Application/*`
- `src/VoidEmpires.Web/*`
- `src/VoidEmpires.Infrastructure/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- Success produces one transfer and reserves the source group.
- Resource balances decrease by the expected estimate.
- Rejected cases do not mutate state.
- Duplicate active transfers and repeated invalid calls remain safe.
- No frontend execution controls, combat, interception, or EF migrations.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end: run `git status`, stage the intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
