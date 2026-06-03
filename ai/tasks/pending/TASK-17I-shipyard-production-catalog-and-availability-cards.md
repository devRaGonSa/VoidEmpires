# TASK-17I-shipyard-production-catalog-and-availability-cards

---
id: TASK-17I-shipyard-production-catalog-and-availability-cards
title: Shipyard production catalog and availability cards
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Render the Shipyard production catalog as readable availability cards that clearly separate available, blocked, and unavailable orbital asset options.

## Purpose
Make production choices understandable and visually actionable without exposing raw backend payloads or forcing the player to decode diagnostics.

## Current Problem
Even with a read model and dashboard, Shipyard still needs a production catalog that explains what can be built, what it costs, how long it takes, and why an option is blocked. Without that layer, the page remains informational but not cockpit-like.

## Context
- Research already uses availability-oriented card patterns.
- Construction has readable catalog and queue behavior that Shipyard can echo without copying blindly.
- Shipyard needs to communicate production readiness, not fleet execution or movement.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/` Shipyard mapping and label helpers
- `src/VoidEmpires.Frontend/src/styles.css`
- Shipyard API type and view-model files

## Implementation Requirements
1. Render asset production options as cards or grouped panels rather than raw JSON-like rows.
2. Each option should display, when available:
   - asset name;
   - category or role;
   - production cost;
   - duration;
   - quantity or output unit if relevant;
   - availability state;
   - blocked reason summary if not available.
3. Visually separate:
   - available options;
   - blocked options;
   - unavailable or unsupported options.
4. Add a recommended or highlighted option only if the view model can justify it truthfully.
5. Avoid rendering raw enum names, DTO names, GUIDs, or request payload details in primary UI.
6. Keep diagnostics collapsed and secondary.

## UI/UX Requirements
- Spanish-first labels and action copy.
- Cards should be easy to scan on desktop and mobile.
- A blocked card should still explain itself instead of looking broken.
- Do not imply that every visible card is immediately buildable.

## Backend/API Requirements
- Reuse the Shipyard read model and frontend view model.
- No backend mutation support is required for this task.

## Safety Constraints
- No enqueue action yet unless the later mutation task is complete.
- Do not infer affordability or readiness from incomplete data.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/` Shipyard view-model or presentation helpers
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- The Shipyard catalog shows readable available and blocked production options.
- Blocked reasons are understandable in Spanish.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If the backend catalog is thin, the UI should still render an honest partial catalog.
- Leave confirmation and mutation wiring for the dedicated enqueue task.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Keep structural styling focused on the catalog section.
