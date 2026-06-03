# TASK-15P-research-frontend-api-types-and-view-models

---
id: TASK-15P-research-frontend-api-types-and-view-models
title: Research frontend API types and view models
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Add frontend TypeScript types, API client functions and view-model normalization for the Research cockpit.

## Purpose
The Research page should never render backend payloads directly. It needs typed access and a normalized presentation model so the UI stays readable and safe.

## Current Problem
If the page consumes raw DTOs, it will leak backend field names, categories and status codes into the primary UI. That makes later UX and maintenance much harder.

## Context
- The Planet and Construction pages already proved the value of view-model normalization.
- Research should follow the same style and reuse route/context helpers where possible.
- Raw backend fields belong in diagnostics, not in the main cockpit.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`, if present

## Implementation Requirements
1. Add frontend types for Research UI state.
2. Suggested types:
   - `ResearchUiState`
   - `ResearchTechnology`
   - `ResearchCategory`
   - `ResearchRequirement`
   - `ResearchCost`
   - `ResearchQueueItem`
   - `ResearchActionAvailability`
   - `ResearchDiagnostics`
3. Add an API function:
   - `fetchResearchUiState(...)`
   matching the actual endpoint from TASK-15O.
4. Add view-model helpers:
   - `mapResearchUiStateToViewModel(...)`
   - `groupResearchTechnologiesByCategory(...)`
   - `selectRecommendedResearch(...)`
   - `getResearchPrimaryAction(...)`
5. Normalize:
   - category labels;
   - technology names;
   - queue statuses;
   - unavailable reasons;
   - cost display;
   - duration display.
6. Add safe fallbacks for missing backend fields.
7. Keep diagnostics accessible without polluting the main content.

## UI/UX Requirements
- The view model must support:
  - dashboard cards;
  - catalog sections;
  - active queue;
  - completed items;
  - available and blocked actions;
  - confirmation modal state.
- The primary UI must not show raw JSON blocks or technical DTO field names.

## Backend/API Requirements
- Do not change backend in this task unless a type mismatch is discovered and tests are updated.
- Do not introduce mutation calls here.

## Safety Constraints
- API client must not call mutating endpoints yet except in later tasks.
- No optimistic mutation here.
- No hidden parsing assumptions that depend on production-only behavior.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/api/researchApi.ts` or equivalent.
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts` or equivalent.
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts` or equivalent.
- Possibly `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx` for wiring.

## Acceptance Criteria
- Frontend compiles.
- Types are centralized and reusable.
- Research page can use typed state without raw payload rendering.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Some fields may remain nullable until backend support matures.
- Prefer explicit normalization over clever inference.
- This task should set up the shape needed by later mutation and refresh tasks.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single API module and a single view-model helper module.
