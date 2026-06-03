# TASK-15Q-research-route-context-loading-and-placeholder-upgrade

---
id: TASK-15Q-research-route-context-loading-and-placeholder-upgrade
title: Research route context loading and placeholder upgrade
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Upgrade `/research` from a placeholder or readiness screen to a data-loading Research cockpit shell with context.

## Purpose
The Research page should feel like a real module cabin. It needs to load state, preserve context and show useful loading, empty and error states instead of a static placeholder.

## Current Problem
The Research cabin is still a boundary placeholder. Without route-aware loading it will not fit into the Planet and Construction handoff flow.

## Context
- Preserve `civilizationId` and optional `planetId`.
- Use shared route helpers rather than manual query string concatenation.
- Keep the boundary explanation if it is still useful, but integrate it into the cockpit header.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Ensure `/research` accepts:
   - `civilizationId`
   - optional `planetId`
2. Load Research UI state from the endpoint or client established earlier.
3. Show loading state in Spanish.
4. Show error state in Spanish.
5. Show empty state in Spanish if no research catalog is available.
6. Keep suspicious-context warning behavior if `civilizationId` looks like a planet id.
7. Preserve links:
   - Volver a Planeta;
   - Abrir Construccion;
   - Volver a Galaxia;
   - Abrir Flotas if context exists.
8. Do not remove the useful boundary explanation unless the new cockpit header fully replaces it.

## UI/UX Requirements
- Page title should be clearly `Investigacion`.
- It should feel like a cockpit, not a generic placeholder.
- Use the same dark galactic command style as the other module cabins.
- Diagnostics should be collapsed by default.
- Empty and error states should still look intentional, not like missing data.

## Backend/API Requirements
- No backend changes expected if earlier tasks already created the endpoint or client.
- If backend support is missing, keep the page stable and explain the limitation.

## Safety Constraints
- Do not add mutation actions in this task.
- Do not hide route problems.
- Do not lose context when navigating back and forth.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts` if helpers are missing or incomplete
- `src/VoidEmpires.Frontend/src/styles.css` if the shell needs minimal layout support

## Acceptance Criteria
- `/research` loads with context-aware state.
- Loading, error and empty states are clear.
- Navigation preserves context.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If the read model is missing, the page should still render a safe shell with a precise explanation.
- Do not confuse a readiness screen with a playable cockpit.
- Keep the implementation focused on context and loading only.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer to reuse the existing module shell pattern.
