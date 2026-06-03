# TASK-16K-research-summary-counts-and-recommendation-consistency

---
id: TASK-16K-research-summary-counts-and-recommendation-consistency
title: Research summary counts and recommendation consistency
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: high
---

## Goal
Ensure summary counts and recommendations match the actual catalog state.

## Purpose
The cockpit summary must be truthful. A blocked item should not be presented as the next executable research if the summary says there are zero available items.

## Current Problem
The summary currently suggests a research recommendation like `Ingenieria planetaria` while `Disponibles: 0` and the corresponding card is blocked. That is inconsistent and confuses QA.

## Context
- The frontend already has recommendation selection logic.
- That logic may be selecting a useful future goal rather than a truly available item.
- That is acceptable only if the UI labels it honestly.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`

## Implementation Requirements
1. Recalculate summary counts from the same normalized view model that drives the catalog.
2. Ensure:
   - available count equals the number of cards with an available primary action;
   - blocked count equals the number of blocked cards;
   - queue count equals the queue panel;
   - due count equals due or ready items;
   - completed count equals the completed panel.
3. Update recommendation logic so:
   - if an available technology exists, recommend that;
   - if none are available, label the recommendation honestly as a future goal or blocked objective.
4. If nothing is available, show a safe fallback like:
   - `No hay investigaciones disponibles ahora.`
5. Do not show a blocked item as if it were the next startable item.

## UI/UX Requirements
- Summary must be truthful.
- Spanish labels must remain the default in the main UI.
- Do not create a contradiction between the summary strip and the catalog cards.

## Backend/API Requirements
- No backend change is expected unless the backend already computes an authoritative recommendation.
- If the backend provides a recommendation, prefer it over frontend guessing.

## Safety Constraints
- Do not override backend availability.
- Do not infer actionability from desirability alone.
- Do not mislabel a blocked item as ready.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`

## Acceptance Criteria
- Summary and catalog agree.
- Recommendation text is accurate.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Balance and recommendation heuristics can be improved later.
- A future game-design pass can decide whether there should be a separate `future goal` area.
- The immediate fix is honesty, not clever ranking.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer one shared view-model source of truth.
