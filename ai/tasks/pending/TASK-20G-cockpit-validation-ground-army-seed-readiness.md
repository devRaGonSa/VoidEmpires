# TASK-20G-cockpit-validation-ground-army-seed-readiness

---
id: TASK-20G-cockpit-validation-ground-army-seed-readiness
title: Cockpit validation Ground Army seed readiness
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - qa
  - docs
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Ensure `cockpit-validation` supports useful Ground Army QA.

## Purpose
Give the new cockpit rich, repeatable seeded data so it can be visually reviewed without manual SQL or fragile local setup steps.

## Current Problem
Ground Army cannot be visually accepted if `cockpit-validation` does not provide meaningful terrestrial readiness state.

## Context
- `cockpit-validation` already supports Galaxy, Planet, Construction, Research, Shipyard, Fleets, and Defenses.
- Ground Army should join that rich QA profile or a closely related validation profile if the seed contract supports it.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/Infrastructure/Development/DevelopmentSeedServiceTests.cs`
- Ground Army read-model tests
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Implementation Requirements
1. Extend `cockpit-validation` or add a `ground-army-validation` profile if the profile contract supports it.
2. Ensure the seeded Ground Army context includes, where supported:
   - Aurelia as a controlled planet
   - local resources
   - population or manpower
   - at least one Ground Army structure or readiness item
   - at least one available or ready Ground Army option if safe
   - at least one blocked option if supported
   - queue state if supported
   - a clear complete-due limitation if completion is not supported
3. Keep the seed idempotent.
4. Do not duplicate queue or order rows when the seed is applied repeatedly.
5. Add focused tests that verify:
   - Ground Army UI state loads after `cockpit-validation`
   - expected counts or readiness summaries exist
   - available and blocked distribution if supported
   - reapplying the profile does not duplicate state
6. Update seed docs accordingly.

## UI/UX Requirements
- The seeded state should support screenshot QA and clear visible differences between available and blocked paths.

## Backend/API Requirements
- Development-only seed changes only.
- No migrations.

## Safety Constraints
- No combat data that implies active attacks.
- No invasion data that implies active assaults.
- No manual SQL.
- No destructive reset behavior.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- focused Development seed tests under `tests/VoidEmpires.Tests/`
- `docs/dev/development-seed-profiles.md`

## Acceptance Criteria
- `cockpit-validation` supports Ground Army QA meaningfully.
- The seed remains idempotent.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend docs or types are touched

## Notes / Residual Risks
- If backend cannot support available actions safely, the seed should still provide useful readiness and blocked states.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task tightly scoped to seed readiness.
