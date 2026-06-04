# TASK-20A-ground-army-units-structures-and-readiness-catalog

---
id: TASK-20A-ground-army-units-structures-and-readiness-catalog
title: Ground Army units structures and readiness catalog
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Render Ground Army units, structures, or readiness options as readable grouped cards with status, requirements, costs, and availability.

## Purpose
Give Ground Army a roster and readiness catalog comparable in quality to Defenses and Shipyard while staying strictly scoped to terrestrial preparation.

## Current Problem
Ground Army needs a roster or readiness catalog similar to Defenses and Shipyard, but scoped to terrestrial force preparation rather than orbital production or combat execution.

## Context
- Construction may already know how to build military terrestrial structures.
- Ground Army should show those items as readiness and preparation without turning Construction into the primary view for terrestrial posture.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- Ground Army view-model and presentation helpers
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx` for action-card patterns
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx` for safe handoff patterns
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Render ground units, structures, and readiness options grouped by category.
2. Each card should show, where supported:
   - unit, structure, or readiness name
   - category
   - current count, level, or state
   - target level or action if applicable
   - cost
   - duration
   - requirements
   - status
   - primary action or blocked reason
3. Available preparations should show player-facing actions such as:
   - `Revisar preparacion terrestre`
   - `Revisar orden terrestre`
4. Blocked cards should show clear reasons such as:
   - `Faltan recursos`
   - `Requisito pendiente`
   - `No disponible en esta build`
5. If an item belongs to Construction rather than direct Ground Army mutation, show a handoff such as `Gestionar desde Construccion`.
6. Avoid raw enum names and DTO field names in the main catalog.

## UI/UX Requirements
- Cards should be compact and clear.
- Blocked cards must not visually dominate available actions.
- Spanish player-facing copy throughout.

## Backend/API Requirements
- No backend change is expected unless the read model lacks data needed to distinguish card state.

## Safety Constraints
- No combat.
- No direct mutation unless a later confirmation task enables a safe path.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- Ground Army view-model or presentation helpers
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Ground Army catalog and readiness cards render meaningfully.
- Available and blocked states are clear.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- It is acceptable for the initial catalog to be mostly read-only if the mutation path remains unsafe.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on grouped catalog rendering.
