# TASK-24M

---
id: TASK-24M
title: Phase 24M - Real flow regression against other cockpits
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - qa
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Ensure creating real Construction and Research orders does not break neighboring read-only or summary cockpits.

## Purpose

Persisted rows created during QA can affect shared resource, queue, and economy summaries. The repository needs proof that Planet, Market, Construction, and Research read-models remain coherent after those real orders exist.

## Current problem

Construction and Research mutations do not exist in isolation. If the new persisted rows break Planet or Market read-state assumptions, the repo could pass narrow mutation tests while still regressing accepted cockpit behavior.

## Context

Planet, Market, Construction, and Research already share resource, queue, and economy-adjacent data. This task should protect cross-cockpit stability without requiring browser or visual automation.

## Files to read first

- Planet UI-state tests
- Market UI-state tests
- Construction UI-state tests
- Research UI-state tests
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Component discovery

Inspect which current read-model tests already cover seeded post-mutation behavior and which ones assume a pristine queue state. Prefer extending those tests instead of creating parallel one-off checks.

## Dependency analysis

Expected regression flow:

- apply `cockpit-validation`
- create Construction order
- create Research order
- fetch Planet, Market, Construction, and Research read-state
- assert no errors and coherent summary behavior

## Implementation requirements

1. Add or strengthen tests that:
   - apply `cockpit-validation`
   - create a Construction order
   - create a Research order
   - fetch Planet UI or read state
   - fetch Market UI or read state
   - fetch Construction UI or read state
   - fetch Research UI or read state
   - assert no errors
   - assert summaries remain coherent
2. Do not require visual or browser automation.
3. Do not mutate unrelated modules.
4. If a shared read-model contract needs a narrow hardening fix, cover it with tests and keep the change focused.

## Backend/API requirements

- Backend tests are required if these read-model or endpoint paths exist.
- Do not introduce new gameplay behavior while fixing regressions.

## Frontend/UI requirements

- None required.
- If docs need a small note about cross-cockpit expectations after persisted orders, keep it concise.

## Expected files to modify

- read-model or endpoint tests under `tests/VoidEmpires.Tests/`
- backend read-model files only if a narrow bug is found
- docs only if a small regression expectation note is needed

## Safety constraints

- No destructive reset
- No manual SQL
- No mutation of unrelated modules
- No broad shared helper rewrite unless a real bug requires it

## Acceptance criteria

- Neighboring read-models tolerate real QA-created Construction and Research orders.
- `dotnet test` passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If a regression only appears in a single shared read-model summarizer, fix the narrow defect and keep this task from turning into a broad cockpit cleanup pass.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on cross-cockpit regression coverage or a narrow shared fix.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer strengthening existing read-model tests over adding a new bespoke regression harness.
- If multiple unrelated regressions appear, fix only the ones required for this persisted QA block and split the rest.
