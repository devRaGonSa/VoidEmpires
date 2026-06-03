# TASK-13Z

---
id: TASK-13Z
title: Phase 13Z - Planet cross-surface context with Galaxy and Fleets
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Connect Planet context cleanly with Galaxy and Fleets so the three cockpit surfaces feel related without adding new gameplay systems.

## Context
By this point Galaxy and Planet should already have direct navigation intent, and Fleets may already have useful planet context. This task should make the movement between surfaces clearer while keeping Galaxy read-only and avoiding new global-state complexity.

## Implementation steps

1. Review current route context, query-param handling, and cross-link opportunities across Galaxy, Planet, and Fleets.
2. Add clear links from Planet back to Galaxy selected system and to Fleets when relevant context exists.
3. Ensure Galaxy planet quick links route to Planet and that Fleets adds a non-invasive Planet link when current context supports it.
4. Keep any cross-surface context transfer simple and compatible with current app patterns.

## Files to read first

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`, if present
- Planet route page and helpers
- `src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- Planet route page and helpers
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`, if needed
- small shared navigation helpers, if needed

## Acceptance criteria

- Galaxy can route into Planet from planet quick links.
- Planet can route back into Galaxy and toward Fleets where relevant.
- Fleets can offer a Planet link when existing context supports it.
- No new gameplay mutations are introduced.
- Galaxy remains read-only.

## Constraints

- Prefer query params or simple route state over new shared global state.
- Do not add new fleet mutations.
- Keep the links non-invasive and aligned with current navigation patterns.
- Preserve clear read-only boundaries.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms Galaxy to Planet, Planet to Galaxy, and Planet to Fleets links where data is available

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
