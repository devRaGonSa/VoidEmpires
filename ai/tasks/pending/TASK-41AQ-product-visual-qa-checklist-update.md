# TASK-41AQ

---
id: TASK-41AQ
title: Product visual QA checklist update
status: pending
type: docs
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Update the deferred visual QA checklist for product-facing review.

## Context
Manual/browser QA is deferred, but the checklist should be ready for the later product-facing review pass.

## Implementation steps

1. Inspect the deferred visual QA master checklist.
2. Add checks that no Development, QA, test, prototype, or similar copy is visible.
3. Add checks for home, onboarding, planet, construction, research, shipyard, and readiness pages.
4. Add checks that operator tools are hidden by default.
5. Do not claim visual QA was performed.

## Files to read first

- docs/dev/deferred-visual-qa-master-checklist.md
- docs/dev/product-readiness-report.md
- docs/dev/product-mode-visibility-contract.md

## Expected files to modify

- docs/dev/deferred-visual-qa-master-checklist.md

## Acceptance criteria

- Visual QA checklist covers product-facing copy removal.
- Checklist covers key pages and readiness pages.
- Checklist verifies operator tools hidden by default.
- No manual QA completion is claimed.

## Constraints

- Do not perform browser/manual QA in this task.
- Do not add screenshots.
- Keep this documentation-only.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`

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
