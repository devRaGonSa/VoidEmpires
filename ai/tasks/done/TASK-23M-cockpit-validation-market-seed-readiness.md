# TASK-23M

---
id: TASK-23M
title: Phase 23M - Cockpit validation Market seed readiness
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - qa
  - docs
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Ensure `cockpit-validation` supports useful and repeatable Market QA.

## Purpose

The Market cockpit cannot be visually accepted if the standard seeded validation profile does not provide meaningful economy data, ratio references, and disabled future-action context.

## Current problem

`cockpit-validation` currently supports the accepted cockpits, but Market still needs deterministic seed readiness so QA can verify the cockpit without ad hoc SQL or manual data manipulation.

## Context

`cockpit-validation` is already the preferred rich QA and demo profile. Market should join that same deterministic path without creating destructive reset behavior or duplicating seed rows on reapply.

## Files to read first

- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Market read-model tests
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Component discovery

Inspect how current accepted cockpit seed readiness is modeled, tested, and documented. Prefer extending `cockpit-validation` rather than inventing a separate one-off profile unless the current profile contract clearly requires it.

## Dependency analysis

Expected seed flow:

- development seed apply endpoint -> `cockpit-validation`
- deterministic seed rows -> Market read model
- Market UI state or page -> screenshot-friendly QA state

If Market-specific seed additions are required, ensure the read model and tests consume the same deterministic data.

## Implementation requirements

1. Extend `cockpit-validation` or add a Market-specific profile only if current profile conventions require it.
2. Ensure seeded Market context includes:
   - `Aurelia` or equivalent accepted planet context
   - useful system or route context such as `Helios Gate` where applicable
   - visible resource reserves
   - production or economy flow if available
   - reference ratios or prices
   - at least one economy signal
   - disabled future Market actions
3. Keep the seed idempotent.
4. Do not duplicate systems, planets, resources, or synthetic ratio rows.
5. Add or update tests for:
   - Market UI state loads after `cockpit-validation`
   - resource count is non-zero
   - reference ratios or prices exist if implemented
   - future actions are disabled
   - profile reapply does not duplicate state
6. Update seed docs with the expected Market readiness state.

## UI/UX requirements

- Seed output should support screenshot-driven QA and demo use
- Seeded state should feel meaningful, not empty or purely technical

## Backend/API requirements

- Development-only seed changes only
- No migrations
- No production auth work

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- seed-related tests under `tests/VoidEmpires.Tests/`
- `docs/dev/development-seed-profiles.md`
- Market read-model tests if needed

## Safety constraints

- No active transaction data that implies a real player market
- No manual SQL requirements
- No destructive reset
- No duplicate seed rows on reapply

## Acceptance criteria

- `cockpit-validation` supports meaningful Market QA.
- Seed apply remains deterministic and idempotent.
- Tests pass.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Frontend build is required only if frontend docs or typed contract files are touched.

If integration checks are reviewed and no configured integration suite exists, record:

`No integration tests configured.`

## Notes / residual risks

- If backend cannot represent demand or supply cleanly yet, use deterministic reference signals and document the limitation instead of inventing false dynamic economics.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm changed files match the intended seed and docs scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer extending current seed profile patterns over creating a parallel Market seed system.
- If seed changes grow beyond the budget, split remaining QA data refinements into a follow-up task.
