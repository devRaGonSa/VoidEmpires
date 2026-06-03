# TASK-18Q-galaxy-read-model-cockpit-validation-seed-smoke

---
id: TASK-18Q-galaxy-read-model-cockpit-validation-seed-smoke
title: Galaxy read model cockpit validation seed smoke
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - qa
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: high
---

## Goal
Prove through automated coverage that `cockpit-validation` yields a non-empty Galaxy read model for the seeded civilization.

## Purpose
Separate frontend rendering failures from backend data failures and prevent future regressions where tests stay green while Galaxy data is empty.

## Current Problem
The frontend may be empty either because the strategic read model is empty or because the frontend ignores valid data. Backend smoke coverage is needed to settle that quickly.

## Context
- `cockpit-validation` already supports other accepted cockpits.
- The seeded deterministic civilization and `Helios Gate` system are already documented.
- Existing seed tests and strategic map tests should be extended rather than replaced.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- strategic map query or service implementations under `src/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- existing strategic map endpoint or service tests
- `docs/dev/development-seed-profiles.md`

## Implementation Requirements
1. Add or update automated tests that:
   - apply `cockpit-validation`
   - read the strategic map endpoint or underlying strategic map service
   - assert the read model is non-empty for civilization `00000000-0000-0000-0000-000000000001`
2. Assert at minimum:
   - visible or relevant systems count is at least `1`
   - `Helios Gate` or the current deterministic seeded system is present
   - at least one owned planet exists
   - at least one visible or known comparison planet exists when expected
   - fleet marker count or transfer overlay count is non-zero if exposed
3. If the repository lacks a direct strategic map endpoint smoke test, add one using existing dev-endpoint test patterns.
4. Ensure the test fails meaningfully when the strategic map read model becomes empty.
5. Keep the task read-only; do not add Galaxy mutations.

## UI/UX Requirements
- None directly, but the assertions should match what the cockpit needs to render usefully.

## Backend/API Requirements
- Development-only route guards must remain intact.
- No production route changes.

## Safety Constraints
- No mutations.
- No new gameplay.
- No real database dependency in CI.

## Expected Files to Modify
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- strategic map backend tests adjacent to current coverage
- backend strategic map code only if the read model is actually broken

## Acceptance Criteria
- Automated tests prove Galaxy read-state is non-empty after `cockpit-validation`.
- A strategic-map regression to empty seeded data would fail tests.
- `dotnet` build and tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- If the backend is already correct, keep the implementation small and let later tasks fix the frontend render path.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Focus on smoke coverage, not broad backend redesign.
