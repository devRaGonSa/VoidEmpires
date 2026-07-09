# TASK-49H

---
id: TASK-49H
title: Wire refresh into read endpoints
status: pending
type: backend
team: backend
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Call GameplayRefreshService before normal read UI state returns.

## Context
Planet, construction, research, shipyard, and defenses read flows should refresh backend state automatically.

## Implementation steps

1. Inspect read service entrypoints.
2. Inject gameplay refresh into read services.
3. Add endpoint/read tests where practical.

## Files to read first

- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- src/VoidEmpires.Infrastructure/Research/DevResearchUiStateService.cs
- src/VoidEmpires.Infrastructure/Assets/DevShipyardUiStateService.cs
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs
- src/VoidEmpires.Web/DevEndpoints.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- src/VoidEmpires.Infrastructure/Research/DevResearchUiStateService.cs
- src/VoidEmpires.Infrastructure/Assets/DevShipyardUiStateService.cs
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs
- tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs

## Acceptance criteria

- Reads trigger backend refresh.
- Dev/manual materialize buttons are no longer needed for normal gameplay progress.

## Constraints

- No frontend fake state.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
