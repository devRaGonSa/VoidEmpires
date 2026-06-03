# TASK-13X

---
id: TASK-13X
title: Phase 13X - Planet controlled construction create flow
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Enable a controlled Planet construction enqueue or start flow only if safe backend support already exists or can be added as development-only.

## Context
The repository already contains construction application and infrastructure foundations. This task should inspect whether a safe endpoint already exists and then wire a guarded frontend confirmation flow. If safe backend support is missing, the task may add a development-only endpoint with tests using current repository patterns.

## Implementation steps

1. Inspect existing construction queue or building-construction endpoints and services before adding anything new.
2. If safe support exists, wire a confirmation-first frontend flow for construction enqueue or start.
3. If the endpoint is missing but the service exists, add a development-only endpoint with focused tests that match current conventions.
4. Refresh Planet cockpit data after success and show readable Spanish failure handling without corrupting local UI state.

## Files to read first

- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Application/Buildings/ConstructBuildingRequest.cs`
- `src/VoidEmpires.Application/Buildings/EnqueueConstructionOrderRequest.cs`
- `src/VoidEmpires.Infrastructure/Buildings/`
- `tests/VoidEmpires.Tests/ConstructionOrderCompletionServiceTests.cs`
- guarded fleet mutation UI patterns in the frontend

## Expected files to modify

- relevant construction endpoint files under `src/VoidEmpires.Web`
- construction application or infrastructure services only if endpoint wiring is missing
- focused construction endpoint tests
- Planet frontend action components and API client code

## Acceptance criteria

- Construction create flow is executable only when safe backend support exists.
- The UI requires explicit confirmation and shows planet, building, current or target level, cost, and duration metadata when available.
- After success, Planet cockpit data refreshes.
- Failure copy is readable in Spanish and local state remains coherent.
- If safe support cannot be provided, the action stays clearly disabled and the limitation is documented.

## Constraints

- Keep any new backend route development-only unless an existing stable pattern says otherwise.
- Do not mutate production databases automatically.
- Do not bypass explicit confirmation.
- Keep the feature safe for the current block scope.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms the flow either works with confirmation or stays clearly disabled

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
