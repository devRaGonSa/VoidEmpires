# TASK-20F-ground-army-error-taxonomy-and-diagnostics

---
id: TASK-20F-ground-army-error-taxonomy-and-diagnostics
title: Ground Army error taxonomy and diagnostics
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: medium
---

## Goal
Normalize Ground Army errors into Spanish primary messages with collapsed diagnostics.

## Purpose
Keep backend failures actionable and understandable without leaking raw error payloads into the main cockpit experience.

## Current Problem
Ground readiness and action attempts may fail due to missing context, not controlled planet, missing building, insufficient resources, unavailable troop data, unsafe endpoint wiring, or unavailable features. These must not surface as raw backend errors.

## Context
- Research, Shipyard, and Defenses already use specific error mappings.
- Ground Army should follow the same standard.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- Ground Army presentation helpers
- Ground Army API client
- backend result or error codes if added
- diagnostics patterns already used by Research, Shipyard, and Defenses

## Implementation Requirements
1. Add an error mapper for cases such as:
   - invalid civilization id
   - invalid planet id
   - planet not controlled
   - Ground Army data unavailable
   - missing barracks, academy, or logistics
   - insufficient resources
   - insufficient population or manpower
   - missing prerequisite
   - unsafe endpoint
   - endpoint unavailable outside Development
   - unexpected error
2. Keep primary messages in Spanish.
3. Keep technical details collapsed by default.
4. Make the errors actionable with guidance such as:
   - `Revisa el contexto.`
   - `Abre Construccion.`
   - `Aplica cockpit-validation.`
   - `Esta accion no esta disponible en esta build.`

## UI/UX Requirements
- Error states must be visible and must not break the page layout.
- Diagnostics should stay collapsed by default.

## Backend/API Requirements
- Add backend result codes only if necessary and cover them with tests.

## Safety Constraints
- Do not hide backend rejection.
- No combat.
- No invasion.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- Ground Army API or presentation helper files
- focused backend contract or test files only if new error codes are introduced

## Acceptance Criteria
- Ground Army errors are clear and actionable.
- Frontend build passes.
- Backend tests pass if backend changes are made.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` if backend changes are made

## Notes / Residual Risks
- Future invasion and combat systems will need their own taxonomy later; this task should stay scoped to Ground Army v1 readiness and controlled preparation behavior.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on mapping and diagnostics.
