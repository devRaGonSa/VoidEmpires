# TASK-18C-development-seed-idempotency-and-no-duplicate-tests

---
id: TASK-18C-development-seed-idempotency-and-no-duplicate-tests
title: Development seed idempotency and no duplicate tests
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: high
---

## Goal
Add robust idempotency and duplicate-prevention tests across development seed profiles.

## Purpose
Protect the core promise of seed profiles: users should be able to reapply them repeatedly during QA without duplicating deterministic rows or corrupting expected cockpit state.

## Current Problem
Richer seed profiles make it easier to accidentally duplicate queue rows, stock, orbital groups, or other dependent records. Since repeated apply is expected during QA, idempotency must be proven with automated tests rather than assumed.

## Context
- `minimal-validation` is already intended to be idempotent.
- The new block introduces richer profiles with more entities and possibly seeded queue rows.
- Deterministic ids are already used in multiple parts of the existing seed graph.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Entity sets or repositories affected by seeded profiles
- Cockpit endpoint or service tests that rely on seeded state

## Implementation Requirements
1. Add tests that apply each supported profile at least twice.
2. Assert no duplicate deterministic entities for categories such as:
   - player profile;
   - civilization;
   - galaxy, system, and planet;
   - ownership;
   - resources or stockpiles;
   - buildings;
   - research orders where seeded;
   - construction orders where seeded;
   - asset production orders where seeded;
   - asset stock;
   - orbital groups;
   - fleet transfers.
3. Use stable deterministic ids for seeded queue or order rows where appropriate.
4. If some state is intentionally preserved rather than reset, document that expectation in tests or docs.
5. Keep tests focused and avoid overly brittle assertions that depend on incidental ordering.
6. Fix seed code only where tests reveal actual idempotency gaps.

## UI/UX Requirements
- None directly.
- The outcome should indirectly improve QA confidence by making repeated profile applies safe.

## Backend/API Requirements
- Strengthen test coverage.
- Adjust seed code if idempotency gaps are found.
- Do not require real PostgreSQL.

## Safety Constraints
- No destructive reset logic.
- No dependence on real external services.
- Do not weaken existing invariants to make tests easier.

## Expected Files to Modify
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs` if idempotency fixes are required
- Related targeted tests only if specific profile behaviors need additional assertions
- `docs/dev/development-seed-profiles.md` if preserved-state behavior must be documented

## Acceptance Criteria
- Reapplying supported seed profiles does not duplicate deterministic QA rows.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- Some seeded rows may need explicit deterministic ids before they can be tested safely.
- Prefer clarity and determinism over trying to seed every possible dynamic scenario.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Keep the work concentrated in tests plus narrow seed fixes.
