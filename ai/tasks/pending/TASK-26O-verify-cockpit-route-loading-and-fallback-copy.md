# TASK-26O Verify Cockpit Route Loading And Fallback Copy

---
id: TASK-26O
title: Verify cockpit route loading behavior and loading fallback safety
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: high
---

## Purpose
Verify that every accepted cockpit route still loads correctly after route-level lazy loading and that the shared loading experience is safe, Spanish-first, and non-disruptive.

## Current problem
Dynamic imports can introduce blank screens, broken aliases, or poor loading copy if `Suspense` boundaries and fallback states are not verified carefully.

## Context from current implementation
The application already supports a defined accepted cockpit suite. Route-level code splitting must preserve those routes, preserve any `/` to `/galaxy` behavior already present, and keep future placeholders behaving consistently.

## Goal
Confirm that lazy-loaded route definitions, loading copy, and future-module placeholders still work correctly across the accepted cockpit map.

## Implementation steps
1. Review the route definitions and alias behavior in `App.tsx`.
2. Verify the lazy-loaded imports for all accepted cockpit routes.
3. Refine or add a shared loading component if the first implementation leaves an unsafe or unstyled state.
4. Update the smoke checklist with route-loading-specific checks.
5. Confirm future placeholders still render the intended state for non-implemented modules.

## Files to inspect first
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements
- Verify route definitions for:
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
- Ensure loading copy is Spanish-first.
- Ensure missing-context or placeholder states still render after lazy loading.
- Ensure `/` and `/galaxy` alias behavior remains correct if already supported.
- Ensure future modules still show the correct placeholder or future-state UI.

## Frontend requirements
- No route removed.
- No query parameter loss.
- No raw JavaScript loading errors exposed in normal UI.

## Backend/API requirements
- None.

## Safety constraints
- No gameplay changes.
- No backend changes.
- Keep the verification scope focused on routing and loading behavior.

## Acceptance criteria
- All accepted route imports resolve.
- Loading fallback copy is safe and Spanish-first.
- Smoke checklist includes route-loading checks.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- This task may reveal the need for a small shared route-loading component if the initial lazy-loading implementation uses inline fallback markup that is too fragile.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
