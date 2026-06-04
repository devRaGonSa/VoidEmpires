# TASK-20E-ground-army-handoff-to-construction-defenses-fleets

---
id: TASK-20E-ground-army-handoff-to-construction-defenses-fleets
title: Ground Army handoff to Construction Defenses and Fleets
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: medium
---

## Goal
Clarify how Ground Army relates to Construction, Defenses, Fleets, Planet, and Galaxy.

## Purpose
Help players understand module boundaries so Ground Army feels intentional rather than ambiguous or overlapping with other accepted cockpits.

## Current Problem
Ground Army sits between infrastructure, defense readiness, and future invasion or occupation systems. The user must understand what belongs here and what belongs elsewhere.

## Context
- Module boundaries are already important across Construction, Defenses, Fleets, Planet, and Galaxy.
- Ground Army should manage terrestrial readiness, not orbital logistics, strategic map interaction, or combat execution.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`

## Implementation Requirements
1. Add a handoff panel that explains and links to neighboring modules:
   - Construction for barracks, academy, logistics, or military infrastructure if managed there
   - Defenses for protection and shield readiness
   - Fleets for orbital movement and logistics
   - Planet for the overview
   - Galaxy for strategic read-only context
2. Preserve route context across every handoff link.
3. Add route helper support if any relevant helper is still missing.
4. Do not introduce new Fleet behavior.
5. Do not imply invasion or combat execution from Ground Army.

## UI/UX Requirements
- Spanish-first.
- Handoff cards should be secondary to the main Ground Army cockpit data.
- Module boundaries must be explicit and easy to scan.

## Backend/API Requirements
- None.

## Safety Constraints
- No mutations from the handoff panel.
- No fleet movement from Ground Army.
- No invasion.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/styles.css` if the handoff panel needs shared styling

## Acceptance Criteria
- Ground Army explains neighboring modules clearly.
- Handoff links preserve context.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Future invasion-specific modules may introduce new boundaries later, but Ground Army v1 should already prevent major user confusion.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on boundaries and navigation.
