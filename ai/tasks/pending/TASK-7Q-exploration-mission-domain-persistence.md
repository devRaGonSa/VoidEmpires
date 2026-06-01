# TASK-7Q

---

id: TASK-7Q
title: Add exploration mission domain and persistence foundation
status: pending
type: feature
team: backend
supporting_teams:
  - domain
  - infrastructure
  - application
  - tests
  - docs
roadmap_item: "Phase 7Q - Exploration mission domain and persistence foundation"
priority: high

---

## Goal

Add the first minimal persistent foundation for exploration missions.

This task must create the domain and persistence foundation only. It must not create missions from endpoints yet, complete missions, reveal map visibility, add sensors, add fog-of-war, add espionage, add diplomacy, add combat, add interception, add route graph/pathfinding, or add final UI.

## Context

The project currently has read-only exploration preview metadata derived from map visibility. Unknown nodes can show `exploration.preview` as available, but no mission is created.

Now we need a minimal persistent `ExplorationMission` model so later tasks can create and complete missions safely.

The mission should represent:

- requesting civilization
- target system
- optional target planet
- status
- requested/started timestamp
- due/completion timestamp
- completed timestamp when completed
- optional cancellation/rejection is out of scope unless very cheap and consistent

This should be intentionally small and deterministic.

## Implementation steps

1. Inspect current domain entity patterns for:
   - `OrbitalTransfer`
   - `OrbitalGroup`
   - queue/order entities
   - EF configuration conventions
2. Add domain enum:
   - `ExplorationMissionStatus`
   - likely values: `Planned`, `Completed`, `Cancelled` only if cancellation is cheap; otherwise `Planned`, `Completed`
3. Add domain entity:
   - `ExplorationMission`
   - immutable ids
   - `CivilizationId`
   - `TargetSystemId`
   - optional `TargetPlanetId`
   - `RequestedAtUtc`
   - `DueAtUtc`
   - `CompletedAtUtc`
   - `Status`
4. Add validation rules:
   - ids cannot be empty
   - `RequestedAtUtc` must be UTC
   - `DueAtUtc` must be UTC
   - `DueAtUtc` must be greater than or equal to `RequestedAtUtc`
   - cannot complete twice
5. Add EF persistence mapping:
   - add DbSet
   - configure keys/properties
   - configure indexes useful for civilization/status/due time
6. Add migration only if this repository already uses migrations and task conventions allow it.
   - If migrations exist for persistence changes, create a migration.
   - Do not apply migrations to a real database.
7. Add unit tests for:
   - creating valid mission
   - rejecting invalid ids/timestamps
   - completing mission
   - rejecting double completion
   - EF can persist and read back mission
8. Update `ai/current-state.md` to document Phase 7Q.

## Files to read first

- `src/VoidEmpires.Domain/Fleets/OrbitalTransfer.cs`
- `src/VoidEmpires.Domain/Fleets/OrbitalGroup.cs`
- `src/VoidEmpires.Domain/`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/`
- `tests/VoidEmpires.Tests/*OrbitalTransfer*.cs`
- `tests/VoidEmpires.Tests/*Persistence*.cs`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected, depending on repository conventions:

- `src/VoidEmpires.Domain/Exploration/ExplorationMission.cs`
- `src/VoidEmpires.Domain/Exploration/ExplorationMissionStatus.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/*` if migrations are used
- `tests/VoidEmpires.Tests/ExplorationMissionTests.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionPersistenceTests.cs`
- `ai/current-state.md`

If repository conventions place this under another namespace, follow existing style.

## Acceptance criteria

- `ExplorationMission` domain entity exists.
- Mission status exists.
- EF persistence exists.
- Valid mission can be persisted and loaded.
- Invalid mission creation is rejected.
- Completion rules are covered.
- No mission creation endpoint exists yet.
- No map visibility is revealed.
- No fog-of-war, sensors, espionage, diplomacy, combat, interception, pathfinding, or UI is added.
- `ai/current-state.md` documents Phase 7Q.

## Constraints

- Do not add final UI/frontend code.
- Do not add production endpoints.
- Do not reveal visibility or mutate map visibility.
- Do not add persisted fog-of-war/known-system model.
- Do not add exploration mission creation service in this task.
- Do not add completion worker in this task.
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
   `feat(exploration): add mission persistence foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
