# TASK-17A-final-research-enqueue-closure-checklist

---
id: TASK-17A-final-research-enqueue-closure-checklist
title: Final Research enqueue closure checklist
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: medium
---

## Goal
Close the block with explicit acceptance criteria and task workflow cleanup.

## Purpose
Research has required multiple correction passes. The closure task must make the acceptance boundary unambiguous so the same class of failure does not slip through again.

## Current Problem
Without a strong closure checklist, the repository can drift back into a state where the build passes but the seeded Research flow is not actually usable in the browser.

## Context
- The AGENTS workflow expects pending tasks to be processed in order, validated, and then moved to done.
- The user will perform final browser QA.
- This task should close the block only after the enqueue flow is actually working.

## Files to Inspect First
- `ai/tasks/pending/`
- `ai/tasks/done/`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Add final closure criteria to the Research docs or checklist.
2. Include the required validation commands:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
3. Include the visual and functional acceptance checks:
   - `/research` loads;
   - summary shows `Disponible >= 1`;
   - available item opens confirmation;
   - confirmation submit succeeds;
   - queue updates after success;
   - blocked item cannot mutate;
   - complete-due is disabled or secondary if unavailable;
   - diagnostics are collapsed;
   - no 3D/WebGL;
   - no combat/interception/espionage.
4. Make sure the checklist is usable by a human during browser QA.
5. Do not create broad new follow-up tasks unless a real blocker remains.

## UI/UX Requirements
- The closure checklist should be simple to scan during live QA.
- Use the same Spanish cockpit language as the UI.
- Keep the checklist operational, not essay-like.

## Backend/API Requirements
- No backend change is expected from checklist work alone.

## Safety Constraints
- Do not overclaim production readiness.
- Do not claim the block is closed until the enqueue path truly works.
- Do not hide the fact that complete-due may still be disabled.

## Expected Files to Modify
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md` if the closure note needs a cross-reference

## Acceptance Criteria
- The block can be closed after user validation.
- `ai/tasks/pending` is empty except `.gitkeep`.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If the enqueue still fails, do not close the block.
- Keep the closure criteria precise enough that the next regression is obvious.
- The intent is closure, not expansion.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer one closure checklist and minimal cross-doc updates.
