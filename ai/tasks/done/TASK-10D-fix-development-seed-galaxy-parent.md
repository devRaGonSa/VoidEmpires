# Phase 10D - Fix development seed galaxy parent FK

## Summary

Fix the development `minimal-validation` seed so the seeded `SolarSystem` always has an existing parent `Galaxy` row before persistence, preventing PostgreSQL foreign key violations in `/api/dev/seeds/apply`.

## Scope

- Inspect:
  - `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
  - galaxy/solar system domain entities and EF mappings
  - existing seed constants/IDs
  - `DevelopmentSeedServiceTests`
- Update the `minimal-validation` seed so it idempotently creates or ensures the parent `Galaxy` before creating the seeded `SolarSystem`.
- Use deterministic seed IDs/constants.
- Preserve idempotency:
  - Running the seed multiple times must not duplicate the galaxy, solar system, planets, ownerships, buildings, orbital groups, transfers, or stockpiles.
- Keep the seed explicit/dev-only.
- Do not apply the seed to any database automatically.
- Do not apply EF migrations.
- Do not change production startup behavior.
- Do not touch frontend unless docs need a tiny correction.
- Add or update tests so this FK issue is caught:
  - Prefer a test that verifies the seeded `SolarSystem` has an existing parent `Galaxy`.
  - If feasible with existing test infrastructure, add relational-provider coverage or at least assert the parent entity is present in the same seed dataset.
  - Keep existing strict checks for deterministic planets and orbital groups.
- If documentation lists seed validation flow, update it only if necessary.

## Files to Read First

- `ai/architecture-index.md`
- `ai/orchestrator/component-discovery.md`
- `ai/orchestrator/di-analysis.md`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/Infrastructure/Development/DevelopmentSeedServiceTests.cs`

## Expected Files to Modify

- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/Infrastructure/Development/DevelopmentSeedServiceTests.cs`
- `ai/tasks/in-progress/TASK-10D-fix-development-seed-galaxy-parent.md`

## Validation

Run:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

Expected:

- `dotnet build` passes.
- `dotnet test` passes, expected `544/544` or more if a new test is added.
- Frontend build passes.

## Notes

- Manual validation against `VoidEmpireDB_Dev` remains explicit and will be performed separately after implementation.
- Do not touch PostgreSQL directly.
- Do not add gameplay mutations.
- Preserve explicit opt-in development seed behavior.
