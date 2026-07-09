# TASK-49G

---
id: TASK-49G
title: Due research completion normal flow
status: pending
type: backend
team: backend
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Complete due research orders during normal refresh/read flow.

## Context
Due research should update technology level/state without manual dev buttons.

## Implementation steps

1. Inspect research completion service.
2. Ensure gameplay refresh calls it safely.
3. Add tests for completed project/order and catalog unlock.

## Files to read first

- src/VoidEmpires.Infrastructure/Research/ResearchOrderCompletionService.cs
- src/VoidEmpires.Infrastructure/Research/ResearchQueueService.cs
- src/VoidEmpires.Infrastructure/Research/DevResearchUiStateService.cs
- tests/VoidEmpires.Tests/ResearchOrderCompletionServiceTests.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs
- tests/VoidEmpires.Tests/GameplayRefreshServiceTests.cs

## Acceptance criteria

- Due research orders complete during refresh.
- Research catalog unlocks correctly after read refresh.

## Constraints

- No manual/dev-only trigger required.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
