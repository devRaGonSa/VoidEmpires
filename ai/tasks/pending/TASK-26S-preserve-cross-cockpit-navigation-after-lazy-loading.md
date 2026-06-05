# TASK-26S Preserve Cross Cockpit Navigation After Lazy Loading

---
id: TASK-26S
title: Preserve cross-cockpit navigation and query-param handoffs after lazy loading
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: high
---

## Purpose
Protect cross-cockpit navigation and context-preserving handoffs while the cockpit routes become lazy-loaded.

## Current problem
Route code splitting should not break the query-parameter and navigation flows that move the player between related cockpits.

## Context from current implementation
The accepted cockpit suite relies on shared route helpers and context handoffs between pages such as Planet, Construction, Research, Shipyard, Fleets, Espionage, and Market. Those flows must remain intact after route loading changes.

## Goal
Verify and preserve cross-cockpit navigation behavior, especially query parameters, sidebar active state, and route helper usage.

## Implementation steps
1. Review how `routeUrls.ts` is used across cockpit pages and shell navigation.
2. Confirm that lazy-loaded pages still receive and preserve query parameters.
3. Verify sidebar active-state behavior still updates correctly for all accepted cockpit routes.
4. Update the smoke checklist with a compact navigation regression pass.
5. Rebuild the frontend to confirm the route layer still compiles.

## Files to inspect first
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements
- Review route helper usage and handoff links across:
- Planet
- Construction
- Research
- Shipyard
- Fleets
- Defenses
- GroundArmy
- Espionage
- Market
- Galaxy
- Ensure lazy-loaded pages still receive query parameters.
- Ensure the sidebar active item still updates correctly.
- Update the smoke checklist with this compact navigation regression pass:
- Galaxy -> Planet
- Planet -> Construction
- Construction -> Planet
- Planet -> Research
- Planet -> Shipyard
- Shipyard -> Fleets
- Market -> Planet/Fleets/Galaxy
- Espionage -> Galaxy/Planet/Fleets

## Frontend requirements
- No route names changed.
- No query-parameter loss.
- No visual redesign.

## Backend/API requirements
- None.

## Safety constraints
- No gameplay feature expansion.
- No backend changes.

## Acceptance criteria
- Cross-cockpit navigation remains covered and documented.
- Query-parameter handoffs remain intact.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- This task focuses on route wiring and documentation, not automated browser QA. Visual verification remains user-driven unless the repo already provides a suitable path.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
