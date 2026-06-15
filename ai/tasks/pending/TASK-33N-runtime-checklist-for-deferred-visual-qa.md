# TASK-33N

---
id: TASK-33N
title: Runtime checklist for deferred visual QA
status: pending
type: docs
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: medium
---

## Goal
Prepare a deferred visual QA checklist for the integrated playable loop.

## Context
Visual/browser QA is intentionally deferred for this block. The repository should still provide a clear checklist for a later browser pass.

## Implementation steps

1. Update `docs/dev/frontend-foundation-smoke-checklist.md`.
2. Add a deferred visual QA checklist covering:
   - `/onboarding` creates a playable start;
   - local session is saved;
   - Planet opens without manual ids after onboarding;
   - hub links preserve ids;
   - Construction modal still works;
   - Research modal still works;
   - Shipyard modal still works;
   - Planet resource refresh works;
   - Defenses and Fleets remain read-only;
   - no combat, movement, or missions are visible as active actions.
3. Explicitly state that visual QA was not performed in this block.
4. Make no runtime behavior changes.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/planet-cockpit-checklist.md
- docs/dev/construction-cockpit-checklist.md
- docs/dev/research-cockpit-checklist.md
- docs/dev/shipyard-cockpit-checklist.md

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Later browser pass has a clear checklist.
- Documentation explicitly says visual QA was not performed in this block.
- No visual QA overclaim is introduced.
- No behavior changes are made.

## Constraints

- Do not perform browser/visual QA.
- Do not claim screenshots, manual browser testing, or visual validation.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33N message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
