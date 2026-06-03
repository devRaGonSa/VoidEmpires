# TASK-19P-defenses-error-taxonomy-and-diagnostics

---
id: TASK-19P-defenses-error-taxonomy-and-diagnostics
title: Defenses error taxonomy and diagnostics
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
Normalize Defenses errors into Spanish primary messages with collapsed diagnostics and actionable next-step guidance.

## Purpose
Keep the cockpit consistent with Research and Shipyard by showing user-facing explanations first while preserving technical details for development troubleshooting.

## Current Problem
Defenses state loading or guarded actions may fail for many reasons, including missing context, unavailable data, unmet requirements, unsafe endpoints, or development gating. Raw backend errors would make the cockpit feel brittle and unfinished.

## Context
- Research and Shipyard already map backend failures into tighter cockpit error language.
- Diagnostics are useful in development, but they should not dominate the page.
- This task may need to align both frontend mapping and backend result code naming if current contracts are too vague.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- Defenses presentation helpers
- Defenses API client
- backend result or error code surfaces if added
- diagnostics patterns used by Research and Shipyard

## Implementation Requirements
1. Add a Defenses-specific error mapper that handles at least:
   - invalid civilization id
   - invalid planet id
   - planet not controlled
   - defense data unavailable
   - missing defensive infrastructure
   - insufficient resources
   - missing requirement
   - unsafe endpoint
   - endpoint unavailable outside Development
   - unexpected error
2. Provide Spanish primary messages for each case.
3. Keep technical details available in collapsed diagnostics.
4. Make errors actionable where possible, with guidance such as:
   - `Revisa el contexto`
   - `Abre Construccion`
   - `Aplica cockpit-validation`
   - `Esta accion no esta disponible en esta build`
5. Reuse shared diagnostics patterns where appropriate instead of inventing a separate visual system.
6. If backend result codes are too ambiguous, add only the minimum contract refinement needed and test it.

## UI/UX Requirements
- Error states must be visible without breaking the page layout.
- Diagnostics must remain collapsed by default.
- Spanish copy should be calm, clear, and specific.

## Backend/API Requirements
- Add result codes or error metadata only if needed for stable frontend mapping.
- Any backend refinement must remain Development-safe and tested.

## Safety Constraints
- Do not hide genuine backend rejection.
- No combat logic.
- No mutation behavior added solely to test error handling.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- Defenses API or presentation helper files
- backend DTO or endpoint files only if error metadata needs refinement
- related tests if backend behavior changes

## Acceptance Criteria
- Defenses errors are clear in Spanish.
- Diagnostics are available but secondary.
- Frontend build passes, plus backend validation if the contract changed.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` if backend changes are made
- `dotnet test --no-build` if backend changes are made

## Notes / Residual Risks
- Future combat-related failures should get their own taxonomy later instead of being folded into this readiness-focused set.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task centered on mapping and diagnostics, not broad endpoint redesign.
