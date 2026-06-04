# TASK-24C

---
id: TASK-24C
title: Phase 24C - Construction persisted enqueue smoke test
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Add or strengthen automated coverage proving that a Construction order can be created through the real dev flow and persists in subsequent reads.

## Purpose

The repository needs backend confidence that Construction enqueue is not merely UI-ready, but actually creates persisted rows visible through the normal read-state after mutation.

## Current problem

The Construction cockpit already shows available options and queue state, but this block needs a dedicated automated proof that the persisted enqueue path works end to end with real Development-only data.

## Context

Construction already uses current dev-safe mutation paths and post-action read-state refresh behavior. This task should pin the backend contract so later QA can rely on it safely.

## Files to read first

- `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- Construction command services
- Construction read-model services
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`

## Component discovery

Inspect the existing Construction command endpoint, post-enqueue read-state endpoint, seeded available options, and any current tests around queue persistence, ownership, or affordability.

## Dependency analysis

Expected test flow:

- apply seed baseline -> fetch Construction or Planet read state
- select a known available option -> call Construction enqueue endpoint
- persisted queue write -> fetch Construction or Planet read state again
- assert new persisted order visibility and correct scope

## Implementation requirements

1. Add an integration-style test using the existing test infrastructure.
2. Test flow should:
   - arrange `cockpit-validation` or the smallest deterministic owned-planet baseline
   - fetch Construction UI or read state
   - select a known available Construction option
   - call the Construction enqueue or create endpoint
   - assert a successful response
   - fetch Construction or Planet read state again
   - assert queue count increased or the new order is present
   - assert the order belongs to the correct planet and civilization
   - assert resource validation was applied
3. If exact resource deduction is deterministic and stable, assert the before or after resource delta.
4. If exact deduction is already covered elsewhere, assert at least persisted queue visibility and correct scope.
5. Keep the test deterministic and independent of wall-clock waiting.
6. If the endpoint response lacks enough metadata for safe assertions, add narrow response metadata and cover it with tests.

## Backend/API requirements

- Use existing dev endpoint patterns.
- Do not introduce production endpoints or test-only bypass paths.
- Any response contract change must stay Development-only and be covered by tests.

## Frontend/UI requirements

- None required for this task.

## Expected files to modify

- relevant Construction tests under `tests/VoidEmpires.Tests/`
- backend endpoint or DTO files only if a tiny test-supporting metadata addition is required

## Safety constraints

- Do not loosen Construction validation
- Do not bypass ownership or civilization checks
- Do not use manual SQL
- Do not depend on uncontrolled due-time completion

## Acceptance criteria

- Automated coverage proves the Construction persisted enqueue path works.
- The new persisted row is visible in subsequent reads.
- `dotnet test` passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If Construction option availability is more dynamic than expected, the test should choose a deterministic seeded option rather than a brittle first-match assumption.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on Construction test coverage or narrow contract support.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a narrow smoke test over a broad refactor of Construction services.
- If the current endpoint is too opaque for safe assertions, add only minimal metadata and stop there.
