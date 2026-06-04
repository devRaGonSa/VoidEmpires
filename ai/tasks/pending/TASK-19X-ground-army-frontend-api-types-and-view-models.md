# TASK-19X-ground-army-frontend-api-types-and-view-models

---
id: TASK-19X-ground-army-frontend-api-types-and-view-models
title: Ground Army frontend API types and view models
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Add typed frontend API access and normalized view models for the Ground Army cockpit.

## Purpose
Keep the Ground Army page from rendering raw backend DTOs directly by introducing the same typed client and normalization layers used by the neighboring cockpit modules.

## Current Problem
The Ground Army page must not render raw backend DTOs. It needs typed API state and view-model normalization for garrison status, troop readiness, resources, queue state, limitations, and action availability.

## Context
- Defenses, Shipyard, and Research already use typed API clients and presentation or view-model helpers.
- Ground Army should follow the same approach to keep page logic readable and future-safe.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Add frontend types for Ground Army UI state, for example:
   - `GroundArmyUiState`
   - `GroundArmyPlanetContext`
   - `GroundArmyResourceStockpile`
   - `GroundStructure`
   - `GroundUnitSummary`
   - `GroundArmyOption`
   - `GroundArmyRequirement`
   - `GroundArmyCost`
   - `GroundArmyQueueItem`
   - `GroundReadinessSummary`
   - `GroundActionAvailability`
   - `GroundDiagnostics`
2. Add an API function such as `fetchGroundArmyUiState(...)`.
3. Add view-model helpers, for example:
   - `mapGroundArmyUiStateToViewModel(...)`
   - `groupGroundOptionsByCategory(...)`
   - `selectRecommendedGroundArmyAction(...)`
   - `getGroundArmyPrimaryAction(...)`
4. Normalize labels, categories, statuses, blockers, costs, and durations for page consumption.
5. Keep raw technical details in diagnostics instead of the primary view.
6. Align naming with the established patterns already used by Research, Shipyard, and Defenses.

## UI/UX Requirements
- The view model must support:
   - dashboard overview
   - garrison and readiness summary
   - structures and troop listings
   - available and blocked options
   - queue state
   - confirmation or safe handoff actions
   - disabled complete-due states
- The main page should be able to render meaningful empty states without reading raw DTO internals.

## Backend/API Requirements
- No backend change is expected unless the endpoint DTO shape is missing essential fields.

## Safety Constraints
- Do not call mutation endpoints in this task.
- No optimistic updates.
- No combat assumptions.

## Expected Files to Modify
- Ground Army API client files under `src/VoidEmpires.Frontend/src/api/`
- Ground Army view-model or presentation helpers under `src/VoidEmpires.Frontend/src/utils/`
- frontend types or page scaffolding files if needed for wiring

## Acceptance Criteria
- Typed Ground Army client and view model exist.
- The frontend compiles.
- Later Ground Army page tasks can use typed state instead of raw backend DTOs.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Some fields may stay optional until backend support matures, but the page should still have a stable type boundary.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on contracts and normalization.
