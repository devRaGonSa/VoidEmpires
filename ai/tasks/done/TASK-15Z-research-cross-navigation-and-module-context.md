# TASK-15Z-research-cross-navigation-and-module-context

---
id: TASK-15Z-research-cross-navigation-and-module-context
title: Research cross navigation and module context
status: obsolete
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Integrate Research with Planet, Construction, Galaxy and other module navigation while preserving context.

## Purpose
Research should feel like part of the same cockpit network, not a detached debug page.

## Current Problem
If the module does not preserve `civilizationId` and `planetId`, users will lose context when moving between Planet, Construction, Galaxy and Research.

## Context
- Route helpers already exist and should be reused.
- Manual query string concatenation should be avoided where possible.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- Shared layout or module navigation components

## Implementation Requirements
1. Ensure Planet includes a Research card that links to `/research` with `civilizationId` and `planetId`.
2. Ensure Construction handoff links to Research with context.
3. Ensure Research has links back to:
   - Volver a Planeta;
   - Abrir Construccion;
   - Volver a Galaxia;
   - Abrir Flotas if relevant;
   - optional future links to Astillero, Defensas or Ejercito Tierra.
4. Preserve suspicious-context warnings.
5. Ensure active sidebar state highlights Research.

## UI/UX Requirements
- Link labels should be in Spanish.
- Research must feel like one of the main cabins, not a separate debug route.
- Related module links should be secondary.

## Backend/API Requirements
- No backend change.
- Navigation must not introduce mutations.

## Safety Constraints
- Do not lose context during navigation.
- Do not add unsupported production auth assumptions.
- Do not let navigation imply gameplay changes.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Acceptance Criteria
- Navigation context works end to end.
- No ownership or context error with seeded Aurelia.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Production auth may later remove manual ids.
- Keep route helpers as the single source of truth for these module links.
- Future module cabins should be able to plug into the same pattern.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer route helper reuse over new navigation helpers.
