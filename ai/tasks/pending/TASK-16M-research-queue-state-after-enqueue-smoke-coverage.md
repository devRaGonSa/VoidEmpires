# TASK-16M-research-queue-state-after-enqueue-smoke-coverage

---
id: TASK-16M-research-queue-state-after-enqueue-smoke-coverage
title: Research queue state after enqueue smoke coverage
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: high
---

## Goal
Add or strengthen automated coverage for the usable Research flow after one enqueue.

## Purpose
Build and tests passed previously, but visual QA found that the intended seeded path was not actually available. We need tests that prove the Research cockpit QA path, not just that the project compiles.

## Current Problem
The repository needs regression coverage for the exact path that was expected to work:

1. apply the minimal-validation seed;
2. open the Research UI state;
3. see at least one available item;
4. enqueue that item if the endpoint exists;
5. verify the queue and status change;
6. verify blocked items remain blocked.

Without this coverage, the same defect can return silently in the next block.

## Context
- The repo already has endpoint tests and seed tests.
- This task should add focused smoke coverage around the minimal-validation Research flow.
- Use existing test patterns and test infrastructure; do not add a new framework.

## Files to Inspect First
- `tests/VoidEmpires.Tests/`
- Research endpoint tests
- `DevelopmentSeedServiceTests`
- Research service tests
- Web application factory or integration test patterns

## Implementation Requirements
1. Add test coverage for:
   - apply minimal-validation seed;
   - get Research UI state;
   - assert available count >= 1;
   - assert blocked count >= 1;
   - enqueue the known available Research item if the endpoint exists;
   - assert success;
   - reload Research UI state;
   - assert queue count increased or the technology status changed accordingly;
   - assert resources are not mutated on read;
   - assert blocked Research remains blocked.
2. If enqueue is intentionally unavailable, assert the UI state explains that clearly.
3. Keep the tests deterministic.
4. Prefer a single focused smoke test or a small, clear test group.

## UI/UX Requirements
- No direct UI changes are required here.
- The coverage should correspond to the visible cockpit behavior and not just backend internals.

## Backend/API Requirements
- Add backend tests where appropriate.
- Do not add a frontend test framework if none exists.
- Use in-memory or test infrastructure; do not rely on real PostgreSQL.

## Safety Constraints
- Do not depend on the real database.
- Do not introduce fake success by loosening the assertion too much.
- Do not create new gameplay rules only for tests.

## Expected Files to Modify
- `tests/VoidEmpires.Tests/` research-related test files
- Possibly one supporting seed or endpoint test if needed

## Acceptance Criteria
- Tests catch the previous `Disponibles: 0` regression.
- `dotnet test --no-build` passes.
- `npm run build --prefix src/VoidEmpires.Frontend` passes if frontend files are touched.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend is touched.

## Notes / Residual Risks
- This is the key anti-regression task for Research v1.
- If the endpoint contract changes, keep the assertions focused on visible state rather than fragile implementation details.
- The test should prove the intended QA path, not just a convenient backend branch.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer one smoke test that covers the exact regression path.
