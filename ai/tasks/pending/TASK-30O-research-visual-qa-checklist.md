# TASK-30O

---
id: TASK-30O-research-visual-qa-checklist
title: Add visual QA checklist for Research enqueue UX
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Prepare user-driven visual QA checklist for the Research enqueue workflow.

## Context
No visual QA is being executed automatically; checklist should define what must be observed by hand.

## Implementation steps

1. Update `docs/dev/frontend-foundation-smoke-checklist.md` with Research items:
   - available research selection
   - confirmation panel
   - loading state
   - success state
   - blocked technologies
   - resource/queue/progress refresh
   - no auto-complete control
   - no raw payloads in primary UI
2. Mark items as ready-to-run checks only (not executed).

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify

- `docs/dev/frontend-foundation-smoke-checklist.md`

## Acceptance criteria

- Checklist includes all required Research UX checks.
- No claims of visual QA execution are added.
- No code-side behavior change.

## Constraints

- Keep concise and user-driven.
- No broad visual redesign.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `docs: add visual QA checklist for research enqueue`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
