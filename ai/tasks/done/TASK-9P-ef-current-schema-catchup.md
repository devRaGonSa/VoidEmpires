# TASK-9P

---
id: TASK-9P
title: Phase 9P - EF migration catch-up for current gameplay schema
status: pending
type: backend
team: backend
supporting_teams:
  - architecture
  - qa
roadmap_item: "Phase 9P - EF migration catch-up for current gameplay schema"
priority: high
---

## Goal

Add EF Core migration coverage so PostgreSQL schema can be brought up to date with the current `VoidEmpiresDbContext` model, including current gameplay and readiness entities such as orbital groups, fleet state, and alliance pacts.

## Context

Current state:

- `main` includes Phase `9O` and `9O-B` fixes.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes with `528/528` tests.
- PostgreSQL is running.
- A new development database was created manually: `VoidEmpireDB_Dev`.
- Existing migrations were applied manually to `VoidEmpireDB_Dev` using `dotnet ef database update` with an explicit `--connection` string.
- Applied migrations currently go only up to `20260527185144_AddResearchModel`.

Current runtime problem:

- `GET /api/dev/strategic-map?civilizationId=00000000-0000-0000-0000-000000000001` fails with:
  - `PostgresException: 42P01: relation "AlliancePacts" does not exist`
- `GET /api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001` fails with:
  - `PostgresException: 42P01: relation "OrbitalGroup" does not exist`

Diagnosis:

- The current EF Core model contains entities and tables added after the last existing migration.
- The repository does not yet include migration coverage for the current gameplay schema.
- The database itself is not the root cause; migrations are incomplete relative to the current `DbContext` model.

Important design-time note:

- The design-time `DbContext` factory appears to default to localhost and `voidempires_design`, forcing manual EF commands to use explicit `--connection`.
- Inspect `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs`.
- If a small and safe fix is possible, update the factory to honor configuration or environment connection strings before falling back to a local design-time default.
- If that update would expand scope or risk, defer it to a follow-up task and document that explicitly in the task result.

## Implementation steps

1. Inspect:
   - `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
   - `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs`
   - `src/VoidEmpires.Infrastructure/Persistence/Configurations/*`
   - `src/VoidEmpires.Infrastructure/Persistence/Migrations/*`
   - current model snapshot
2. Generate a new EF Core migration with a clear name, for example:
   - `AddCurrentGameplaySchemaCatchup`
3. Ensure migration coverage includes missing tables, columns, indexes, and relationships required by current backend code, including but not limited to:
   - orbital group and fleet-related current persistence entities
   - `AlliancePacts`
   - alliance readiness and pact readiness entities if persisted
   - diplomatic contacts if persisted
   - exploration missions and exploration knowledge if persisted
   - sensors, detection, and interception persistence if present in the `DbContext`
   - strategic map and readiness persistence if present in the `DbContext`
4. Keep migration output deterministic, incremental, and reviewable.
5. Do not manually create ad-hoc SQL outside EF migration unless absolutely necessary.
6. Do not apply `database update` to any NAS, remote, or real database.
7. Run validation:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

8. Move this task from `ai/tasks/pending` to `ai/tasks/done` only after validation passes.
9. Leave unrelated untracked files untouched, especially `xuniverse_planet_generator_variety.py`.

## Files to read first

- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/VoidEmpiresDbContextModelSnapshot.cs`
- `AGENTS.md`

## Expected files to modify

Expected:

- one new migration file under `src/VoidEmpires.Infrastructure/Persistence/Migrations/`
- one new migration designer file under `src/VoidEmpires.Infrastructure/Persistence/Migrations/`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/VoidEmpiresDbContextModelSnapshot.cs`

May also modify:

- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs` only if the connection-string precedence fix is small and safe
- backend tests only if needed for deterministic migration coverage

## Acceptance criteria

- A new migration exists that catches up schema from `20260527185144_AddResearchModel` to the current `VoidEmpiresDbContext` model.
- Migration includes missing objects required by current backend reads, including `AlliancePacts` and orbital-group-related tables when present in model.
- Migration follows existing naming and style conventions.
- No migration is applied automatically to real databases.
- No secrets or real connection strings are committed.
- Frontend is untouched.
- Gameplay behavior is unchanged.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes (`528/528` expected unless test count legitimately changes).

## Constraints

- Do not commit secrets.
- Do not hardcode local IPs, database passwords, or connection strings.
- Do not apply EF migrations automatically.
- Do not alter production gameplay behavior.
- Do not touch frontend.
- Do not add CORS in this task.
- Do not add auth productization.
- Do not add WebSockets, Three.js/WebGL, or UI changes.
- Keep changes incremental and reviewable.
- Preserve the current successful test baseline.

## Validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

Expected:

- build passes
- all tests pass
- no new warnings introduced

## Manual validation note

After migration is committed and local tests pass, manually apply it only to the development database `VoidEmpireDB_Dev` using an explicit `--connection` string, then rerun:

- `/health`
- `/api/dev/strategic-map`
- `/api/dev/fleets/ui-state`

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only intended migration, snapshot, and optional narrow backend files changed.
4. Commit with a clear message.
5. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when feasible for hand-authored edits.
- Prefer a single small commit for this task.
- If the work expands beyond migration catch-up and safe design-time factory handling, split follow-up work into a new task.
