# TASK-20Z-final-cross-cockpit-polish-closure

---
id: TASK-20Z-final-cross-cockpit-polish-closure
title: Final cross-cockpit polish closure
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
  - docs
  - qa
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Close the cross-cockpit UX polish block and ensure workflow cleanup.

## Purpose
Leave the repository in a stable, reviewable state after the polish block while confirming the accepted cockpit set remains coherent and no new gameplay systems slipped in.

## Current Problem
The block should leave the repo clean, with pending tasks moved, docs updated, and validation passing. Shared polish work is especially prone to leaving scattered follow-up debt if closure is not explicit.

## Context
- `AGENTS.md` requires queue hygiene and clean closure.
- This block is intentionally polish-oriented rather than feature-expansion oriented.
- Closure should confirm that the accepted cockpit set remains usable and that `cockpit-validation` is still demo-ready.

## Files to Inspect First
- `ai/tasks/pending/`
- `ai/tasks/done/`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `ai/current-state.md`

## Implementation Requirements
1. Move `TASK-20K` through `TASK-20Z` into `ai/tasks/done` when they are fully processed.
2. Ensure `ai/tasks/pending` contains only `.gitkeep`.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
4. Confirm the final expected state:
   - all accepted cockpits remain usable
   - primary copy is less technical
   - diagnostics remain available but secondary
   - handoffs are more consistent
   - sidebar reflects accepted and future modules more clearly
   - `cockpit-validation` remains idempotent and demo-ready
   - no new gameplay systems were introduced
5. Do not create broad follow-up tasks unless real blockers remain.
6. If follow-ups are needed, create at most `3` specific tasks.
7. Ensure the working tree is clean before closing the block.

## UI/UX Requirements
- Closure docs should remain screenshot-QA friendly.

## Backend/API Requirements
- None unless earlier tasks in the block required backend changes.

## Safety Constraints
- No combat.
- No invasion.
- No 3D or WebGL.
- No new gameplay systems.
- No production auth.

## Expected Files to Modify
- `ai/tasks/done/` task files moved from this block
- `ai/tasks/pending/` queue cleanup only
- final docs touched by the block
- `ai/current-state.md`

## Acceptance Criteria
- The block can be closed after user visual QA.
- `ai/tasks/pending` is empty except `.gitkeep`.
- Validation passes.
- The working tree is clean.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If visual QA is still pending at closure time, record that explicitly rather than assuming polish equals acceptance.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep closure operational and explicit.
