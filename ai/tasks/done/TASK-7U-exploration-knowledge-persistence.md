# TASK-7U

---

id: TASK-7U
title: Add exploration knowledge persistence foundation
status: pending
type: feature
team: backend
supporting_teams:
  - domain
  - infrastructure
  - application
  - tests
  - docs
roadmap_item: "Phase 7U - Exploration knowledge persistence foundation"
priority: high

---

## Goal

Add a minimal persistent foundation for exploration-derived map knowledge.

This task must only add the persistence/domain foundation for known systems/planets. It must not integrate the knowledge into visibility yet, complete missions, create missions, add sensors, add scanner mechanics, add espionage, add diplomacy, add combat, add interception, add route graph/pathfinding, or add final UI.

## Context

The project currently has:

- derived map visibility from planet ownership
- exploration previews
- persistent exploration missions
- mission creation
- mission completion
- lifecycle smoke coverage proving completion does not reveal visibility yet

Now we need a minimal persisted knowledge model so completed exploration missions can reveal targets in a later task.

The model should be intentionally small:

- civilization id
- known system id
- optional known planet id
- discovery source
- discovered timestamp
- optional source mission id
- deterministic uniqueness constraints

## Implementation steps

1. Inspect current domain entity patterns:
   - `ExplorationMission`
   - `OrbitalTransfer`
   - `PlanetOwnership`
2. Add domain enum:
   - `ExplorationKnowledgeSource`
   - values such as `MissionCompletion`, `Seeded`, `ManualDev`
3. Add domain entity:
   - `ExplorationKnowledge`
   - `Id`
   - `CivilizationId`
   - `SystemId`
   - optional `PlanetId`
   - `Source`
   - optional `SourceMissionId`
   - `DiscoveredAtUtc`
4. Add validation:
   - ids cannot be empty
   - `DiscoveredAtUtc` must be UTC
   - `PlanetId` may be null for system-level knowledge
5. Add EF persistence:
   - DbSet
   - entity configuration
   - indexes for civilization/system/planet
   - uniqueness to avoid duplicate knowledge rows for the same civilization/system/planet combination if feasible with current database/provider conventions
6. Add migration if repository migration conventions require it.
   - Do not apply migrations to a real database.
7. Add tests:
   - valid system-level knowledge can be created
   - valid planet-level knowledge can be created
   - invalid ids/timestamps are rejected
   - EF can persist/load knowledge
   - duplicate prevention is covered either at service level later or database/index level now, depending on current conventions
8. Update `ai/current-state.md` to document Phase 7U.

## Files to read first

- `src/VoidEmpires.Domain/Exploration/`
- `src/VoidEmpires.Domain/Colonization/PlanetOwnership.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/`
- `tests/VoidEmpires.Tests/ExplorationMissionTests.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionPersistenceTests.cs`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected, depending on conventions:

- `src/VoidEmpires.Domain/Exploration/ExplorationKnowledge.cs`
- `src/VoidEmpires.Domain/Exploration/ExplorationKnowledgeSource.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/*` if migrations are used
- `tests/VoidEmpires.Tests/ExplorationKnowledgeTests.cs`
- `tests/VoidEmpires.Tests/ExplorationKnowledgePersistenceTests.cs`
- `ai/current-state.md`

## Acceptance criteria

- `ExplorationKnowledge` domain/persistence foundation exists.
- System-level and planet-level knowledge are supported.
- Knowledge is civilization-scoped.
- Invalid creation is rejected.
- EF persistence works.
- No visibility behavior changes yet.
- No mission completion behavior changes yet.
- No sensors/scanners/espionage/diplomacy/combat/interception/pathfinding/UI are added.
- `ai/current-state.md` documents Phase 7U.

## Constraints

- Do not integrate knowledge into `MapVisibilityService` in this task.
- Do not modify mission completion behavior in this task.
- Do not add production endpoints.
- Do not add final UI/frontend code.
- Do not add sensors/scanners.
- Do not add espionage, diplomacy, combat, or interception.
- Do not add route graph/pathfinding.
- Do not apply migrations to a real database.
- Keep the model minimal.

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
   `feat(exploration): add knowledge persistence foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
