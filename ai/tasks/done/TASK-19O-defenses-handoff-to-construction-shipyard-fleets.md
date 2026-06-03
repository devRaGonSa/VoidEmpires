# TASK-19O-defenses-handoff-to-construction-shipyard-fleets

---
id: TASK-19O-defenses-handoff-to-construction-shipyard-fleets
title: Defenses handoff to construction shipyard fleets
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: medium
---

## Goal
Clarify, in the Defenses cockpit itself, how the route relates to Construction, Shipyard, Fleets, Planet, and Galaxy.

## Purpose
Protect module boundaries so players understand where to go for neighboring actions and so future work does not collapse multiple systems back into one generic planet page.

## Current Problem
Defenses sits close to infrastructure, orbital production, and fleet posture. Without explicit handoff guidance, users may assume the route should handle ship production, fleet commands, or combat.

## Context
- `docs/dev/planet-module-boundaries.md` already defines strong route responsibilities.
- Neighboring cockpits now preserve query context through route helpers.
- Defenses should reinforce those boundaries instead of duplicating other modules.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`

## Implementation Requirements
1. Add a handoff panel or equivalent secondary section for neighboring modules:
   - `Construccion` for defensive infrastructure when owned there
   - `Astillero` for orbital platforms or assets when owned there
   - `Flotas` for group command and movement
   - `Planeta` for overall colony context
   - `Galaxia` for strategic read-only context
2. Preserve `civilizationId` and `planetId` context in all links.
3. Add route helper support if any of these links are still hand-built.
4. Use concise copy that explains module ownership without implying missing gameplay exists here.
5. Do not introduce any new Fleet behavior from this handoff panel.

## UI/UX Requirements
- Handoff cards or links should be visible but clearly secondary to the main Defenses content.
- Copy must be Spanish-first and boundary-oriented.
- The module relationship should be understandable at a glance.

## Backend/API Requirements
- None expected.

## Safety Constraints
- No mutations from the handoff panel itself.
- No fleet movement from Defenses.
- No combat or orbital attack implications.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts` if helper additions are needed
- optional shared styles

## Acceptance Criteria
- Defenses clearly explains neighboring modules.
- Links preserve context correctly.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Future combat or defense-operations modules may add more boundaries later; keep the current wording conservative.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on boundary explanation and navigation.
