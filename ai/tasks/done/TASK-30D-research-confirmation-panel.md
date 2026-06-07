# TASK-30D

---
id: TASK-30D-research-confirmation-panel
title: Add explicit research confirmation panel before enqueue
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Require explicit user confirmation before calling real research enqueue.

## Context
Selection is not enough; users need explicit, clear consent language before DB-enqueue mutation.

## Implementation steps

1. Add a confirmation panel/section in `ResearchPage.tsx` that is bound to current selection.
2. Include mandatory Spanish copy:
   - `Esta acción creará una orden real de investigación en la base de datos de Development.`
   - `Los recursos se descontarán cuando el backend confirme la orden.`
   - `No se completará automáticamente desde esta cabina.`
3. Add buttons:
   - `Confirmar investigación`
   - `Cancelar`
4. Disable confirm button unless candidate is available and selected.
5. Keep no-mutation behavior in this task; confirm click only changes local UI state for now.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- Confirmation UI exists with required copy.
- Confirmation is explicit and cannot be triggered without eligible selection.
- No accidental mutation from just selecting or opening the panel.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- No actual enqueue POST in this task.
- Keep copy Spanish-first.
- No raw ids in primary interface.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `feat(frontend): add research confirmation panel`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
