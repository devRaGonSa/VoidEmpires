# TASK-18D-cockpit-profile-endpoint-smoke-tests

---
id: TASK-18D-cockpit-profile-endpoint-smoke-tests
title: Cockpit profile endpoint smoke tests
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: high
---

## Goal
Add cross-cockpit smoke tests that prove seeded profiles support the intended cockpit UI-state conditions.

## Purpose
Catch regressions where builds and unit tests remain green but a cockpit read-model becomes visually thin, contradictory, or unusable for QA.

## Current Problem
Recent history already showed that passing validation is not enough if Research reports zero available items or Shipyard loads with weak or contradictory state. Cross-cockpit smoke assertions are needed to protect the intended QA baseline of richer seed profiles.

## Context
- The repo already has endpoint and service tests for Planet, Research, Shipyard, and Fleets.
- `cockpit-validation` is intended to become the richer reusable QA baseline.
- Browser automation is not required for this block; read-model and endpoint assertions are enough.

## Files to Inspect First
- `tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs`
- Strategic map endpoint tests if present
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`

## Implementation Requirements
1. Add smoke assertions for `cockpit-validation` covering at least:
   - Galaxy or strategic map showing visible systems, planets, and transfer or fleet context;
   - Planet cockpit showing owned planet, resources, current buildings, and module links or equivalent state;
   - Construction cockpit showing available and blocked general actions when supported;
   - Research cockpit showing available and blocked items and preserving the enqueue smoke path;
   - Shipyard cockpit showing available and blocked production options plus stock or queue readiness;
   - Fleets cockpit showing stationed groups and active transfer context.
2. Keep the tests at endpoint or service level where possible.
3. Do not require browser automation.
4. Avoid brittle exact UI text assertions unless the DTO contract depends on them.
5. Ensure the tests would catch regressions such as Research or Shipyard unexpectedly reporting zero available options.

## UI/UX Requirements
- None directly, but the protected states should reflect real visual QA expectations.

## Backend/API Requirements
- Extend endpoint and service test coverage where it gives the best signal.
- Do not turn these into oversized integration tests with unnecessary setup.

## Safety Constraints
- No real database.
- No external services.
- No hidden mutation just to satisfy smoke expectations.

## Expected Files to Modify
- `tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs`
- Potentially strategic map or shared seed tests if needed

## Acceptance Criteria
- `cockpit-validation` is protected by cross-cockpit smoke tests.
- Key read-model conditions are asserted for all intended cockpits.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- Manual browser QA still matters, but these tests should catch major data-thin regressions earlier.
- Keep the assertions high-signal and stable over time.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Split wider test expansion if too many endpoints need large updates at once.
