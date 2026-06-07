# TASK-30F

---
id: TASK-30F-research-post-submit-refresh-and-delta
title: Reflect backend refresh after research enqueue
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Make post-submission state effects visible and honestly sourced from backend re-read.

## Context
Backend-confirmed enqueue must be explained with visible queue and resource/state deltas after reload.

## Implementation steps

1. Re-fetch Research UI state after successful enqueue.
2. Update visible fields from refreshed model:
   - queue count
   - open/current research
   - resources (if provided)
   - queue item list if supported by read model
3. Add message path when backend row exists but read model does not yet show it:
   - `La investigación fue aceptada por el backend; la cola visible se actualizará con la siguiente lectura disponible.`
4. Keep success flow free from guessed queue display.
5. Ensure no auto-complete actions are introduced during refresh.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Acceptance criteria

- Post-submit UI reflects backend refresh.
- User sees clear message when order is accepted but not yet visible in queue.
- No simulated resource changes.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- Preserve read-model-first behavior.
- Do not fake success visuals.
- Keep messages in Spanish.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `feat(frontend): show backend-backed research enqueue refresh`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
