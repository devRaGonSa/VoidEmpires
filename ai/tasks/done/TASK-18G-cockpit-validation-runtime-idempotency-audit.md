# TASK-18G-cockpit-validation-runtime-idempotency-audit

---
id: TASK-18G-cockpit-validation-runtime-idempotency-audit
title: Cockpit validation runtime idempotency audit
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 18G-18R - Cockpit validation seed idempotency runtime hardening"
priority: high
---

## Goal
Audit why `cockpit-validation` passes automated tests but fails against a reused development PostgreSQL database.

## Purpose
Identify the exact mismatch between seed existence checks and the real database uniqueness rules before changing seed behavior.

## Current Problem
Applying `cockpit-validation` on a reused local development database throws `Npgsql.PostgresException 23505` on `IX_ResearchOrders_CivilizationId_Sequence`. The current seed likely detects an existing logical row using business fields such as `ResearchType`, `TargetLevel`, and `Status`, but the database uniqueness contract is based on `Sequence` per owner. That makes the seed pass in-memory tests while failing against real persisted data after prior QA mutations.

## Context
- `main` is currently ahead of `origin/main` with seed-profile work but has not been pushed.
- Automated validation is green.
- Runtime failure happens when a reused local PostgreSQL database already contains queue rows from manual QA or prior endpoint usage.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/ResearchOrderConfiguration.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/PlanetConstructionOrderConfiguration.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/AssetProductionOrderConfiguration.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`

## Implementation Requirements
1. Identify all seeded queue or order entities that use `Sequence` plus owner uniqueness.
2. Document which seed existence checks currently do not align with the actual unique indexes.
3. Confirm why in-memory tests did not catch the runtime collision.
4. Add a short note to `docs/dev/development-seed-profiles.md` explaining the runtime idempotency expectation over reused development databases.
5. Do not change runtime behavior yet unless a tiny documentation-accuracy fix is required.

## UI/UX Requirements
- None directly, but the audit output should help developers understand why a 500 occurred during local QA.

## Backend/API Requirements
- Prefer audit, docs, and tests only.
- Keep seed behavior unchanged in this task unless a trivial accuracy correction is required.

## Safety Constraints
- No manual SQL.
- No destructive reset behavior.
- No deletion of existing QA rows.

## Expected Files to Modify
- `docs/dev/development-seed-profiles.md`
- Audit-related tests only if necessary to document the mismatch

## Acceptance Criteria
- The root cause of the runtime collision is clearly documented.
- The order entities affected by `Sequence` uniqueness are identified.
- Follow-up fix tasks have a concrete technical baseline.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- The runtime failure may not be isolated to ResearchOrders; construction and asset production must be checked too.
- Avoid fixing behavior here unless it is truly trivial.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep this task diagnostic-first.
