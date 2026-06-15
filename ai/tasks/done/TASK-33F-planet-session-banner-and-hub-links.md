# TASK-33F

---
id: TASK-33F
title: Planet session banner and hub links
status: pending
type: frontend
team: gameplay
supporting_teams: []
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: high
---

## Goal
Integrate the playable session banner into Planet and make Planet a clearer hub for the playable loop.

## Context
After onboarding, Planet should be the natural home page for the selected playable start. Hub links must preserve `civilizationId` and `planetId`.

## Implementation steps

1. Read Planet page behavior, route helpers, and the session banner component.
2. Show the session banner when query context or local session context is available.
3. If query ids are absent but a local session exists, offer continuation.
4. Ensure hub links route clearly to Construction, Research, Shipyard, Defenses, and Fleets.
5. Preserve `civilizationId` and `planetId` in all handoff URLs.
6. Keep backend-first resource refresh/materialization behavior unchanged.
7. Do not add new Planet mutations beyond already supported resource materialization.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx
- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- Optional: src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Planet displays playable session context when available.
- Planet hub links preserve ids.
- Missing-id Planet entry can continue from local session.
- Resource refresh remains backend-first.

## Constraints

- Do not fake resource values.
- Do not optimistic-update resource snapshots.
- Do not add combat, movement, missions, market, or alliance actions.
- Do not perform or claim browser/visual QA.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33F message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
