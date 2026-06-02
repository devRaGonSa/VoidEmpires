# TASK-10C

---
id: TASK-10C
title: Phase 10C - Align development seed tests with final visual validation dataset
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Phase 10C - Align development seed tests with final visual validation dataset"
priority: high
---

## Goal

Align the development seed tests with the final deterministic validation dataset so the tests verify the intended seed contract instead of stale exact planet counts.

## Context

The `minimal-validation` development seed profile now supports the validation flow for strategic map, fleet UI state, and planet visual state.

The current tests still assert exactly three planets in the seeded system, but the intended final dataset may contain a richer set of validation rows. The tests should preserve the meaningful contract:

- the seeded system exists
- the three core seed planets exist with deterministic IDs
- the dataset remains idempotent
- the seed supports the strategic map and visual validation scenarios

## Implementation steps

1. Inspect `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`.
2. Inspect the development seed service implementation and the seed documentation in `README.md`.
3. Replace stale exact-count assertions with contract-based assertions that use the deterministic seed IDs and roles.
4. Prefer verifying:
   - at least the three required seed planets exist
   - the owned, industrial, and orbital/visual validation planets have the expected types or roles
   - re-running the seed does not duplicate rows
5. Do not weaken the tests to only check that data exists somewhere.
6. Do not change production startup, migrations, or frontend behavior.

## Files to read first

- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `README.md`

## Expected files to modify

- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`

## Acceptance criteria

- The seed tests verify the intended deterministic contract instead of exact stale counts.
- The idempotency test confirms repeated seed application does not duplicate rows.
- Validation commands pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
