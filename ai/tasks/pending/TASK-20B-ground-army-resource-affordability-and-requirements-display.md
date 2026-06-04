# TASK-20B-ground-army-resource-affordability-and-requirements-display

---
id: TASK-20B-ground-army-resource-affordability-and-requirements-display
title: Ground Army resource affordability and requirements display
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Make Ground Army affordability and requirements clear using the correct resource scope.

## Purpose
Show players exactly why a terrestrial action is available or blocked without relying on vague generic blockers or misleading frontend-only guesses.

## Current Problem
Ground preparations, building, or training actions may depend on resources, population or manpower, and prerequisites. The UI must clearly show whether an option is available and why not.

## Context
- Construction, Research, Shipyard, and Defenses all needed precise affordability alignment.
- Ground Army should reuse those lessons and avoid generic blockers.

## Files to Inspect First
- Ground Army read model or service
- construction requirement and cost services
- population or manpower logic if present
- resource stockpile logic
- Ground Army presentation helpers
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`

## Implementation Requirements
1. Identify the correct resource scope for Ground Army options:
   - planet stockpile
   - civilization stockpile
   - existing Construction resources
   - population or manpower if present
2. Display the scope with player-facing wording such as:
   - `Reservas de Aurelia`
   - `Capacidad de poblacion`
   - `Preparacion terrestre local`
   - `Reservas disponibles en esta build`
3. Show costs such as:
   - `Creditos`
   - `Metal`
   - `Cristal`
   - `Gas`
   - `Poblacion` or `Manpower` when applicable
4. Show missing resources if available, for example:
   - `Falta Metal X`
   - `Falta Cristal Y`
   - `Falta Poblacion X`
5. Show requirements such as:
   - `Requiere Barracones`
   - `Requiere Academia militar`
   - `Requiere Centro logistico`
   - `Requiere investigacion`
   - `No disponible en esta build`
6. Prefer backend missing amounts if available.
7. Do not use frontend-only availability as the source of truth if the backend says an option is blocked.

## UI/UX Requirements
- Spanish-first.
- The block reason must be visible near the relevant action.
- No raw resource enum names.

## Backend/API Requirements
- If the read model needs explicit missing-resource fields, add focused tests.

## Safety Constraints
- Read paths must not mutate resources.
- No combat.

## Expected Files to Modify
- Ground Army read-model or DTO files if missing-resource fields are needed
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- Ground Army presentation or view-model helpers
- focused tests only if backend fields are added

## Acceptance Criteria
- Available and blocked Ground Army cards are understandable.
- Frontend build passes.
- Backend tests pass if backend changes are made.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` if backend changes are made

## Notes / Residual Risks
- Balance values can change later, but the cockpit must still explain current blockers truthfully.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on affordability and requirements display.
