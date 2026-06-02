# TASK-11X

---
id: TASK-11X
title: Phase 11X - Frontend smoke checklist cancel-flow alignment
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11X"
priority: medium
---

## Goal
Align the frontend smoke checklist with the current controlled cancel-transfer behavior.

## Context
The review pass after Phase 11W found that `docs/dev/frontend-foundation-smoke-checklist.md` still describes cancel as a disabled prototype-only control even though the Fleet page now supports guarded cancel execution after explicit confirmation.

## Implementation steps

1. Review the current fleet mutation docs and the frontend smoke checklist.
2. Update the frontend smoke checklist so it describes estimate, create-transfer, and cancel-transfer accurately.
3. Keep complete-due, split, and merge documented as disabled or prototype-only.
4. Preserve the current non-visual validation emphasis and manual-browser deferral guidance.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/fleet-controlled-mutation-checklist.md
- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Frontend/README.md

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- The frontend smoke checklist states that cancel-transfer can execute only through the explicit confirmation path.
- The checklist continues to describe complete-due, split, and merge as disabled or prototype-only.
- The checklist keeps the current non-visual validation commands and manual-browser deferral guidance.
- Validation commands pass.

## Constraints

- Keep the task documentation-only.
- Do not modify unrelated files.
- Keep the change minimal.

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
- Prefer a single commit for this task.
