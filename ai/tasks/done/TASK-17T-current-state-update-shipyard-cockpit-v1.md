# TASK-17T-current-state-update-shipyard-cockpit-v1

---
id: TASK-17T-current-state-update-shipyard-cockpit-v1
title: Current state update Shipyard cockpit v1
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: medium
---

## Goal
Update `ai/current-state.md` once the Shipyard cockpit foundation block is actually complete and accepted.

## Purpose
Keep the repository continuity document accurate so later orchestration knows exactly what Shipyard v1 supports and what remains intentionally excluded.

## Current Problem
After the Shipyard block lands, the current-state document will need to record the cockpit baseline, the safe development-only behavior, and the boundaries that remain intact. If that continuity step is skipped or exaggerated, future work can easily mix up Shipyard and Fleet responsibilities.

## Context
- `ai/current-state.md` is the continuity source for future chats.
- Shipyard is currently a placeholder-style module boundary route.
- The final v1 state must not overclaim combat, movement, split, merge, 3D, or production auth.

## Files to Inspect First
- `ai/current-state.md`
- `docs/dev/shipyard-cockpit-checklist.md`
- `docs/dev/planet-module-boundaries.md`
- `ai/tasks/pending/`
- `ai/tasks/done/`

## Implementation Requirements
1. Update the phase line to include `Phase 17T - Shipyard cockpit playable foundation v1` or equivalent accepted wording.
2. Record, if and only if true after validation, that:
   - `/shipyard` was upgraded from placeholder to Shipyard cockpit foundation;
   - Shipyard shows production capability, resources, asset options, queue, and stock or readiness;
   - Shipyard enqueue is enabled only if safe dev endpoint support exists, otherwise disabled with explanation;
   - complete-due remains disabled or placeholder unless safe;
   - Shipyard links to Fleets but does not execute fleet movement;
   - split and merge remain disabled;
   - combat remains out of scope;
   - no 3D or WebGL was introduced;
   - Galaxy remains read-only;
   - Planet, Construction, and Research boundaries remain intact.
3. Keep the test count accurate after validation.
4. Move processed Shipyard tasks to `ai/tasks/done` following repo convention.
5. If blockers remain, create at most 3 narrowly scoped follow-up tasks and explain why they are needed.

## UI/UX Requirements
- The continuity note should help future chats understand Shipyard versus Fleet boundaries quickly.
- Do not describe placeholder or disabled actions as fully playable features.

## Backend/API Requirements
- No backend change is expected from this documentation task alone.

## Safety Constraints
- Do not claim final ship production support if the block only achieved readiness or partial dev-only enqueue.
- Do not claim production auth, combat, or Fleet control features.
- Do not leave pending tasks behind unless blockers genuinely remain.

## Expected Files to Modify
- `ai/current-state.md`
- `ai/tasks/done/` Shipyard task files
- `ai/tasks/pending/` only if narrow follow-up tasks are required

## Acceptance Criteria
- `ai/current-state.md` accurately reflects the accepted Shipyard v1 baseline.
- `ai/tasks/pending` is empty except `.gitkeep` unless specific blockers required follow-up tasks.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- This task should be completed last in the block.
- Keep the wording specific enough that future agents know exactly what is stable and what is still intentionally absent.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer a narrow continuity update over a broad retrospective rewrite.
