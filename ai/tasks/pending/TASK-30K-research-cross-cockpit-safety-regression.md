# TASK-30K

---
id: TASK-30K-research-cross-cockpit-safety-regression
title: Verify research mutation does not break cockpit handoffs
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Ensure Research UI mutation does not introduce cross-cockpit side effects.

## Context
This block explicitly adds mutation only in Research and must preserve read-only behavior of neighboring cockpits.

## Implementation steps

1. Review research handoff links to Planet, Construction, Market, Espionage, Ranking, and Galaxy.
2. Verify query params are preserved across handoffs.
3. Confirm no new routes trigger route-level eager imports.
4. Update smoke checklist with Research real-enqueue path.
5. Confirm Construction remains the only previously accepted mutable cockpit.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Expected files to modify

- `docs/dev/frontend-foundation-smoke-checklist.md`

## Acceptance criteria

- Handoffs preserve params and route stability.
- No cross-cockpit mutation path is introduced.
- Lazy-import checks remain valid.

## Constraints

- No gameplay mutation outside Research enqueue flow.
- Do not expand visual QA in this task.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `test(frontend): validate research cross-cockpit safety`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
