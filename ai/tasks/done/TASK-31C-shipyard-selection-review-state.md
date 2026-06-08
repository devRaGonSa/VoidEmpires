# TASK-31C

---
id: TASK-31C
title: Shipyard selection and review state
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Add explicit Shipyard candidate selection and review state without posting to the backend.

## Context
Construction and Research already use a conservative select -> review -> confirm -> submit pattern. Shipyard needs the same approach so the user can inspect the exact candidate, cost, duration, and queue implications before any real production order is created.

## Implementation steps

1. Inspect the existing Shipyard catalog and determine which rows are truly producible versus blocked or informational.
2. Add explicit local selection state for one Shipyard candidate without triggering any mutation.
3. Render a Spanish-first review summary that explains the selected candidate, available stock if present, quantity model, cost, duration, source context, and expected queue impact.
4. Ensure blocked or unavailable items remain visibly non-executable.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/shipyard-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- A user can select one Shipyard production candidate without mutating anything.
- The review state presents the selected candidate clearly in Spanish-first copy.
- Blocked candidates do not look like safe primary actions.
- No POST is triggered from selection alone.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not submit production orders in this task

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- no new warnings or obvious regressions are introduced

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
