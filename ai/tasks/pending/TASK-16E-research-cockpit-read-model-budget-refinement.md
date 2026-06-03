# TASK-16E-research-cockpit-read-model-budget-refinement

---
id: TASK-16E-research-cockpit-read-model-budget-refinement
title: Refine Research cockpit read model to fit repository budget
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: low
---

## Goal
Trim or split the current Research UI-state implementation so the read model and tests stay within the repository change budget.

## Purpose
The first Research read-model pass is functional, but the implementation grew larger than the preferred task budget. This follow-up keeps later maintenance small and reviewable.

## Context
- The current Research dev endpoint exists and passes build/tests.
- This follow-up should focus on reducing coupling or moving repeated shaping logic into smaller pieces if needed.
- Do not change Research behavior.

## Files to Inspect First
- `src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`
- `docs/dev/research-cockpit-checklist.md`

## Expected Files to Modify
- `src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`
- `src/VoidEmpires.Web/Program.cs` only if wiring changes are needed

## Acceptance Criteria
- The Research read model remains stable.
- The task size is reduced to a safer boundary.
- Validation still passes.

