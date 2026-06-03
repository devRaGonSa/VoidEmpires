# TASK-16X-research-complete-due-visual-regression-guard

---
id: TASK-16X-research-complete-due-visual-regression-guard
title: Research complete-due visual regression guard
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: medium
---

## Goal
Ensure complete-due remains visually truthful and non-mutating when unavailable.

## Purpose
The complete-due control was previously misleading in the Research cockpit. Even though it now appears more truthful, this block should protect that behavior while the enqueue path is being aligned.

## Current Problem
The complete-due action should not look like a primary executable button if it is unavailable. That behavior must remain stable while we change adjacent Research code.

## Context
- The control should remain disabled or secondary unless a safe, tested cockpit-scoped complete-due path exists.
- The Research page already has a disabled placeholder pattern for this case.
- This task is a regression guard, not a feature expansion.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/research-cockpit-checklist.md`

## Implementation Requirements
1. Verify the current complete-due control state.
2. Ensure unavailable complete-due:
   - is not styled as a primary blue CTA;
   - has no mutation handler;
   - uses a label such as `Completar vencidas no disponible`;
   - keeps an explanation visible.
3. If due count is zero, the control should not look urgent or executable.
4. If a future safe endpoint exists, it must require confirmation, but do not add that here unless already supported and tested.

## UI/UX Requirements
- Spanish-first.
- Disabled or placeholder state must be obvious.
- The control should visually read as unavailable, not as an active task.

## Backend/API Requirements
- No backend change is expected.

## Safety Constraints
- No global unsafe complete-due execution.
- No background worker controls.
- No hidden mutation from the disabled control.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Complete-due remains visually truthful after enqueue fixes.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Completion can be a future task after enqueue is stable.
- If the backend ever exposes a safe cockpit-scoped completion path, that must be handled explicitly in a separate task.
- Keep the visual language consistent with other disabled actions in the cockpit.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer a label/state guard over new mutation plumbing.
