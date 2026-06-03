# TASK-16G-research-requirement-classification-fix

---
id: TASK-16G-research-requirement-classification-fix
title: Research requirement classification fix
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: high
---

## Goal
Fix or clarify the `Requisito pendiente de clasificar` message so requirement states are meaningful and actionable.

## Purpose
The Research cockpit must explain why a technology is blocked. A generic unclassified requirement message in the primary UI is not acceptable for the first usable flow.

## Current Problem
Some cards show:

- `No se puede iniciar: Requisito pendiente de clasificar.`

That suggests either:

- the backend emits a generic or unknown requirement reason;
- the frontend does not understand a backend requirement type;
- or the mapper collapses several distinct blockers into one placeholder.

In any case, the primary UI needs more precise and user-friendly copy.

## Context
- Known blockers should have distinct Spanish explanations.
- `Requisito pendiente de clasificar` should only appear in diagnostics, if at all.
- The cockpit should distinguish resource blockers, prerequisite blockers, queue blockers, completion blockers, and build-scope blockers.

## Files to Inspect First
- `src/VoidEmpires.Application/Research/`
- `src/VoidEmpires.Infrastructure/Research/`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `tests/VoidEmpires.Tests/`

## Implementation Requirements
1. Identify all requirement and reason codes emitted by the backend Research UI state.
2. Map each known code to a Spanish player-facing label.
3. If the backend emits unknown reason types, classify them more precisely where possible.
4. If a requirement is truly unknown, show:
   - primary: `Requisito no disponible en esta build.`
   - diagnostics: raw code or details.
5. Ensure `Requisito pendiente de clasificar` is not used as the primary reason for known cases.
6. Add backend tests if the reason code mapping changes there.
7. Add frontend coverage if there is an existing frontend test path; otherwise rely on `npm` build plus manual QA.

## UI/UX Requirements
- Requirement chips should be clear, concise, and visually readable.
- Blocked cards should explain the blocker without exposing developer-only phrasing in the main UI.
- The available/blocked distinction should remain obvious after the fix.

## Backend/API Requirements
- If reason codes are added or changed, keep them backward-compatible where possible.
- Do not change core gameplay rules only to improve copy.
- Do not hide real backend rejection reasons.

## Safety Constraints
- Do not make blocked research available by relabeling it.
- Do not bypass prerequisites or resource checks.
- Do not invent tech effects.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- Backend tests or mapping code only if the backend reason vocabulary changes

## Acceptance Criteria
- Known blockers have meaningful Spanish explanations.
- `Requisito pendiente de clasificar` no longer appears as the primary text for the seeded QA path.
- Build, tests, and frontend build pass.

## Validation
- `dotnet build --no-restore` if backend changes are made.
- `dotnet test --no-build` if backend changes are made.
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Future research requirement types must be added to the mapper when introduced.
- If the backend can provide richer typed reasons, prefer that over more string parsing in the frontend.
- Keep the fallback message honest and clearly marked as a build limitation.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer targeted mapping fixes over broad UI refactors.
