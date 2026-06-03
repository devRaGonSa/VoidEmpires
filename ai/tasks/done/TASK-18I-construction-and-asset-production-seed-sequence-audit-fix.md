# TASK-18I-construction-and-asset-production-seed-sequence-audit-fix

---
id: TASK-18I-construction-and-asset-production-seed-sequence-audit-fix
title: Construction and asset production seed sequence audit fix
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 18G-18R - Cockpit validation seed idempotency runtime hardening"
priority: high
---

## Goal
Prevent the same runtime idempotency bug from affecting seeded construction and asset production orders.

## Purpose
Harden all queue-like seed rows that are keyed by owner plus sequence, not only research history.

## Current Problem
The observed crash happened on `ResearchOrder`, but `PlanetConstructionOrder` and `AssetProductionOrder` use similar sequence-based uniqueness constraints. If their seed checks are also based on business fields instead of actual unique dimensions, reused development databases can fail later in the same way.

## Context
- `cockpit-validation`, `shipyard-validation`, and `planet-full-validation` add completed queue history.
- Existing QA flows can create real construction and asset production orders through supported dev endpoints.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/PlanetConstructionOrderConfiguration.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/AssetProductionOrderConfiguration.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Shipyard and construction-related endpoint tests

## Implementation Requirements
1. Inspect construction and asset production sequence uniqueness contracts.
2. Align seed existence checks with the real unique indexes.
3. Ensure `cockpit-validation`, `shipyard-validation`, and `planet-full-validation` do not collide if previous QA created queue rows.
4. Reuse or reserve deterministic sequence ranges rather than deleting existing rows.
5. Add tests that cover repeated profile apply after existing construction and asset production order state.

## UI/UX Requirements
- Construction and Shipyard read models should continue to show completed history without breaking active QA flows.

## Backend/API Requirements
- Stay inside Development seed behavior and tests.
- Reuse existing queue and persistence conventions.

## Safety Constraints
- Do not delete existing queue rows.
- Do not add reset behavior.
- No manual SQL.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Construction or Shipyard endpoint tests if additional runtime protections are needed

## Acceptance Criteria
- Construction and asset production seed rows no longer risk sequence collisions on reused development databases.
- Reapplying the richer profiles remains safe.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- Runtime failures may still surface if other seeded entities rely on hidden uniqueness assumptions; keep the audit explicit.
- Prefer one deterministic sequence strategy shared across seeded queue rows where possible.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the hardening focused on sequence-safe behavior.
