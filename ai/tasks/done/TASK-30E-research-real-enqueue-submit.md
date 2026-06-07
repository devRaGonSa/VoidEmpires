# TASK-30E

---
id: TASK-30E-research-real-enqueue-submit
title: Wire confirmed research to real backend enqueue
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Submit research order only after explicit confirmation and backend success, with safe error/success handling.

## Context
This is the first behavior-changing task in Research. It must preserve backend truth and reject optimism.

## Implementation steps

1. In confirmation action, call `enqueueResearchOrder(...)` from task 30B.
2. Block submit unless `Confirmar investigación` is intentionally pressed.
3. Add loading state to confirm button and prevent duplicate submissions.
4. On success:
   - Show success notice
   - Show order id only in diagnostics/secondary area
   - Display starts/ends times in player-facing format
   - Trigger backend state refresh
5. On known 409 open-order no-op:
   - show Spanish no-op message
   - do not invent a new order state
   - keep selected candidate when useful
6. On known/unknown failures:
   - show Spanish summary
   - preserve selected candidate
   - keep backend validation details in diagnostics
   - do not apply optimistic resource/progress changes

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Acceptance criteria

- Backend real enqueue executes only on confirm.
- No fake success message before backend response.
- Double submit is prevented while loading.
- No mutation without backend success.

## Constraints

- No auto-complete, no simulation of success.
- Preserve backend source of truth.
- No mutation from page load/navigation/hover/selection.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `feat(frontend): connect confirm action to real research enqueue`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
