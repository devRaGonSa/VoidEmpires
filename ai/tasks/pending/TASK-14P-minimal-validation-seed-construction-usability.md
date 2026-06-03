# TASK-14P

---
id: TASK-14P
title: Phase 14P - Minimal validation seed construction usability
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Ensure the minimal-validation seed supports at least one safe, affordable construction action for visual QA.

## Context
The current seed needs to support the new Planet and Construction readability work without making the game feel empty or entirely blocked. This task should keep the seed deterministic and limited while ensuring the QA scenario has both available and blocked actions.

## Implementation steps

1. Inspect the current minimal-validation seed and its construction-related rows.
2. Ensure the seed provides a stable owned planet, visible resources, existing buildings, at least one affordable action, and at least one blocked action.
3. Keep the queue empty or readable initially.
4. Update tests and documentation if seed expectations change.

## Files to read first

- `src/VoidEmpires.Web` seed application code
- `tests/VoidEmpires.Tests/`
- `README.md`
- `ai/current-state.md`

## Expected files to modify

- minimal-validation seed files under `src/VoidEmpires.Web` or infrastructure seed code, if needed
- focused tests for seed expectations, if needed
- supporting documentation, if needed

## Acceptance criteria

- The seed gives QA at least one affordable construction action.
- The seed also leaves at least one blocked action for comparison.
- Construction setup remains deterministic.
- Production behavior is not affected.

## Constraints

- Do not inflate resources unrealistically.
- Do not affect production behavior.
- Keep the change minimal and documented.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

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
