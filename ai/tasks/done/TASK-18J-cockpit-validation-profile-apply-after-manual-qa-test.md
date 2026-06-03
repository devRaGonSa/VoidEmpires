# TASK-18J-cockpit-validation-profile-apply-after-manual-qa-test

---
id: TASK-18J-cockpit-validation-profile-apply-after-manual-qa-test
title: Cockpit validation profile apply after manual QA test
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 18G-18R - Cockpit validation seed idempotency runtime hardening"
priority: high
---

## Goal
Add high-value regression coverage for the real user workflow of applying richer seed profiles after manual QA activity.

## Purpose
Protect the runtime scenario that failed in practice: a reused development database with prior research, shipyard, and possibly construction queue rows.

## Current Problem
Current tests prove clean-database idempotency but do not simulate a reused QA database that already contains endpoint-created queue rows. That gap allowed `cockpit-validation` to pass CI while failing at runtime.

## Context
- Manual QA can enqueue research and asset production orders through safe development endpoints.
- A realistic protection test should apply a baseline seed, create or simulate mutations, then reapply `cockpit-validation` repeatedly.

## Files to Inspect First
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs`

## Implementation Requirements
1. Add a regression scenario that:
   - applies `minimal-validation`;
   - executes or simulates one research enqueue;
   - executes or simulates one shipyard enqueue if safe;
   - optionally creates construction queue state if that flow is safe and already covered;
   - applies `cockpit-validation`;
   - applies `cockpit-validation` again.
2. Assert both profile applies succeed.
3. Assert key cockpit read models still load after the profile reapply:
   - Research
   - Shipyard
   - Planet
   - Fleets
4. Assert no duplicate-sequence exception path is triggered.
5. Keep the regression test focused on persisted state reuse rather than full browser coverage.

## UI/UX Requirements
- None directly, but the protected flow should preserve accepted cockpit state.

## Backend/API Requirements
- Reuse safe endpoint or service paths where available so the test matches real user behavior.
- Avoid inventing fake queue state that bypasses actual code paths when a safe supported path exists.

## Safety Constraints
- No real database requirement.
- No destructive reset.
- No unsupported gameplay flows.

## Expected Files to Modify
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Related endpoint tests that best capture the manual QA mutation path

## Acceptance Criteria
- The runtime reuse workflow is covered by automated regression tests.
- Reapplying `cockpit-validation` after manual QA no longer fails.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- If one mutation path is hard to express safely in current tests, document which path was simulated and why.
- Prioritize realistic research coverage first because that is the confirmed runtime failure.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the regression high-signal and maintainable.
