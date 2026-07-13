# TASK-52D

---
id: TASK-52D
title: Record queue refresh failures before UI-state reads
status: done
type: backend
team: gameplay
supporting_teams: []
roadmap_item: "Block 52"
priority: high
---

## Goal
Ensure Construction and Research UI-state reads record unsuccessful gameplay refreshes without exposing technical exception details to players.

## Context
The Block 52 audit confirmed due queues are processed before active reads, but both Development endpoints silently swallowed refresh exceptions and ignored unsuccessful refresh results. This follow-up keeps Task 52C within its file budget.

## Implementation steps

1. Log unsuccessful refresh results before planet and research UI-state reads.
2. Log caught refresh exceptions while preserving readable Development responses.
3. Record final Block 52 behavior and validation baseline.

## Files to read first

- src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs
- src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs
- src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs
- ai/current-state.md

## Expected files to modify

- src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs
- src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs
- ai/current-state.md
- ai/tasks/done/TASK-52D-queue-refresh-failure-observability.md

## Acceptance criteria

- Unsuccessful refresh results are logged before active queue reads.
- Refresh exceptions are logged without exposing technical details in normal UI responses.
- Existing due-order materialization tests remain green.

## Constraints

- Do not change queue rules or response contracts.
- Do not add polling or expose exception text to players.

## Validation

- dotnet build --no-restore
- dotnet test --no-build

## Commit and push

Commit this focused follow-up and push the completed Block 52 branch.

## Change Budget

- Prefer fewer than 5 files and under 200 lines.
- Prefer one commit.
