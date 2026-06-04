# TASK-24H

---
id: TASK-24H
title: Phase 24H - Research validation failure scenarios
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Add real negative-path coverage for the Research persisted flow.

## Purpose

Research enqueue must reject invalid or unavailable requests consistently, preserve authoritative readiness rules, and avoid persisting invalid rows or mutating resources.

## Current problem

Research went through earlier backend or frontend alignment work, but this hardening block needs dedicated negative-path coverage around the real persisted mutation flow.

## Context

Research availability and enqueue validation were previously aligned. This task should pin the failure behavior so later QA can trust the backend rules.

## Files to read first

- Research command endpoint and service files
- Research error mapping
- `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- Research presentation helpers

## Component discovery

Inspect current Research validation branches, blocked or unavailable reason metadata, current negative tests, and any frontend error-mapping patterns already in place.

## Dependency analysis

Expected failure-path flow:

- invalid request -> Research endpoint or service validation
- non-success result or status -> no persisted queue row -> unchanged resources and follow-up read-state
- optional frontend error mapping -> Spanish user-facing message

## Implementation requirements

1. Add tests for failure scenarios such as:
   - invalid civilization id
   - invalid planet id if required by the current route
   - missing or invalid research id
   - unavailable or blocked research
   - insufficient resources if deterministic
   - missing owned-planet requirement
   - queue already occupied if applicable
2. Assert for each relevant failure:
   - non-success status or result
   - no order persisted
   - no resource mutation
3. If the frontend error mapper lacks coverage for a known backend error surfaced by this task, add a Spanish mapping.
4. Do not weaken validation or add fallback mutation behavior.

## Backend/API requirements

- Backend tests are required.
- Do not weaken validation.
- Keep any result-shape additions minimal and tested.

## Frontend/UI requirements

- Only adjust Spanish error mapping if needed.
- Keep technical ids and backend request details secondary.

## Expected files to modify

- Research tests under `tests/VoidEmpires.Tests/`
- backend error or result-shape files only if a narrow addition is needed
- frontend error mapping files only if the UI currently leaks poor error text

## Safety constraints

- No bypass
- No optimistic order creation
- No manual SQL
- No relaxed readiness or ownership checks

## Acceptance criteria

- Research negative paths are covered.
- Invalid requests do not mutate state.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Frontend build is required only if frontend mapping changes.

## Notes / residual risks

- If Research blocked-state reasons currently depend too heavily on generic errors, a small backend result-code improvement may be justified, but it should remain narrow and well tested.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on Research failure handling and any narrow mapping support.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer targeted negative tests over broad Research error refactors.
- If unrelated validation behavior needs cleanup, split it into a follow-up task.
