# TASK-16Y-research-manual-qa-doc-update-for-enqueue-success

---
id: TASK-16Y-research-manual-qa-doc-update-for-enqueue-success
title: Research manual QA doc update for enqueue success
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: medium
---

## Goal
Update Research QA docs to require a successful enqueue, not just available and blocked visual states.

## Purpose
The docs must explicitly catch the failure that surfaced in QA: an available item opens confirmation but the backend rejects the request. The checklist should now require a successful submit and visible queue update.

## Current Problem
The current checklist may still be too weak if it only asks whether the card is visible and confirmation opens. That would miss the actual bug we are fixing in this block.

## Context
- `docs/dev/research-cockpit-checklist.md` already exists.
- This task should update the checklist so it reflects the corrected usable flow and the enqueue-success acceptance criteria.

## Files to Inspect First
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Update the checklist with:
   - the exact seeded QA URL;
   - confirmation that `Disponibles >= 1`;
   - the available item name, if the seed remains deterministic;
   - the expectation that one confirm action succeeds;
   - the expected post-enqueue queue state;
   - the expected card-state change after success.
2. Include a note that if the user already enqueued once, reseeding may need to reset the environment depending on seed idempotency.
3. Add screenshot targets or explicit checkpoints for:
   - available card before enqueue;
   - confirmation modal;
   - queue after enqueue;
   - blocked card;
   - complete-due placeholder.
4. Make the docs explicit that a generic validation error is a failure, not an acceptable state.

## UI/UX Requirements
- Docs must be clear enough for a user to follow during browser QA.
- Keep the language aligned with the Spanish cockpit UI.

## Backend/API Requirements
- No backend change is expected from documentation work alone.

## Safety Constraints
- Do not document unsupported tech effects.
- Do not imply complete-due is active if it remains disabled.

## Expected Files to Modify
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md` if a small cross-reference is needed

## Acceptance Criteria
- The docs would have caught the current failure.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Manual QA remains required because browser automation may be unavailable.
- Keep the checklist short and operational, not theoretical.
- If a new failure mode appears during the fix, add one targeted note rather than broadening the whole doc.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer updating one checklist and one smoke note.
