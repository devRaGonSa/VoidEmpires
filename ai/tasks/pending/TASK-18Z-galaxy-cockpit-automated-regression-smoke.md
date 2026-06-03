# TASK-18Z-galaxy-cockpit-automated-regression-smoke

---
id: TASK-18Z-galaxy-cockpit-automated-regression-smoke
title: Galaxy cockpit automated regression smoke
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
  - qa
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: high
---

## Goal
Add automated regression protection so Galaxy cannot silently degrade to an empty cockpit again while builds still pass.

## Purpose
Capture the specific failure shape of this block in durable tests.

## Current Problem
The repository passed build and test validation while the Galaxy cockpit was visually broken. Current coverage is not strong enough to catch a strategic-map regression of this kind.

## Context
- The project primarily relies on backend/service tests and frontend build validation.
- Heavy browser automation is not required unless it already exists.
- Seed profile smoke coverage is the most realistic safety net for the current architecture.

## Files to Inspect First
- `tests/VoidEmpires.Tests/`
- existing strategic map tests
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Implementation Requirements
1. Extend automated smoke coverage with Galaxy-specific regression assertions.
2. Add test names that mention Galaxy cockpit regression explicitly.
3. Assert at minimum:
   - visible or relevant systems count is at least `1`
   - a focusable system exists
   - map projection can derive at least one node or marker
   - planets count is at least `1`
   - transfer or fleet context exists when expected from the seeded profile
4. If a frontend unit-test framework already exists, consider a lightweight render-guard or state-branch test.
5. Do not add heavy browser automation unless the repository already uses it.

## UI/UX Requirements
- None directly, but the regression criteria should line up with what users actually see as a non-empty cockpit.

## Backend/API Requirements
- Use existing test patterns and development-only contracts.

## Safety Constraints
- No real database dependency.
- No external services.
- No new end-to-end infrastructure.

## Expected Files to Modify
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- existing strategic map smoke or endpoint test files
- frontend test files only if the framework already exists and the added test remains lightweight

## Acceptance Criteria
- Tests fail if the Galaxy read model becomes empty again after `cockpit-validation`.
- Regression coverage is explicit and understandable from test names.
- `dotnet test` passes after the new assertions are added.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend code or tests change

## Notes / Residual Risks
- Automated coverage should catch empty-state regressions, but manual visual QA is still required for layout and polish.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer targeted smoke coverage over broad test rewrites.
