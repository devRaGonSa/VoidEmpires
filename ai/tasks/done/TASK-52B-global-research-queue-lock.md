# TASK-52B

---
id: TASK-52B
title: Enforce the civilization-wide research queue lock
status: done
type: full-stack
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 52"
priority: critical
---

## Goal
Make every research catalog action unavailable while any Pending or Active research order exists for the civilization, in both backend read models and frontend controls.

## Context
The Research UI-state endpoint currently derives the open-order flag per technology, so unrelated technologies can appear actionable even though the enqueue service correctly rejects a second order.

## Implementation steps

1. Derive one global open-research boolean from the civilization queue and pass it to every readiness evaluation.
2. Preserve per-technology order metadata only for the active technology presentation.
3. Add a frontend queue-derived defensive gate that prevents and clears stale modal preparation.
4. Add backend and frontend guard regression coverage, including availability recovery after materialization.

## Files to read first

- src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs
- src/VoidEmpires.Infrastructure/Research/ResearchEnqueueReadinessEvaluator.cs
- src/VoidEmpires.Infrastructure/Research/ResearchQueueService.cs
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/utils/enumNormalization.ts
- tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs
- scripts/check-frontend-copy-regressions.ps1
- ai/tasks/done/TASK-52B-global-research-queue-lock.md

## Acceptance criteria

- Any Pending or Active research order makes every technology report CanEnqueue false.
- The active technology remains identifiable as researching; other technologies use the canonical open-slot reason.
- The UI cannot open or retain a second research modal while the queue is open.
- After due materialization, the empty queue restores normal readiness evaluation.
- The enqueue service remains the final authority against direct duplicate requests.

## Constraints

- Construction and Research queues remain independent.
- No schema or migration changes.
- Do not expose technical refresh errors in player-facing UI.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-copy-regressions.ps1

## Commit and push

At the end, stage only intended files and commit with a focused message. Push after the complete Block 52 validation cycle.

## Change Budget

- Prefer modifying fewer than 5 implementation/test files.
- Prefer changes under 200 lines of code.
- Prefer one commit for this task.
