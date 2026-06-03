# TASK-16N-research-visual-qa-docs-update

---
id: TASK-16N-research-visual-qa-docs-update
title: Research visual QA docs update
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: medium
---

## Goal
Update Research QA documentation with the corrected expected visual state.

## Purpose
The docs must match what the user should actually see after the seed and availability fix. Without that, future QA will continue to expect a broken state or will misread a corrected one.

## Current Problem
The current checklist likely reflects the previous assumption that a seeded available item already existed. That assumption is no longer safe until the new availability fix is in place.

## Context
- `docs/dev/research-cockpit-checklist.md` already exists from the previous block.
- This task should update that doc after the fixes so the next smoke run uses the right expectations.

## Files to Inspect First
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Update the Research checklist with:
   - the exact QA URL;
   - the exact seed command or preparation note;
   - expected summary counts;
   - the expected available technology name;
   - an example blocked technology and its readable blocker;
   - confirmation behavior;
   - complete-due behavior;
   - diagnostics state.
2. State clearly that if a user has already enqueued Research, reapplying the seed may not reset the queue unless the seed explicitly resets it.
3. Document the manual verification steps:
   - apply seed;
   - open `/research`;
   - check the summary;
   - open one confirmation;
   - optionally confirm exactly one research;
   - verify the queue and status refresh.
4. Update the frontend foundation smoke checklist if it needs to reference the corrected Research flow.
5. Do not overclaim any real technology effects.

## UI/UX Requirements
- The docs should be practical for screenshot-based QA.
- Keep the language aligned with the Spanish cockpit terms used in the UI.
- The checklist should be short enough to use live, but precise enough to catch regressions.

## Backend/API Requirements
- No backend change is expected from documentation work alone.

## Safety Constraints
- Do not document unsupported complete-due behavior as implemented.
- Do not claim real research effects.
- Do not make the documentation imply production auth or hidden gameplay.

## Expected Files to Modify
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md` if a small cross-reference is needed

## Acceptance Criteria
- Docs match the corrected real behavior.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Keep the docs aligned with current-state.
- If the seed behavior is still changing, add a short note about what is deterministic and what is not.
- The checklist should help a human reproduce the exact QA scenario quickly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer updating one checklist and one smoke note.
