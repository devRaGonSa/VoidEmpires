# TASK-18F-current-state-update-development-simulation-data-profiles

---
id: TASK-18F-current-state-update-development-simulation-data-profiles
title: Current state update development simulation data profiles
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: medium
---

## Goal
Update `ai/current-state.md` after the development simulation seed profile system is complete and accepted.

## Purpose
Record the new QA seeding baseline so future orchestration knows that manual SQL is no longer the standard approach for cockpit data setup.

## Current Problem
Once reproducible seed profiles are added, `ai/current-state.md` must reflect that change precisely. If it is not updated, future work may keep treating ad hoc DB edits or `minimal-validation` alone as the only available QA path.

## Context
- `ai/current-state.md` is the continuity source for future chats and orchestration.
- The block is about deterministic, idempotent, Development-only seed profiles.
- Existing accepted cockpit state and exclusions must remain intact.

## Files to Inspect First
- `ai/current-state.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/tasks/pending/`
- `ai/tasks/done/`

## Implementation Requirements
1. Update `ai/current-state.md` to include `Phase 18F - Development simulation data profiles and cockpit QA seeds` or equivalent wording.
2. Record, if and only if true after validation, that:
   - `minimal-validation` remains supported;
   - `cockpit-validation` exists for richer QA;
   - optional cockpit-specific profiles exist if they were actually implemented;
   - seed profiles are deterministic, idempotent, and Development-only;
   - manual SQL is not the standard QA path;
   - Galaxy, Fleets, Planet, Construction, Research, and Shipyard are covered by seed QA state;
   - Ground Army and Defenses remain placeholder or readiness cabins unless specifically enriched.
3. Preserve existing accepted cockpit state and exclusions, including:
   - no 3D or WebGL;
   - no combat;
   - no production auth;
   - no WebSockets;
   - no split or merge.
4. Keep the current test count accurate after validation.
5. Move processed task files to `ai/tasks/done` following repo convention.
6. Do not create broad follow-up tasks unless blockers remain.

## UI/UX Requirements
- The current-state update should help future blocks choose the correct seed profile quickly.
- Do not overclaim full gameplay simulation if the profiles are still QA-oriented.

## Backend/API Requirements
- No backend change is expected from this documentation task alone.

## Safety Constraints
- Do not overclaim final gameplay support.
- Do not imply destructive reset support unless it truly exists and is approved.
- Do not leave pending tasks behind unless real blockers require narrow follow-up tasks.

## Expected Files to Modify
- `ai/current-state.md`
- `ai/tasks/done/` seed profile task files
- `ai/tasks/pending/` only if narrow blocker follow-ups are genuinely required

## Acceptance Criteria
- `ai/current-state.md` accurately reflects the development seed profile system.
- `ai/tasks/pending` is empty except `.gitkeep` unless blockers require follow-up tasks.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- This task should be completed last in the block.
- Keep the wording specific enough that future agents know whether to use `minimal-validation`, `cockpit-validation`, or a cockpit-specific profile.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Prefer a narrow continuity update over a broad retrospective rewrite.
