# TASK-14J

---
id: TASK-14J
title: Phase 14J - Construction enqueue result and queue refresh
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Make successful construction enqueue visible and understandable.

## Context
When a construction action succeeds, the player should see a clear success message, the queue should refresh, and stale optimistic entries should not linger. This task keeps the mutation feedback readable and aligned with the current backend state.

## Implementation steps

1. Review the enqueue response handling and queue refresh flow.
2. Show a Spanish success message after enqueue.
3. Refresh queue and available actions after success.
4. Keep backend validation failures visible rather than hidden.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `tests/VoidEmpires.Tests/DevConstructionQueueEndpointTests.cs`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- queue refresh or mutation handling helpers, if needed

## Acceptance criteria

- Successful enqueue shows a Spanish success message.
- The queue refreshes after success.
- Available actions update when backend state changes.
- Failed mutations show a clear Spanish error.
- Technical response payloads stay in diagnostics.

## Constraints

- Avoid duplicate optimistic entries unless the backend confirms them.
- Do not hide validation failures.
- Keep the action availability state honest.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore` and `dotnet test --no-build` if backend changes are made
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
