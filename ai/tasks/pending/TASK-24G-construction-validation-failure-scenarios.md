# TASK-24G

---
id: TASK-24G
title: Phase 24G - Construction validation failure scenarios
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

Add real negative-path coverage for the Construction persisted flow.

## Purpose

A successful enqueue path is not enough. The repository also needs proof that invalid Construction requests are rejected cleanly, do not persist rows, and do not mutate resources.

## Current problem

Construction cards already show available and blocked states in the UI, but the backend remains authoritative. This block needs explicit negative-path coverage around the real dev mutation path.

## Context

Construction read-state has already been aligned with a working cockpit foundation. This task should pin down the backend’s failure behavior and, if needed, ensure frontend Spanish error mapping stays aligned.

## Files to read first

- Construction command endpoint and service files
- Construction error mapping
- `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- Construction presentation helpers

## Component discovery

Inspect current Construction validation branches, existing negative tests, blocked-state metadata from the read model, and current frontend error presentation.

## Dependency analysis

Expected failure-path flow:

- invalid request -> Construction endpoint or service validation
- non-success result or status -> no persisted queue row -> unchanged read-state and resources
- optional frontend error mapping -> Spanish user-facing message

## Implementation requirements

1. Add tests for failure scenarios such as:
   - invalid civilization id
   - invalid planet id
   - planet not controlled by the civilization
   - unknown building or option id
   - insufficient resources if deterministic
   - duplicate or queue-blocked state if applicable
   - disabled or unavailable option
2. Assert for each relevant failure:
   - non-success status or result
   - no order persisted
   - no resource mutation
3. If the frontend error mapper lacks coverage for a known backend error surfaced by this task, add a Spanish mapping.
4. Do not weaken validation or add fallback mutation behavior.

## Backend/API requirements

- Backend tests are required.
- Do not weaken validation.
- Keep any result-shape changes minimal and tested.

## Frontend/UI requirements

- Only adjust Spanish error mapping if needed.
- Keep technical ids and payload details secondary to player-facing messages.

## Expected files to modify

- Construction tests under `tests/VoidEmpires.Tests/`
- backend error or result-shape files only if a narrow addition is needed
- frontend error mapping files only if the UI currently leaks poor error text

## Safety constraints

- No bypass
- No optimistic order creation
- No manual SQL
- No relaxed ownership checks

## Acceptance criteria

- Construction negative paths are covered.
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

- If some blocked-state reasons are currently only inferable from generic backend failures, a small result-code improvement may be warranted, but it should remain narrow and Development-safe.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on Construction failure handling and any narrow mapping support.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer targeted negative tests over broad Construction error refactors.
- If multiple unrelated validation branches need cleanup, split follow-up tasks instead of broadening this one.
