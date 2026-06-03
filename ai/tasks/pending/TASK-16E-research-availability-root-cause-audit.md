# TASK-16E-research-availability-root-cause-audit

---
id: TASK-16E-research-availability-root-cause-audit
title: Research availability root cause audit
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: high
---

## Goal
Find the exact reason why `/research` shows zero available technologies after the `minimal-validation` seed is applied.

## Purpose
The previous Research block reported a usable seeded flow, but the actual visual QA result shows `Disponibles: 0`. Before changing any seed, backend rule, or frontend label, we need to know where the regression comes from.

## Current Problem
The current Research cockpit state shows:

- `Disponibles: 0`
- `Bloqueadas: 8`
- `Cola: 0`
- `En espera de cierre: 0`

All cards are blocked. Some cards show messages like:

- `Recursos insuficientes`
- `Accion bloqueada`
- `No se puede iniciar: Requisito pendiente de clasificar.`

That means the user cannot validate the confirmation, enqueue, queue refresh, or blocked-versus-available contrast that the cockpit is supposed to support.

## Context
- Research already has a backend read model, frontend DTOs, presentation helpers, queue panel, catalog and action states.
- The bug may exist in seed data, research catalog rules, resource affordability, prerequisite evaluation, queue availability, backend UI-state composition, frontend normalization, or the final render mapping.
- The audit must distinguish a real backend bug from a frontend interpretation bug.

## Files to Inspect First
- `src/VoidEmpires.Application/Research/`
- `src/VoidEmpires.Infrastructure/Research/`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Implementation Requirements
1. Trace the full path from the seed to the rendered card state:
   - `minimal-validation` seed data;
   - research catalog;
   - resource stockpile used for affordability;
   - prerequisite evaluation;
   - queue availability;
   - backend Research UI-state DTO;
   - frontend API mapping;
   - frontend view model;
   - rendered status and CTA state.
2. Record whether the root cause is:
   - a missing or wrong seed resource amount;
   - a missing or wrong prerequisite;
   - a queue capacity or queue-state rule;
   - a backend calculation bug;
   - a frontend normalization bug;
   - a stale or misleading presentation helper.
3. Add a concise developer note to `docs/dev/research-cockpit-checklist.md` or a small dedicated research backend note if that is the better fit.
4. Do not hide the problem with hardcoded frontend availability.
5. If the cause is a small, safe fix, include it only with tests and keep the scope narrow.

## UI/UX Requirements
- No visual polish work is required in this task unless the audit discovers a mislabeled status that must be corrected for truthfulness.
- The audit output should keep Spanish player-facing terminology when describing the visible problem.
- Technical terms belong in the diagnostic note, not in the player-facing cockpit.

## Backend/API Requirements
- If backend logic changes, add tests proving the availability count.
- Do not add new production endpoints.
- Do not enable unsafe mutation.
- Do not invent research effects.

## Safety Constraints
- Do not make blocked research available by only changing labels.
- Do not bypass prerequisites.
- Do not introduce hidden queue behavior.
- Do not claim the issue is frontend-only unless the data path proves it.

## Expected Files to Modify
- `docs/dev/research-cockpit-checklist.md` or a narrow research audit note.
- Backend files only if the audit reveals a small real bug.
- Tests only if backend behavior changes.

## Acceptance Criteria
- The reason for `Disponibles: 0` is known and documented.
- The next task can fix the issue without guessing.
- Build and tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched.

## Notes / Residual Risks
- The audit must distinguish among insufficient resources, missing prerequisites, queue capacity, and frontend mapping errors.
- If several factors contribute, record the exact primary blocker and the secondary ones separately.
- Prefer evidence from tests or the live read model over assumptions from task history.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer documentation plus tests over broad refactors.
