# TASK-16L-research-resource-affordability-source-and-display-fix

---
id: TASK-16L-research-resource-affordability-source-and-display-fix
title: Research resource affordability source and display fix
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: high
---

## Goal
Fix research affordability display so available and blocked status reflects the correct resource scope.

## Purpose
The current UI implies `Recursos insuficientes` for every item. That may be correct, but it may also be a scope mismatch between planet stockpile, civilization resources, or the backend read model. We need the real source and a truthful display.

## Current Problem
It is unclear whether the cockpit is reading the correct resources for affordability. If the wrong scope is used, every item can appear blocked even when the seed intended one item to be available.

## Context
- Construction uses planet stockpile.
- Research may use the same scope or another one depending on backend design.
- The UI must not guess incorrectly.

## Files to Inspect First
- Research backend service or read model
- Resource affordability or spend services
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Implementation Requirements
1. Identify the resource scope actually used for Research availability.
2. Ensure the UI displays that scope accurately, for example:
   - `Reservas de Aurelia`
   - `Reservas de civilizacion`
   - `Reservas disponibles en esta build`
3. If the frontend computes missing amounts, make sure it uses the same resource balances as the backend.
4. If the backend already provides affordability and missing amounts, prefer backend data over frontend inference.
5. Ensure the seeded available item has sufficient resources under the correct scope.
6. Show missing resources for blocked items when that detail is available:
   - `Falta Metal X`
   - `Falta Cristal Y`
   - `Falta Gas Z`
7. Avoid a generic `Recursos insuficientes` when more detail is already known.

## UI/UX Requirements
- Spanish-first copy.
- Cost and missing amounts must be easy to read.
- Raw resource enum names should not dominate the display.

## Backend/API Requirements
- If adding missing-resource fields to the Research UI state, add tests.
- Backend validation remains authoritative.
- Do not mutate resources in the read path.

## Safety Constraints
- Do not invent a new empire-wide resource model unless the backend already has it.
- Do not change combat, fleet, or construction resource models to support this task.
- Do not misreport affordability based on a frontend heuristic.

## Expected Files to Modify
- Research backend service or UI-state model if needed
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- Tests for the relevant backend contract if the model changes

## Acceptance Criteria
- At least one seeded item is affordable.
- Blocked items show clear missing resources when applicable.
- Build, tests, and frontend build pass.

## Validation
- `dotnet build --no-restore` if backend changes are made.
- `dotnet test --no-build` if backend changes are made.
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Long-term research may need its own empire-wide resource model; do not add that here unless the backend already requires it.
- If affordability is intentionally global, the UI must say so clearly instead of implying planet stockpiles.
- The display should help QA understand why a card is blocked, not just whether it is blocked.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer display corrections over model redesign.
