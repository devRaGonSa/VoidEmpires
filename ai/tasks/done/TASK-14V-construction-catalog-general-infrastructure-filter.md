# TASK-14V

---
id: TASK-14V
title: Construction catalog general infrastructure filter
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Restrict `/construction` to general colony construction, economy, civil infrastructure and basic logistics.

## Current problem
Visual QA showed that `/construction` still exposes specialized categories such as defenses, ground military, shipyard and research. That makes the screen feel like a universal admin panel instead of a focused construction cockpit.

## Context from current implementation
The cards are already more readable than before, so the next improvement is boundary filtering rather than styling. The backend should keep its data; this task only changes how the frontend groups and presents it.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/construction-cockpit-checklist.md`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Use the taxonomy from `TASK-14U`.
- Keep the primary catalog to:
  - civil / administration buildings;
  - industrial / resource buildings;
  - energy buildings;
  - basic infrastructure and logistics where appropriate.
- Do not show full cards for:
  - defensive structures;
  - ground army structures;
  - shipyard / orbital production structures;
  - research structures;
  unless the taxonomy explicitly marks them as general infrastructure.
- For specialized categories, provide a compact related-modules or handoff section instead of duplicating the full catalog.
- Each related-module entry must preserve `civilizationId` and `planetId`.
- If a related module route is only a placeholder, the link should still be useful but must not imply the feature is fully implemented.

## UI/UX requirements
- `/construction` should feel shorter and more focused.
- The player should immediately understand it is the place for colony infrastructure and economic/civil upgrades.
- Use Spanish-only primary text.
- Keep diagnostics collapsed.

## Backend/API requirements
- No backend change expected.
- Do not remove data from the read model; just filter frontend presentation.

## Safety constraints
- Hidden actions must not become executable from the wrong screen.
- Available construction actions shown on `/construction` must still require explicit confirmation.

## Acceptance criteria
- `/construction` no longer shows full card sections for defenses, ground military, shipyard and research.
- `/construction` shows general construction and infrastructure content.
- Specialized modules are linked as related cabins, not mixed into the main catalog.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Some buildings may be ambiguous. Prefer conservative routing and document ambiguous cases in the code.
