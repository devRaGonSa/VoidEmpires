# TASK-50A

---
id: TASK-50A
title: Hotfix gameplay refresh idempotency
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 49 follow-up"
priority: high
---

## Goal
Make normal gameplay refresh idempotent for due construction and research completion, and provide a SQL Server repair script for duplicate legacy data.

## Context
After Block 49, normal read flows trigger gameplay refresh. Concurrent refreshes can process the same due orders more than once, creating duplicate `PlanetBuildings` or attempting duplicate `ResearchProjects`. Existing duplicate `PlanetBuildings` also crash construction UI state when single-row assumptions are used.

## Implementation steps

1. Inspect gameplay refresh, construction completion, research completion, and planet UI state building lookup.
2. Make construction completion safe when a building already exists for `PlanetId + BuildingType`.
3. Make research completion safe when a project already exists for `CivilizationId + ResearchType`.
4. Make planet UI state tolerate legacy duplicate buildings by using the highest level per building type.
5. Add SQL Server diagnostics and repair script for duplicate `PlanetBuildings`.
6. Add regression tests for repeated refresh and legacy duplicate read-model behavior.

## Files to read first

- src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs
- src/VoidEmpires.Infrastructure/Buildings/ConstructionOrderCompletionService.cs
- src/VoidEmpires.Infrastructure/Research/ResearchOrderCompletionService.cs
- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- tests/VoidEmpires.Tests/GameplayRefreshServiceTests.cs
- tests/VoidEmpires.Tests/ResearchOrderCompletionServiceTests.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Buildings/ConstructionOrderCompletionService.cs
- src/VoidEmpires.Infrastructure/Gameplay/GameplayQueueMaterializationService.cs
- src/VoidEmpires.Infrastructure/Research/ResearchOrderCompletionService.cs
- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- tests/VoidEmpires.Tests/GameplayRefreshServiceTests.cs
- tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs
- artifacts/sqlserver/VoidEmpires_Dev_Block49_DedupeBuildingsAndResearchRefresh.sql

## Acceptance criteria

- Repeated gameplay refresh over the same due construction order does not create duplicate `PlanetBuildings`.
- Repeated gameplay refresh over the same due research order does not throw or duplicate `ResearchProjects`.
- Existing duplicate `PlanetBuildings` do not crash construction UI state.
- Existing `ResearchProject` plus a due research order updates level instead of inserting a duplicate.
- The SQL Server repair script identifies duplicate `PlanetBuildings`, preserves highest `Level` and max `Footprint`, deletes duplicate rows safely, and includes diagnostics before and after.
- SQL is not applied automatically.

## Constraints

- Do not change UI layout.
- Do not add combat, movement, market, alliance, or espionage features.
- Do not commit secrets.
- Keep the change minimal.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits.
