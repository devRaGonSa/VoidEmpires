# TASK-31J

---
id: TASK-31J
title: Planet military and orbital activity summary
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Add a backend-backed or honest Planet dashboard summary for orbital or military preparation without adding any new mutation to Planet.

## Context
Planet already acts as a cockpit hub. This block should strengthen that role by surfacing shipyard activity, defense readiness, and fleet readiness or limitations from existing backend-backed information, while keeping actual mutations delegated to specialized cockpits only.

## Implementation steps

1. Inspect the current Planet page and any backend data it already receives for activity, queue, or orbital context.
2. Add a compact summary section for Shipyard activity, defense readiness, fleet readiness, open production, or honest limitations when data is not yet exposed.
3. Add query-preserving handoff links from Planet to Shipyard, Defenses, and Fleets.
4. Update the Planet cockpit checklist with the new read-only hub behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Web/
- docs/dev/planet-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/planet-cockpit-checklist.md

## Acceptance criteria

- Planet shows a useful orbital or military preparation summary using backend-backed or honest limitation states.
- Planet remains mutation-free for this feature area.
- Query parameters are preserved for Shipyard, Defenses, and Fleets handoffs.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not add new Planet mutations

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
