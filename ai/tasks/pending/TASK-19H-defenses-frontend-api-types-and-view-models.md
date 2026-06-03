# TASK-19H-defenses-frontend-api-types-and-view-models

---
id: TASK-19H-defenses-frontend-api-types-and-view-models
title: Defenses frontend api types and view models
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: high
---

## Goal
Add typed frontend API access and normalized view-model helpers for the Defenses cockpit so the page renders deliberate UI state rather than raw backend DTOs.

## Purpose
Keep the new cockpit aligned with the typed client and mapping patterns already used by Shipyard and Research, while centralizing blockers, categories, costs, and readiness display logic.

## Current Problem
Even with a backend read model, the frontend still needs local typing and normalization. Rendering raw DTOs directly would spread formatting rules across the page and make later UI tasks harder to implement safely.

## Context
- The frontend already uses typed API files and per-module mapping helpers.
- Route helpers and shared cockpit layout pieces exist and should be reused.
- Defenses needs a structured view model that can drive overview cards, readiness summaries, action cards, queue sections, and diagnostics.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Add frontend types for the Defenses UI-state contract, for example:
   - `DefensesUiState`
   - `DefensePlanetContext`
   - `DefenseResourceStockpile`
   - `DefenseStructure`
   - `DefenseOption`
   - `DefenseRequirement`
   - `DefenseCost`
   - `DefenseQueueItem`
   - `DefenseReadinessSummary`
   - `DefenseActionAvailability`
   - `DefenseDiagnostics`
2. Add a typed API function such as `fetchDefensesUiState(...)`.
3. Add mapping helpers such as:
   - `mapDefensesUiStateToViewModel(...)`
   - `groupDefenseOptionsByCategory(...)`
   - `selectRecommendedDefenseAction(...)`
   - `getDefensePrimaryAction(...)`
4. Normalize:
   - labels
   - categories
   - statuses
   - blockers
   - costs
   - durations
5. Keep raw backend details in diagnostics instead of the main view model.
6. Follow existing frontend conventions for null handling, diagnostics, and safe fallback rendering.

## UI/UX Requirements
- The resulting view model must support:
  - dashboard overview
  - structures and readiness rendering
  - available and blocked defense options
  - queue display
  - confirmation state
  - disabled complete-due messaging
- The page should not need to interpret raw backend codes directly.

## Backend/API Requirements
- No backend change is expected unless a contract mismatch is discovered.
- If a mismatch is found, update the corresponding backend task or add a tiny supporting fix without broadening scope.

## Safety Constraints
- Do not call mutation endpoints in this task.
- No optimistic updates.
- No combat-related client behavior.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/api/` defense client and types
- `src/VoidEmpires.Frontend/src/utils/` defense mapping helpers
- optional shared route or utility files if needed for type-safe integration

## Acceptance Criteria
- Typed Defenses client and view-model helpers exist.
- The frontend compiles cleanly.
- The Defenses page can consume typed state without relying on raw DTO rendering.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Some fields may remain optional while the backend contract matures, but the mapping layer should still produce truthful UI fallbacks.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the work focused on typing and normalization, not final page behavior.
