# TASK-16Z-current-state-update-research-enqueue-alignment

---
id: TASK-16Z-current-state-update-research-enqueue-alignment
title: Current state Research enqueue alignment update
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: medium
---

## Goal
Update `ai/current-state.md` after the Research enqueue contract is actually aligned.

## Purpose
`ai/current-state.md` is the continuity source for future chats. Once the enqueue flow works, it needs to record the real accepted behavior without overstating final research gameplay.

## Current Problem
The current state may still describe Research as corrected in general, but it needs to reflect the specific ability to confirm and enqueue a seeded available item successfully.

## Context
- `ai/current-state.md` should capture the actual usable flow.
- It should preserve the earlier module boundaries and exclusions.
- It should mention the corrected Research enqueue contract only if the backend and frontend are truly aligned.

## Files to Inspect First
- `ai/current-state.md`
- `docs/dev/research-cockpit-checklist.md`
- `ai/tasks/done/`

## Implementation Requirements
1. Update the current-state phase line or current cockpit baseline to include:
   - `Phase 17B - Research enqueue contract alignment and usable flow closure`
   - or equivalent wording.
2. Record that:
   - Research read model and enqueue endpoint are aligned for the seeded available technology;
   - available Research items can be confirmed and enqueued safely in Development;
   - queue refresh is visible after enqueue;
   - blocked Research items remain non-mutating;
   - complete-due remains disabled or placeholder unless safe;
   - no real technology effects are applied yet;
   - no combat, interception, espionage, 3D, or production auth was introduced.
3. Preserve the previous state about Galaxy, Fleets, Planet, Construction, and module boundaries.
4. Keep the test count accurate after validation.

## UI/UX Requirements
- Documentation must not overclaim final technology-tree support.

## Backend/API Requirements
- No backend change is expected from documentation work alone.

## Safety Constraints
- Do not claim complete-due if it is still disabled.
- Do not imply production readiness.

## Expected Files to Modify
- `ai/current-state.md`
- Possibly a small cross-reference in a Research doc if needed

## Acceptance Criteria
- `ai/current-state.md` reflects the fixed usable Research flow.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Future Research v2 can add effects or unlocks.
- Keep the wording specific enough that future tasks know the exact stable baseline.
- The current state should make it obvious that this block solved a contract mismatch, not a full gameplay expansion.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer a narrow continuity update.
