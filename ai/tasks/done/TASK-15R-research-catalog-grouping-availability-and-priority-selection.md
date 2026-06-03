# TASK-15R-research-catalog-grouping-availability-and-priority-selection

---
id: TASK-15R-research-catalog-grouping-availability-and-priority-selection
title: Research catalog grouping availability and priority selection
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Turn the Research catalog into readable sections with availability state and a sensible recommended action.

## Purpose
The Research cockpit should help the player or developer understand what to do next, not just dump a long list of technologies.

## Current Problem
Without grouping, sorting and availability logic, the page becomes visually noisy and difficult to validate during development.

## Context
- The previous module cockpits benefit from stateful grouping and card-level hints.
- Research should use the same principle while staying safe and deterministic.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`

## Implementation Requirements
1. Group catalog entries by category or another stable grouping that matches actual data.
2. Surface the current availability of each item.
3. Sort items in a way that makes the page readable and predictable.
4. Add a recommended research selector for the most relevant available item.
5. Make blocked items explain why they are blocked.
6. Make completed items visibly distinct from available and in-progress items.
7. Keep any score or priority logic deterministic for seed-based QA.

## UI/UX Requirements
- Main content should clearly separate:
  - available items;
  - blocked items;
  - active queue;
  - completed items;
  - recommended next action.
- Cards should remain readable at normal desktop widths.
- Do not let low-value diagnostics overpower the main cockpit.

## Backend/API Requirements
- No backend change expected.
- If the backend does not expose enough data, use the read model from TASK-15O or a safe fallback rather than inventing state.

## Safety Constraints
- Do not infer tech effects from names alone.
- Do not turn recommendation into forced automation.
- Do not mutate state as part of grouping or selection.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts` if new shape fields are needed

## Acceptance Criteria
- The catalog is grouped in a stable, readable way.
- Availability is visible in the main UI.
- A recommended next research can be identified without raw technical labels.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If actual backend availability data is limited, the helper should fail safely and clearly.
- Avoid overfitting the grouping to the current seed.
- Prioritization should remain a hint, not a gameplay rule.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single helper for grouping and selection.
