# TASK-26W Final Route Load Smoke Docs

---
id: TASK-26W
title: Finalize route-loading smoke documentation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: medium
---

## Purpose
Finalize the smoke-test documentation that future manual QA can use after route-level lazy loading changes the way cockpit pages load.

## Current problem
The route-loading model will change from eager to lazy. Without an updated smoke checklist, later visual QA or route QA may miss blank-page or loading-state regressions.

## Context from current implementation
The accepted cockpit suite is already defined. The smoke checklist should cover route opening, loading fallback behavior, and successful cockpit rendering without implying that visual QA was automated in this block.

## Goal
Produce a clear route-loading smoke checklist for accepted and future-placeholder routes.

## Implementation steps
1. Review the current frontend smoke checklist.
2. Add route-loading expectations for accepted cockpit routes.
3. Include the expected loading fallback and failure conditions to watch for.
4. Mention future placeholder routes only if they exist.
5. Keep the checklist practical and compact.

## Files to inspect first
- docs/dev/frontend-foundation-smoke-checklist.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements
- Update the frontend smoke checklist so it instructs the reviewer to:
- open each accepted route
- expect Spanish loading fallback if the route loads slowly
- expect each cockpit to render
- expect diagnostics to remain collapsed where applicable
- expect no blank page
- Include these accepted routes:
- `/galaxy`
- `/planet`
- `/construction`
- `/research`
- `/shipyard`
- `/fleets`
- `/defenses`
- `/ground-army`
- `/espionage`
- `/market`
- Include `/alliance` and `/ranking` only as future placeholder checks if those routes exist.
- No visual QA automation is required in this task.

## Frontend requirements
- None beyond accurate smoke-test guidance.

## Backend/API requirements
- None.

## Safety constraints
- Do not imply that future placeholder routes are implemented if they are not.
- Keep the checklist aligned with actual routes.

## Acceptance criteria
- Smoke docs cover the lazy-loaded route behavior clearly.
- Build and tests still pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Manual QA remains user-driven. This task should make that future QA easier, not pretend it already happened automatically.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
