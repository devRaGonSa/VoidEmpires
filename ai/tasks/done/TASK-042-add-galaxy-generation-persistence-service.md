# TASK-042

---
id: TASK-042
title: Add galaxy generation persistence service
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2C.2 - Persisted galaxy generation foundation"
priority: high
---

## Goal
Implement a service that generates a galaxy using `IGalaxyGenerator` and persists it through `VoidEmpiresDbContext`.

## Context
The generator creates a complete galaxy aggregate in memory. The application needs a service capable of saving generated galaxies to the database, but tests must remain network-free and must not use the real NAS PostgreSQL database.

## Implementation steps

1. Modify `src/VoidEmpires.Infrastructure`.
2. Modify tests.
3. Do not add public endpoints yet.
4. Do not apply migrations to a real database.
5. Do not add player or civilisation ownership.
6. Do not add economy, fleets, combat, alliances, espionage, construction, or research.
7. Implement `IGalaxyGenerationService` in Infrastructure, for example with a `GalaxyGenerationService` class.
8. Use `IGalaxyGenerator` and `VoidEmpiresDbContext` as constructor dependencies.
9. Generate the galaxy in memory, persist the aggregate, and call `SaveChangesAsync`.
10. Return `GenerateAndPersistGalaxyResult`.
11. If a galaxy with the same name already exists, return a deterministic failure unless an explicit overwrite flag exists.
12. Do not silently overwrite existing galaxies.
13. Handle validation or generator exceptions deterministically without exposing sensitive data.
14. Register `IGalaxyGenerationService` in DI.
15. Keep tests database-safe, using SQLite in-memory or another safe relational test approach if needed.
16. Do not use real PostgreSQL.

## Files to read first

- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Application/*`
- `tests/VoidEmpires.Tests/*`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- Generated galaxies are persisted through the service.
- Duplicate galaxy names return deterministic failure.
- The service uses the existing generator and DbContext.
- Tests validate galaxy count, solar system count, star count, planet limits, and duplicate name handling.
- Tests do not require real PostgreSQL.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- All tests pass.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(galaxy): persist generated galaxies`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
