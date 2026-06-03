# TASK-19T-final-defenses-cockpit-closure-and-regression-pass

---
id: TASK-19T-final-defenses-cockpit-closure-and-regression-pass
title: Final defenses cockpit closure and regression pass
status: completed
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
  - docs
  - qa
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: high
---

## Goal
Close the Defenses cockpit block cleanly after implementation, validation, documentation, and task-lifecycle cleanup are complete.

## Purpose
Leave the repository in a stable, reviewable state while confirming that the new Defenses cockpit did not regress the accepted neighboring cockpits.

## Current Problem
Defenses touches shared route context, seed data, docs, and possibly construction-facing readiness data. A final closure task is needed to verify the full block, move the task queue cleanly, and confirm the broader cockpit baseline still holds.

## Context
- `AGENTS.md` requires queue hygiene and final validation.
- The accepted neighboring cockpits are Galaxy, Planet, Construction, Research, Shipyard, and Fleets.
- Final closure should be narrow and operational, not a place to add extra features.

## Files to Inspect First
- `ai/tasks/pending/`
- `ai/tasks/done/`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/defenses-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `ai/current-state.md`

## Implementation Requirements
1. Move `TASK-19E` through `TASK-19T` into `ai/tasks/done` when they are fully processed.
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
5. Confirm the final expected state:
   - Defenses loads and is not a placeholder
   - Defenses does not execute combat
   - other accepted cockpits remain usable
   - Galaxy remains read-only
6. Ensure the working tree is clean.
7. Create follow-up tasks only if real blockers remain, and create at most `3`.

## UI/UX Requirements
- Closure docs and checklist updates should be screenshot-QA friendly.

## Backend/API Requirements
- No new backend behavior beyond what earlier tasks already required.

## Safety Constraints
- No combat.
- No 3D or WebGL.
- No Galaxy mutations.
- No fleet movement from Defenses.
- No broad speculative follow-up queue.

## Expected Files to Modify
- `ai/tasks/done/` task files moved from this block
- `ai/tasks/pending/` queue cleanup only
- final docs touched by the block
- `ai/current-state.md`

## Acceptance Criteria
- The Defenses cockpit block can be closed after user visual QA.
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
