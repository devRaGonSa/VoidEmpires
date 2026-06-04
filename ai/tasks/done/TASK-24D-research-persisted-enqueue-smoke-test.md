# TASK-24D

---
id: TASK-24D
title: Phase 24D - Research persisted enqueue smoke test
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Add or strengthen automated coverage proving that a Research order can be created through the real dev flow and persists in subsequent reads.

## Purpose

The repository needs an explicit persisted QA proof for Research so future work does not rely on assumptions from earlier UI-alignment tasks alone.

## Current problem

Research already has a working enqueue path, but this block needs dedicated persisted-flow coverage that proves the mutation writes real database rows and shows up in later read-state.

## Context

Research already went through multiple rounds of alignment around availability metadata, readiness rules, and enqueue validation. This task should lock down the final persisted behavior.

## Files to read first

- `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- Research command services
- Research read-model services
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `docs/dev/research-cockpit-checklist.md`

## Component discovery

Inspect the current Research enqueue endpoint, backend-provided command metadata, seeded available research options, and any existing tests that already validate partial queue or availability behavior.

## Dependency analysis

Expected test flow:

- apply `cockpit-validation` or `research-validation`
- fetch Research read-state
- select a known available option using backend-provided metadata
- call Research enqueue endpoint
- fetch Research read-state again
- assert persisted queue visibility and correct scope

## Implementation requirements

1. Add an integration-style test using existing infrastructure.
2. Test flow should:
   - arrange `cockpit-validation` or `research-validation`
   - fetch Research UI or read state
   - select a known available Research option
   - call the Research enqueue endpoint using backend-provided command metadata where possible
   - assert success
   - fetch Research read state again
   - assert queue count increased or a new order appears
   - assert the order belongs to the correct civilization and planet context if applicable
   - assert completed or read-only sections remain intact
3. If exact resource deduction is deterministic and stable, assert the before or after resource delta.
4. Assert that unavailable research cannot be enqueued.
5. Keep the test deterministic.
6. Add narrow response metadata only if necessary for safe assertions and cover it with tests.

## Backend/API requirements

- Use existing dev endpoint or application service patterns.
- Do not add production endpoints.
- Keep any contract extension minimal and tested.

## Frontend/UI requirements

- None required for this task.

## Expected files to modify

- relevant Research tests under `tests/VoidEmpires.Tests/`
- backend endpoint or DTO files only if a tiny metadata addition is required

## Safety constraints

- Do not bypass readiness validation
- Do not bypass owned-planet or civilization validation
- Do not use manual SQL
- Do not weaken blocked or unavailable option enforcement

## Acceptance criteria

- Automated coverage proves the Research persisted enqueue path works.
- The new persisted row is visible in subsequent reads.
- `dotnet test` passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If Research availability is tied to a very specific seed state, the test should use the documented deterministic option rather than probing arbitrarily.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on Research test coverage or narrow contract support.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a narrow persisted-flow smoke test over a broad Research refactor.
- If additional contract clarity is needed, keep it minimal and explicit.
