# TASK-30R

---
id: TASK-30R
title: Research QA state preparation endpoint and tests
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 30Q-30T - Research manual QA state preparation"
priority: medium
---

## Goal
Add a Development-only endpoint and service that prepare the research manual QA state safely for the seeded civilization and planet, with automated coverage.

## Context
The `/research` cockpit already uses a persisted Development enqueue flow, but repeated manual QA is blocked when the Development database already contains an open research order. The preparation path must stay Development-only, scoped, idempotent, and consistent with the documented contract from `30Q`.

## Implementation steps

1. Add application and infrastructure contracts for research QA state preparation.
2. Add the Development-only endpoint and register the service using existing conventions.
3. Add endpoint and persistence tests for availability, scope, idempotency, and enqueue readiness after preparation.

## Files to read first

- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/Development/IDevelopmentSeedService.cs
- src/VoidEmpires.Infrastructure/Development/ConstructionQaStatePreparationService.cs
- src/VoidEmpires.Infrastructure/Research/ResearchQueueService.cs
- tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevResearchPersistedFlowTests.cs

## Expected files to modify

- src/VoidEmpires.Application/Development/IDevelopmentSeedService.cs
- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Infrastructure/Development/ResearchQaStatePreparationService.cs
- src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
- tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevResearchPersistedFlowTests.cs

## Acceptance criteria

- `POST /api/dev/research/qa-state/prepare` exists only in the Development route set.
- The preparation path clears only blocking open research orders for the targeted civilization and preserves unrelated state.
- Repeated preparation is safe and idempotent.
- Tests prove scope, readiness, and non-impact on unrelated civilizations or cockpits.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- no new warnings or obvious regressions are introduced

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
