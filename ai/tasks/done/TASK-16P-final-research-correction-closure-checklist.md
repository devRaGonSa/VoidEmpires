# TASK-16P-final-research-correction-closure-checklist

---
id: TASK-16P-final-research-correction-closure-checklist
title: Final Research correction closure checklist
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: medium
---

## Goal
Close the corrective block and ensure all tasks are moved to done with a repeatable final QA checklist.

## Purpose
The previous Research block passed technical validation but failed visual QA. This corrective block needs an explicit closure checklist so the same problem does not slip through again.

## Current Problem
Without a strong closure checklist, the repository can drift back into a state where the build passes but the seeded Research flow is not actually usable in the browser.

## Context
- The AGENTS workflow expects pending tasks to be processed in order, validated, and then moved to done.
- The user will perform final browser QA.
- This task is the block closure and should not reopen scope unless a real blocker remains.

## Files to Inspect First
- `ai/tasks/pending/`
- `ai/tasks/done/`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Add a final closure section to the Research docs or checklist.
2. Include the required validation commands:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
3. Include the visual acceptance checks:
   - `/research` loads;
   - summary shows available `>= 1`;
   - summary shows blocked `>= 1`;
   - an available card has a primary review action;
   - a blocked card cannot mutate;
   - `Requisito pendiente de clasificar` does not appear in the primary text for the seeded known blockers;
   - complete-due is disabled or secondary if unavailable;
   - confirmation appears before enqueue;
   - queue updates after one confirmed enqueue if enabled;
   - diagnostics are collapsed.
4. Make sure the checklist is usable by a human during browser QA.
5. Do not create broad new follow-up tasks unless a real blocker remains.

## UI/UX Requirements
- The closure checklist should be simple to scan during live QA.
- The list should be phrased in the same Spanish cockpit language used in the UI.
- Avoid turning the docs into a long essay; keep them operational.

## Backend/API Requirements
- No backend change is expected from checklist work alone.

## Safety Constraints
- Do not overclaim final tech effects.
- Do not claim production auth or unsupported gameplay.
- Do not hide the fact that complete-due may still be disabled.

## Expected Files to Modify
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md` if the closure note needs a cross-reference

## Acceptance Criteria
- The block can be closed after user visual QA.
- `ai/tasks/pending` is empty except `.gitkeep`.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- The checklist should be precise enough that the next regression is obvious before a commit is made.
- If a new blocker is found during the corrective work, keep it specific and narrowly scoped.
- The intent is closure, not expansion.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer one closure checklist and minimal cross-doc updates.
