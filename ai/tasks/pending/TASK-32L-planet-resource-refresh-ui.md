# TASK-32L

---
id: TASK-32L
title: Expose backend-backed planet resource progression in the UI
status: pending
type: feature
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Show the new backend-backed resource progression on the Planet page and provide a safe refresh path without simulating growth in the browser.

## Context
Once backend accrual exists, Planet should stop feeling static. The UI should present current resources and any backend-exposed production or last-updated metadata honestly, while preserving query-parameter navigation and avoiding frontend-only fake progress.

## Implementation steps

1. Review the current Planet page data-fetch and navigation handoff behavior.
2. Add a safe refresh action if needed to retrieve or materialize updated resource state from the backend.
3. Display updated resources and any additional rate or timestamp information the backend exposes.
4. Use Spanish-first copy and include honest wording if production or rate data is approximate or unavailable.
5. Preserve links and ids for navigation into other cockpits.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api/*`
- `src/VoidEmpires.Frontend/src/styles.css` only if minimal layout support is needed
- `docs/dev/planet-cockpit-checklist.md` only if behavior notes must be updated

## Acceptance criteria

- Planet shows real backend-backed resource progression.
- Any refresh action uses backend data instead of frontend timers.
- The page keeps Spanish-first copy and preserves existing navigation handoffs.
- Frontend build passes.

## Constraints

- Do not add fake increasing timers.
- Do not mutate outside the supported backend refresh or materialization path.
- Avoid broad redesign of the Planet page.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(planet): expose backend resource progression`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split additional UX polish into later tasks if needed.
