# TASK-16W-research-submit-integration-smoke-test

---
id: TASK-16W-research-submit-integration-smoke-test
title: Research submit integration smoke test
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: medium
---

## Goal
Add a single high-value smoke test that protects the full Research submit path.

## Purpose
The previous tests passed while the visual submit flow still failed. We need an automated test that covers the exact path the user uses, from the available card to successful enqueue and refreshed state.

## Current Problem
The current regression was not caught by the existing coverage. The new smoke test should fail if the available read metadata cannot be submitted successfully or if the post-submit state does not change as expected.

## Context
- Existing Research tests are useful but were not sufficient to catch the read/mutation mismatch.
- This task should add a focused smoke test rather than broad new test scaffolding.
- The test must use the same seeded scenario and the same command metadata that the UI uses.

## Files to Inspect First
- `tests/VoidEmpires.Tests/`
- Existing Research endpoint tests
- Existing `DevelopmentSeedServiceTests`
- Existing web application factory patterns

## Implementation Requirements
1. Add or adjust a test that performs:
   - apply `minimal-validation` seed;
   - GET Research UI state;
   - find the first available technology;
   - extract the exact command metadata from the read model;
   - POST enqueue using that metadata;
   - assert success;
   - GET Research UI state again;
   - assert queue count increased or technology status changed.
2. Add a complementary negative case:
   - pick a blocked technology;
   - attempt enqueue;
   - assert validation rejection with a meaningful code.
3. Ensure the test fails if available read metadata cannot be submitted.
4. Keep the test deterministic and narrow.

## UI/UX Requirements
- None directly.

## Backend/API Requirements
- The test should hit the dev endpoint or service path used by the cockpit.
- Do not rely on real PostgreSQL.

## Safety Constraints
- No external services.
- No real database dependency.
- Do not widen the assertion so much that the regression could still pass unnoticed.

## Expected Files to Modify
- `tests/VoidEmpires.Tests/` Research-related test files
- Possibly one supporting helper or fixture if required

## Acceptance Criteria
- The test would have caught the current failure.
- `dotnet test --no-build` passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- This is the second most important task after backend validation alignment.
- If the endpoint contract changes, keep the assertions focused on visible state rather than implementation trivia.
- The test should protect the exact browser QA path, not just a backend branch.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer one smoke test that covers the real user flow.
