# TASK-19V-ground-army-taxonomy-and-display-labels

---
id: TASK-19V-ground-army-taxonomy-and-display-labels
title: Ground Army taxonomy and display labels
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Create or consolidate Spanish player-facing labels for troops, garrison structures, training categories, readiness statuses, and Ground Army actions.

## Purpose
Give the Ground Army cockpit clear gameplay language so it does not surface raw enum names, DTO keys, category ids, or backend labels in the player-facing experience.

## Current Problem
Ground Army must not render raw enum names, building type numbers, or backend category values. The cockpit needs domain language that feels like gameplay while remaining honest about unsupported combat and invasion systems.

## Context
- Construction, Research, Shipyard, and Defenses already use presentation helpers to convert backend terminology into player-facing copy.
- Ground Army should follow the same pattern and reuse existing labels where appropriate.
- Diagnostics can keep raw values, but primary copy must be Spanish-first.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/defensesPresentation.ts`
- `src/VoidEmpires.Domain/Buildings/`
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`

## Implementation Requirements
1. Inspect actual military and terrestrial enum, catalog, or manifest values that later Ground Army tasks will need to display.
2. Add or extend frontend presentation helpers, for example:
   - `getGroundUnitLabel(...)`
   - `getGroundStructureLabel(...)`
   - `getGroundArmyCategoryLabel(...)`
   - `getGroundReadinessLabel(...)`
   - `getGroundActionLabel(...)`
   - `formatGroundTrainingCost(...)`
   - `formatGroundTrainingDuration(...)`
3. Use Spanish player-facing labels for known values. Suggested labels if they match real data:
   - `Infanteria ligera`
   - `Infanteria pesada`
   - `Vehiculos blindados`
   - `Artilleria terrestre`
   - `Drones terrestres`
   - `Barracones`
   - `Academia militar`
   - `Centro logistico terrestre`
   - `Guarnicion planetaria`
4. Suggested categories if they align with real data:
   - `Guarnicion`
   - `Reclutamiento`
   - `Entrenamiento`
   - `Logistica terrestre`
   - `Mando terrestre`
   - `Soporte defensivo`
   - `Preparacion colonial`
5. Unknown values must fall back to player-facing placeholders such as:
   - `Unidad terrestre pendiente de clasificar`
   - `Preparacion terrestre pendiente de clasificar`
   and must not fall back to raw enum or camelCase names.
6. Keep raw values only in diagnostics or collapsed technical sections.
7. Do not rename persisted enum values or backend contracts.

## UI/UX Requirements
- Primary UI must be Spanish-first.
- Labels must stay consistent across Ground Army, Construction handoff, Defenses, and Planet module cards.
- Copy must not imply that invasion or combat execution is already implemented.

## Backend/API Requirements
- Prefer frontend-only helpers unless backend display metadata already exists and is clearly the canonical source.

## Safety Constraints
- No gameplay rule changes.
- No stat or balance changes.
- No combat-specific readouts unless they already exist and are intentionally read-only.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/` Ground Army presentation helper file
- related Ground Army page or view-model helper files if wiring is needed

## Acceptance Criteria
- Ground Army labels and categories are centralized.
- Known Ground Army items render with player-facing Spanish names.
- Unknown values render with safe fallback names rather than raw technical identifiers.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` only if backend changes are made

## Notes / Residual Risks
- Final lore and naming can be refined later, but the cockpit must stop leaking raw backend names immediately.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on taxonomy and presentation helpers.
