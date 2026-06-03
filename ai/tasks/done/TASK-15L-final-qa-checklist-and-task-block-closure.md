# TASK-15L

---
id: TASK-15L
title: Final QA checklist and task block closure
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Close the block with validation documentation and ensure the task queue can be completed cleanly.

## Current problem
The previous block was technically valid but visually not closed. This task exists to make sure the module boundaries are verifiable and the pending queue can be emptied safely.

## Context from current implementation
The AGENTS workflow expects pending tasks to be processed in order and moved to done. The user validates locally, so the checklist must be usable without relying on hidden context.

## Files to inspect first
- `ai/tasks/pending/`
- `ai/tasks/done/`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-module-boundaries.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `ai/current-state.md`

## Expected files to modify
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-module-boundaries.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `ai/current-state.md`

## Implementation requirements
- Add a final block checklist section to the docs.
- The checklist must include:
  - build;
  - tests;
  - frontend build;
  - pending empty except `.gitkeep`;
  - `/planet` is dashboard only;
  - `/construction` is scoped to general construction;
  - `/research` opens module placeholder;
  - `/ground-army` opens module placeholder;
  - `/shipyard` opens module placeholder;
  - `/defenses` opens module placeholder;
  - Galaxy remains read-only;
  - Fleets not regressed;
  - no 3D/WebGL;
  - diagnostics collapsed.
- Ensure task files are moved to `ai/tasks/done` when implemented by the AI platform workflow.
- Do not create follow-up tasks unless a real blocker remains.
- If follow-up tasks are needed, create no more than three and keep them specific.

## UI/UX requirements
- QA docs must be actionable by the user with screenshots.

## Backend/API requirements
- No backend change expected.

## Safety constraints
- Do not overclaim specialized gameplay support.

## Acceptance criteria
- The block can be locally validated.
- Pending queue is empty except `.gitkeep`.
- Documentation reflects known residual risks.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- If the queue cannot be closed because of a real blocker, note it explicitly rather than broadening scope.
