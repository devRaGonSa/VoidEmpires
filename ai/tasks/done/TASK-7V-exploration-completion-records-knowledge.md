# TASK-7V

---

id: TASK-7V
title: Record exploration knowledge on mission completion
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 7V - Exploration completion records knowledge"
priority: high

---

## Goal

Update exploration mission completion so completed due missions persist exploration knowledge for their target system and optional target planet.

This task should record knowledge only. It should not yet integrate that knowledge into `MapVisibilityService` or `StrategicMapService`.

## Context

Phase 7U adds `ExplorationKnowledge`.
Phase 7S already completes due exploration missions without visibility reveal.

Now completing a mission should create the durable knowledge rows that a later task will use for map visibility.

Rules:

- system-target mission creates system-level knowledge
- planet-target mission creates system-level knowledge and planet-level knowledge
- repeated completion should be idempotent
- duplicate knowledge rows should not be created
- knowledge source should be `MissionCompletion`
- source mission id should be set
- no resources/fleets/rewards are modified

## Implementation steps

1. Read `ExplorationMissionCompletionService`.
2. Read `ExplorationKnowledge` from Phase 7U.
3. Add or update infrastructure logic:
   - when completing a due mission, create knowledge for the mission target
   - avoid duplicate knowledge rows
   - save mission completion and knowledge in the same persistence operation
4. Preserve existing completion behavior:
   - future missions are not completed
   - completed missions are not completed twice
   - non-UTC request is rejected
5. Add tests:
   - due system mission records system knowledge
   - due planet mission records system and planet knowledge
   - duplicate knowledge is not created on repeated completion calls
   - future mission records no knowledge
   - completion still does not modify resources or fleets
6. Do not update `MapVisibilityService` yet.
7. Update docs/current-state only to state that knowledge is recorded but not consumed by visibility until Phase 7W.
8. Update `ai/current-state.md` to document Phase 7V.

## Files to read first

- `src/VoidEmpires.Domain/Exploration/`
- `src/VoidEmpires.Application/StrategicMap/CompleteDueExplorationMissions*.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/ExplorationMissionCompletionService.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionCompletionServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionLifecycleSmokeTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected:

- `src/VoidEmpires.Infrastructure/StrategicMap/ExplorationMissionCompletionService.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionCompletionServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionLifecycleSmokeTests.cs` if needed
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`

## Acceptance criteria

- Completing a due system-level mission records system knowledge.
- Completing a due planet-level mission records system and planet knowledge.
- Completion is idempotent with respect to knowledge creation.
- Future missions do not create knowledge.
- Visibility does not consume knowledge yet in this task.
- No resources/fleets/rewards are mutated.
- Tests cover behavior.
- Docs/current-state mention knowledge is recorded but visibility integration comes later.
- `ai/current-state.md` documents Phase 7V.

## Constraints

- Do not update `MapVisibilityService` in this task.
- Do not reveal visibility yet.
- Do not add production endpoints.
- Do not add sensors/scanners.
- Do not add rewards.
- Do not mutate resources/fleets.
- Do not add espionage, diplomacy, combat, or interception.
- Do not add route graph/pathfinding.
- Keep completion deterministic and idempotent.

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
   `feat(exploration): record knowledge on mission completion`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
