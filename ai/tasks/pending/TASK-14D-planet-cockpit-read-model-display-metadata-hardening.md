# TASK-14D

---
id: TASK-14D
title: Phase 14D - Planet cockpit read model display metadata hardening
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Harden the Planet cockpit read model so the UI can render buildings and actions without relying on raw numeric values.

## Context
The Planet cockpit already has a working data foundation, but the read model still needs a safer display layer for building names, categories, action titles, affordability, and unavailable reasons. This task should keep the API compatible while making the UI easier to consume.

## Implementation steps

1. Inspect the current Planet cockpit backend DTOs and frontend types.
2. Normalize stable fields in frontend view models if the backend already provides enough data.
3. If needed, add Development-only display metadata to the read model.
4. Add tests whenever backend DTO or service behavior changes.

## Files to read first

- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs`
- `src/VoidEmpires.Application/Planets/IDevPlanetUiStateService.cs`
- `src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs`
- `tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Application/Planets/IDevPlanetUiStateService.cs`, if the contract needs display metadata
- `src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs`, if the read model changes
- focused tests for the read model or endpoint, if backend behavior changes

## Acceptance criteria

- The UI can render buildings and actions without raw numeric display values.
- Display metadata remains compatible with existing consumers.
- Primary read model does not expose unnecessary implementation details.
- Tests cover any backend DTO or service changes.

## Constraints

- Keep the endpoint development-only.
- Do not break existing API consumers.
- Prefer display metadata over broad contract expansion.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

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
