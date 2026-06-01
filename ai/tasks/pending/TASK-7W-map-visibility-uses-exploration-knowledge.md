# TASK-7W

---

id: TASK-7W
title: Integrate exploration knowledge into map visibility
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 7W - Map visibility uses exploration knowledge"
priority: high

---

## Goal

Integrate persisted exploration knowledge into the map visibility read model and strategic map read model.

Completed exploration missions should now be able to make target systems/planets known/visible to the requesting civilization.

This must remain read-only from the visibility service perspective.

## Context

Phase 7U adds exploration knowledge persistence.
Phase 7V records knowledge on mission completion.

Now `MapVisibilityService` should include knowledge-derived visibility:

- ownership still wins
- systems containing owned planets remain visible
- exploration knowledge can make a system visible/known
- planet-level knowledge can make a planet visible/known
- system-level knowledge may reveal system-level metadata but should not necessarily reveal all planet details unless current product decision says so
- no sensors/scanners/espionage/diplomacy/combat are added

Need to choose a conservative rule:

- system knowledge: system becomes `Visible` or `Known`, if a new enum value is added.
- planet knowledge: planet becomes `Visible` or `Known`.
- if adding `Known` would be too invasive, use existing `Visible` and document it as knowledge-derived visibility.
- avoid leaking other-civilization ownership details.

## Implementation steps

1. Inspect current `MapVisibilityLevel` and `MapVisibilityReason`.
2. Decide whether to add:
   - `Known`
   - `ExplorationKnowledge`
   - `ExploredSystem`
   - `ExploredPlanet`
     according to current enum style.
3. Update `MapVisibilityService`:
   - load exploration knowledge for requesting civilization
   - merge with ownership-derived visibility
   - ownership/owned-system visibility remains highest priority
   - knowledge-derived visibility applies only to known systems/planets
   - preserve hidden details where appropriate
4. Update strategic map behavior if needed:
   - strategic map should include systems relevant through owned planets, active transfers, and now exploration knowledge
   - knowledge-derived systems should appear in the strategic map result
5. Update command availability:
   - known/visible nodes allow view commands according to selected rule
   - exploration preview should be blocked for already-known nodes
6. Add tests:
   - system knowledge changes unknown system visibility
   - planet knowledge changes target planet visibility
   - system-level knowledge does not incorrectly mark all planets as owned
   - foreign ownership still not leaked
   - strategic map includes knowledge-derived system
   - exploration preview blocks known/visible explored nodes
   - service remains read-only
7. Update docs/current-state.
8. Update `ai/current-state.md` to document Phase 7W.

## Files to read first

- `src/VoidEmpires.Application/StrategicMap/MapVisibilityLevel.cs`
- `src/VoidEmpires.Application/StrategicMap/MapVisibilityReason.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/MapVisibilityService.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/ExplorationActionPreviewService.cs`
- `tests/VoidEmpires.Tests/MapVisibilityServiceTests.cs`
- `tests/VoidEmpires.Tests/StrategicMapServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationActionPreviewServiceTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected:

- `src/VoidEmpires.Application/StrategicMap/MapVisibilityLevel.cs` if adding `Known`
- `src/VoidEmpires.Application/StrategicMap/MapVisibilityReason.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/MapVisibilityService.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/ExplorationActionPreviewService.cs` if preview behavior depends on new level/reason
- `tests/VoidEmpires.Tests/MapVisibilityServiceTests.cs`
- `tests/VoidEmpires.Tests/StrategicMapServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationActionPreviewServiceTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`

## Acceptance criteria

- Exploration knowledge affects map visibility.
- Knowledge-derived systems can appear in strategic map results.
- Owned visibility still has priority.
- Foreign ownership is not leaked.
- Exploration preview blocks already known/visible explored nodes.
- Visibility services remain read-only.
- No sensors/scanners/espionage/diplomacy/combat/interception/pathfinding/UI are added.
- Tests cover behavior.
- Docs are updated.
- `ai/current-state.md` documents Phase 7W.

## Constraints

- Do not add production endpoints.
- Do not add final UI/frontend code.
- Do not add sensors/scanners.
- Do not add scanner range or sensor strength.
- Do not add espionage, diplomacy, combat, or interception.
- Do not add route graph/pathfinding.
- Do not mutate visibility from read services.
- Avoid over-revealing details.
- Keep behavior deterministic and conservative.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Expected result:

- clean build
- 0 errors
- no new warnings
- all tests passing

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected files changed.
4. Commit with a clear message, for example:
   `feat(map): use exploration knowledge in visibility`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
