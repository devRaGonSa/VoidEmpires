# TASK-7Y

---
id: TASK-7Y
title: Sanitize strategic map details for unknown planets
status: pending
type: bug
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Post Phase 7X - Conservative strategic map reveal hardening"
priority: high
---

## Goal

Ensure the strategic map does not expose planet details for planets that the map visibility service marks as `Unknown`.

## Context

Phase 7W made exploration knowledge reveal systems and planet-specific targets. `MapVisibilityService` hides details for unknown planets in an explored system, but `StrategicMapService` still projects planet visual-state fields directly. A system-level reveal, or a planet-level reveal in a multi-planet system, can therefore expose names/types/sizes for unrevealed planets.

## Implementation steps

1. Read the strategic map projection and visibility DTOs.
2. Mask strategic map planet fields when visibility is missing or `Unknown`.
3. Preserve owned and visible planet behavior.
4. Add or extend smoke/unit coverage for an explored system with an unrevealed planet.
5. Update docs/current-state if response behavior changes.

## Files to read first

- `src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs`
- `src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs`
- `src/VoidEmpires.Application/StrategicMap/GetMapVisibilityResult.cs`
- `tests/VoidEmpires.Tests/StrategicMapServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionLifecycleSmokeTests.cs`
- `docs/dev/strategic-map-api-contract.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs`
- `tests/VoidEmpires.Tests/StrategicMapServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionLifecycleSmokeTests.cs` if needed
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`

## Acceptance criteria

- Strategic map planets marked `Unknown` do not expose name, type, size, colonization status, orbital slot, layout, or visual intensity details.
- Owned and visible planets keep their existing details.
- Exploration-known systems can still appear in the strategic map.
- Tests cover an explored system containing an unrevealed planet.
- Validation passes.

## Constraints

- Do not change mission creation or completion behavior.
- Do not add sensors/scanners, fog-of-war state, diplomacy, espionage, combat, interception, route graph, pathfinding, production endpoints, or UI.
- Keep the change minimal.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected files changed.
4. Commit with a clear message, for example:
   `fix(map): sanitize unknown strategic planet details`
5. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
