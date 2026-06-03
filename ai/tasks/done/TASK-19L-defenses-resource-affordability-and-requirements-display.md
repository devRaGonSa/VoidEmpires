# TASK-19L-defenses-resource-affordability-and-requirements-display

---
id: TASK-19L-defenses-resource-affordability-and-requirements-display
title: Defenses resource affordability and requirements display
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: medium
---

## Goal
Make defense affordability and prerequisite requirements explicit, using the correct resource scope and truthful blocker explanations.

## Purpose
Help players understand why a defense option is available or blocked without forcing them to infer missing resources or hidden requirements from generic disabled buttons.

## Current Problem
Defense preparations will depend on resources, structures, research, or route ownership. If the cockpit does not explain those blockers clearly, the route will feel inconsistent with Construction, Research, and Shipyard quality.

## Context
- Previous cockpits needed careful affordability alignment between backend authority and frontend display.
- The selected resource scope may be local planet stockpile, broader civilization context, or an existing construction-affordability rule.
- The frontend must reflect backend availability rather than invent its own.

## Files to Inspect First
- Defenses read model or service
- construction requirement or cost services
- resource stockpile logic
- Defenses presentation helpers
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`

## Implementation Requirements
1. Identify the real resource scope that applies to defense-related actions:
   - planet stockpile
   - civilization stockpile
   - existing construction resource rules
2. Display that scope clearly in the cockpit, for example:
   - `Reservas de Aurelia`
   - `Reservas defensivas locales`
   - `Reservas disponibles en esta build`
3. Show costs for the relevant resources, such as:
   - `Creditos`
   - `Metal`
   - `Cristal`
   - `Gas`
   - `Energia` only if the backend actually exposes it
4. Show missing-resource amounts when available:
   - `Falta Metal X`
   - `Falta Cristal Y`
   - `Falta Gas Z`
5. Show prerequisite blockers with specific Spanish wording such as:
   - `Requiere Centro de mando`
   - `Requiere Malla defensiva`
   - `Requiere Astillero`
   - `Requiere investigacion`
   - `No disponible en esta build`
6. Prefer backend-provided missing amounts or blocker reasons when available.
7. Do not override backend blocked state with frontend-only affordability guesses.

## UI/UX Requirements
- Block reasons should sit near the relevant action card.
- Spanish-first labels must avoid raw resource enums or backend reason codes.
- Affordability display should stay compact and readable.

## Backend/API Requirements
- If the read model needs explicit missing-resource or requirement fields, add them conservatively and cover them with tests.
- Do not add mutation behavior here.

## Safety Constraints
- Read path must not mutate resources.
- No combat behavior.
- No fake affordability logic that bypasses backend validation.

## Expected Files to Modify
- Defenses read-model DTO or service files if blocker metadata is missing
- related backend tests if backend changes are made
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- Defenses helper files

## Acceptance Criteria
- Available and blocked defense cards are understandable.
- Resource scope is explicit and consistent with backend authority.
- Frontend build passes, plus backend validation if contracts were adjusted.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` if backend changes are made
- `dotnet test --no-build` if backend changes are made

## Notes / Residual Risks
- Balance values can change later; this task is about correctness and clarity, not tuning.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the scope to affordability and prerequisites only.
