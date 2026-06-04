# TASK-20J-final-ground-army-cockpit-closure-and-regression-pass

---
id: TASK-20J-final-ground-army-cockpit-closure-and-regression-pass
title: Final Ground Army cockpit closure and regression pass
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
  - docs
  - qa
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Close the Ground Army cockpit block cleanly after implementation, validation, documentation, and task-lifecycle cleanup are complete.

## Purpose
Leave the repository in a stable, reviewable state while confirming that the new Ground Army cockpit does not regress the accepted neighboring cockpits.

## Current Problem
Ground Army touches shared route context, seed data, docs, and possibly construction-facing readiness data. A final closure task is needed to verify the full block, move the queue cleanly, and confirm the broader cockpit baseline still holds.

## Context
- `AGENTS.md` requires queue hygiene and final validation.
- The accepted neighboring cockpits are Galaxy, Planet, Construction, Research, Shipyard, Fleets, and Defenses.
- Final closure should be narrow and operational, not a place to add extra features.

## Files to Inspect First
- `ai/tasks/pending/`
- `ai/tasks/done/`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/ground-army-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `ai/current-state.md`

## Implementation Requirements
1. Move `TASK-19U` through `TASK-20J` into `ai/tasks/done` when they are fully processed.
2. Ensure `ai/tasks/pending` contains only `.gitkeep`.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
4. Update docs with the final narrow regression checklist:
   - apply `cockpit-validation` twice
   - open `/galaxy`
   - open `/planet`
   - open `/construction`
   - open `/research`
   - open `/shipyard`
   - open `/fleets`
   - open `/defenses`
   - open `/ground-army`
5. Confirm the final expected state:
   - Ground Army loads and is not a placeholder
   - Ground Army does not execute combat or invasion
   - other accepted cockpits remain usable
   - Galaxy remains read-only
6. Do not create broad follow-up tasks unless real blockers remain.
7. If follow-ups are needed, create at most `3`.
8. Ensure the working tree is clean before closing the block.

## UI/UX Requirements
- Closure docs and checklist updates should be screenshot-QA friendly.

## Backend/API Requirements
- No new backend behavior beyond what earlier Ground Army tasks already required.

## Safety Constraints
- No combat.
- No invasion.
- No 3D or WebGL.
- No Galaxy mutations.
- No fleet movement from Ground Army.
- No broad speculative follow-up queue.

## Expected Files to Modify
- `ai/tasks/done/` task files moved from this block
- `ai/tasks/pending/` queue cleanup only
- final docs touched by the block
- `ai/current-state.md`

## Acceptance Criteria
- The Ground Army cockpit block can be closed after user visual QA.
- `ai/tasks/pending` is empty except `.gitkeep`.
- Build, tests, and frontend build all pass.
- The working tree is clean.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If manual visual QA is still pending at closure time, record that explicitly rather than silently assuming acceptance.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep closure operational and explicit.
