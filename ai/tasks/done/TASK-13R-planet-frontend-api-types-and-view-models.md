# TASK-13R

---
id: TASK-13R
title: Phase 13R - Planet frontend API types and view models
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Add frontend API typing and view-model helpers for the Planet cockpit so raw backend values do not leak into the primary UI.

## Context
Once the Planet cockpit read surface exists, the frontend needs stable TypeScript types, display helpers, and safe normalization of backend values into Spanish-first labels. This should follow the current typed API pattern used by the frontend strategic map and fleet routes.

## Implementation steps

1. Review the existing frontend API type patterns and add Planet cockpit contracts.
2. Define the TypeScript types required by the Planet screen and its supporting panels.
3. Add Spanish display helpers for planet, ownership, colonization, resource, building-category, queue-state, and short-id presentation.
4. Keep missing or unknown data graceful and technical raw values secondary.

## Files to read first

- `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`
- `src/VoidEmpires.Frontend/src/api/strategicMapTypes.ts`
- `src/VoidEmpires.Frontend/src/api/fleetTypes.ts`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`, if present

## Expected files to modify

- `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`
- new or extended Planet API type files under `src/VoidEmpires.Frontend/src/api/`
- frontend Planet label or view-model helpers, if needed

## Acceptance criteria

- Frontend Planet cockpit data has typed contracts for cockpit, summary, resources, buildings, queue items, available actions, and diagnostics.
- Raw enum or backend values are normalized into Spanish display labels.
- Helper functions exist for the required user-facing labels and short ids.
- Missing or unknown values degrade gracefully without crashing the UI.

## Constraints

- Do not leak raw backend names into primary UI labels.
- Follow existing frontend typing conventions.
- Keep the helpers deterministic and lightweight.
- Avoid unrelated route or styling work in this task.

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
