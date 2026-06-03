# TASK-16J-research-complete-due-button-truthful-placeholder

---
id: TASK-16J-research-complete-due-button-truthful-placeholder
title: Research complete-due button truthful placeholder
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: high
---

## Goal
Fix the visual contradiction around `Completar investigaciones vencidas`.

## Purpose
The cockpit must not present an unavailable action as if it were executable. If complete-due is not safe from Research, it must look disabled and read like a placeholder.

## Current Problem
The UI currently shows `Completar investigaciones vencidas` with an active-looking button style, but the explanatory text says the action is not available from Research in this build. That is misleading.

## Context
- Construction already uses safe disabled placeholder behavior when complete-due is not available.
- Research should do the same unless a safe, tested Development-only complete-due path is actually available.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- Any Research complete-due backend endpoint or tests, if present

## Implementation Requirements
1. Determine whether complete-due is actually safe and available from Research.
2. If it is not safe:
   - render it as disabled or secondary;
   - use a label such as `Completar vencidas no disponible` or `No disponible en esta build`;
   - do not attach a mutation handler;
   - keep the explanation text.
3. If it is safe:
   - require explicit confirmation;
   - show due count;
   - confirm before mutation;
   - refresh state after success;
   - add tests if the backend path changes or is newly used.
4. Prefer the conservative default: disabled placeholder until the safe path is proven.

## UI/UX Requirements
- Do not show a blue, active-looking button for an unavailable action.
- Disabled state must be visually obvious.
- Spanish copy must be used in the primary UI.

## Backend/API Requirements
- No backend change is expected unless enabling complete-due safely.
- If safe support is added, it must be tested and scoped to the dev workflow.

## Safety Constraints
- Do not add background worker control.
- Do not apply research effects unexpectedly.
- Do not expose global unsafe completion as though it were cockpit-scoped.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- Backend files only if safe complete-due support is added

## Acceptance Criteria
- The complete-due control no longer looks executable when unavailable.
- If executable, it is confirmation-based and tested.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` if backend changes are made.

## Notes / Residual Risks
- The safer default is disabled placeholder.
- If the backend can only complete globally, that limitation should remain explicit.
- Keep the visual language consistent with the rest of the cockpit controls.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer a label and state fix over new mutation plumbing.
