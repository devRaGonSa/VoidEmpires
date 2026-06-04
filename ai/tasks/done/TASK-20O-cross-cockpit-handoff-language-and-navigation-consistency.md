# TASK-20O-cross-cockpit-handoff-language-and-navigation-consistency

---
id: TASK-20O-cross-cockpit-handoff-language-and-navigation-consistency
title: Cross-cockpit handoff language and navigation consistency
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Standardize handoff panels and navigation language across accepted cockpits.

## Purpose
Make neighboring-module navigation predictable so the demo feels like one cockpit network rather than eight isolated pages with slightly different link language.

## Current Problem
Each cockpit now links to neighbors, but wording and placement differ. Some surfaces say `Abrir`, others `Volver`, `Handoff`, `Cabinas vecinas`, or `Contexto cruzado`. The links work, but the experience is inconsistent.

## Context
- Module boundaries are now central to the product:
   - Galaxy is read-only strategic context
   - Planet is the colonial dashboard
   - Construction builds infrastructure
   - Research advances technology
   - Shipyard produces orbital assets
   - Fleets moves orbital groups
   - Defenses reads and prepares protection
   - Ground Army reads and prepares terrestrial readiness
- Route context must remain preserved between those cabinas.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- all accepted cockpit pages
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/planet-module-boundaries.md`

## Implementation Requirements
1. Standardize the navigation-section naming to one chosen term, such as:
   - `Cabinas relacionadas`
   or
   - `Siguientes cabinas`
2. Standardize common button labels, including:
   - `Volver a Planeta`
   - `Abrir Construccion`
   - `Abrir Investigacion`
   - `Abrir Astillero`
   - `Abrir Flotas`
   - `Volver a Galaxia`
   - `Abrir Defensas`
   - `Abrir Ejercito Tierra`
3. Ensure all relevant route helpers preserve context consistently.
4. Remove misleading handoff copy that implies unsupported mutation or out-of-scope actions.
5. Ensure each cockpit explains what remains outside its scope.

## UI/UX Requirements
- Links should be predictable.
- Spanish-first.
- Navigation should not break or lose context.

## Backend/API Requirements
- None.

## Safety Constraints
- No mutations.
- No new gameplay features.
- No new route semantics beyond consistency improvements.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts` if helper alignment is needed
- targeted cockpit pages
- `src/VoidEmpires.Frontend/src/styles.css`
- optionally `docs/dev/planet-module-boundaries.md` if copy references need syncing

## Acceptance Criteria
- Handoff sections are more consistent across accepted cockpits.
- Context is preserved during navigation.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Exact placement may remain slightly page-specific.
- Consistency matters more than forcing identical layouts.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on language and navigation consistency.
