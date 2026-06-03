# TASK-17F-shipyard-frontend-api-types-and-view-models

---
id: TASK-17F-shipyard-frontend-api-types-and-view-models
title: Shipyard frontend API types and view models
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Add typed frontend API access and normalized Shipyard view models so the page can render stable cockpit state instead of raw payloads.

## Purpose
Translate backend Shipyard state into a predictable frontend contract that is readable, testable, and consistent with the standards already used by Research and other accepted modules.

## Current Problem
The Shipyard page cannot safely consume backend payloads directly. Without typed DTOs, API helpers, and mapping functions, the UI will either duplicate transport logic or expose technical fields that should remain secondary.

## Context
- Research already introduced typed cockpit API patterns.
- Route helpers and presentation helpers already exist elsewhere in the frontend.
- Shipyard needs support for summary cards, catalog grouping, queue panels, stock summaries, disabled states, and confirmation copy.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Add typed frontend DTOs for the Shipyard UI-state response.
2. Suggested types include:
   - `ShipyardUiState`
   - `ShipyardPlanetContext`
   - `ShipyardResourceStockpile`
   - `ShipyardAssetOption`
   - `ShipyardRequirement`
   - `ShipyardCost`
   - `ShipyardQueueItem`
   - `ShipyardAssetStockItem`
   - `ShipyardActionAvailability`
   - `ShipyardDiagnostics`
3. Add a typed API function such as `fetchShipyardUiState(...)`.
4. Add view-model helpers such as:
   - `mapShipyardUiStateToViewModel(...)`
   - `groupAssetOptionsByCategory(...)`
   - `selectRecommendedAssetProduction(...)`
   - `getShipyardPrimaryAction(...)`
5. Normalize:
   - labels and categories;
   - queue statuses;
   - blocked reasons;
   - costs and durations;
   - stock quantities;
   - diagnostics versus player-facing content.
6. Keep raw payload details in diagnostics only.
7. Do not call mutation endpoints in this task.

## UI/UX Requirements
- The view model must support a cockpit overview, production catalog, queue, stock, confirmation modal state, and blocked action rendering.
- Spanish player-facing copy should be derived from mapping helpers rather than hardcoded in multiple page sections.

## Backend/API Requirements
- No backend change is expected unless a DTO mismatch is discovered.
- If the backend contract changes, keep the frontend mapping explicit and well named.

## Safety Constraints
- No mutation wiring yet.
- No optimistic queue or stock updates.
- Do not add hidden fallback requests that mask backend issues.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/api/` Shipyard client and type files
- `src/VoidEmpires.Frontend/src/utils/` Shipyard mapping or presentation helpers
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`

## Acceptance Criteria
- Shipyard has typed frontend DTOs and mapping helpers.
- The page can consume normalized Shipyard state without rendering raw payloads directly.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Nullable fields are acceptable if the backend surface is still maturing, but the mapper should handle them intentionally.
- Keep the mapping layer small and composable rather than creating one giant transform function.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Split layout work into later tasks if the typing work grows too broad.
