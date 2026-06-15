# TASK-35L

---
id: TASK-35L
title: Deferred visual QA master checklist
status: pending
type: docs
team: frontend
supporting_teams: [gameplay, platform]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: medium
---

## Goal
Create a single master deferred visual QA checklist for the current playable loop.

## Context
Visual/browser QA remains intentionally deferred. A master checklist should prepare one later pass covering the entire playable loop without claiming it has been performed.

## Implementation steps

1. Update `docs/dev/frontend-foundation-smoke-checklist.md` or add `docs/dev/deferred-visual-qa-master-checklist.md`.
2. Include an ordered browser QA plan:
   - start backend;
   - run playable-loop guide;
   - create onboarding session;
   - verify local session banner;
   - verify Planet hub;
   - verify resource materialization;
   - enqueue construction;
   - enqueue research;
   - enqueue shipyard;
   - materialize due queues;
   - verify updated building/research/stock;
   - verify Defenses/Fleets read-only;
   - verify no combat/movement/missions active.
3. Include a screenshot list to capture later.
4. Explicitly state visual QA has not been performed yet.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/planet-cockpit-checklist.md
- docs/dev/construction-cockpit-checklist.md
- docs/dev/research-cockpit-checklist.md
- docs/dev/shipyard-cockpit-checklist.md

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md
- Optional: docs/dev/deferred-visual-qa-master-checklist.md

## Acceptance criteria

- One master checklist exists.
- Screenshot capture list is documented for later.
- Documentation explicitly states visual QA has not been performed yet.
- Validation passes.

## Constraints

- Do not perform browser/visual QA.
- Do not claim screenshots or manual visual verification.
- Do not alter gameplay behavior.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35L message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
