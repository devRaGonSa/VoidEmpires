# TASK-24A

---
id: TASK-24A
title: Phase 24A - Construction and Research persisted flow scope audit
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Audit the current Construction and Research mutation and read surfaces, then define the exact safe persisted QA flow for Development-only testing.

## Purpose

Before the repository starts a hardening pass around real persisted gameplay loops, it needs a precise inventory of which Construction and Research actions are truly persisted, which endpoints are dev-only, which reads reflect post-mutation state, and which operations remain unsafe or intentionally out of scope.

## Current problem

Construction and Research already have working cockpit foundations and mutation paths, but the repository still needs an explicit scope note that says exactly how to test the real persisted flows safely and repeatably. Without that audit, later tasks may guess at endpoints, assume the wrong resource behavior, or use unsafe completion paths.

## Context

Current accepted state shows that:

- `Construction` is the accepted construction cockpit.
- `Research` is the accepted technology or progress cockpit.
- `cockpit-validation` already provides a stable seeded baseline and is documented as additive and idempotent.
- prior work aligned Research enqueue metadata with frontend and backend validation.
- the current block is not about visual QA, production auth, new gameplay expansion, or destructive reset behavior.

## Files to read first

- `AGENTS.md`
- `ai/current-state.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Application/`
- `src/VoidEmpires.Infrastructure/`
- `tests/VoidEmpires.Tests/`

## Component discovery

Inspect the existing Construction and Research command endpoints, UI-state or read-model endpoints, queue persistence services, resource validation helpers, and development seed wiring first. Prefer documenting the current authoritative flow rather than inferring behavior from frontend copy alone.

## Dependency analysis

Map the current persisted flow chain for both modules, for example:

- dev endpoint mapping -> enqueue or create service -> resource validation or spend service -> queue persistence -> read-model refresh endpoint
- `cockpit-validation` seed -> deterministic baseline rows -> cockpit read-state -> mutation path -> follow-up read-state

Document which specific components call which services so later tasks know where persisted flow assertions belong.

## Implementation requirements

1. Identify the current Construction enqueue or create endpoint.
2. Identify the current Research enqueue or create endpoint.
3. Identify the read endpoints or read services used after mutation.
4. Identify how resources are validated, deducted, reserved, or otherwise handled during Construction enqueue.
5. Identify how resources are validated, deducted, reserved, or otherwise handled during Research enqueue.
6. Identify how queue rows are persisted and how the correct planet or civilization scope is enforced.
7. Identify any complete-due or completion routes and classify whether they are:
   - global
   - scoped
   - disabled
   - unsafe for this block
8. Identify existing test coverage and concrete test gaps.
9. Document the safe persisted QA flow in `docs/dev/persisted-gameplay-flow-checklist.md` or an equivalent new section in existing docs.
10. State explicitly:
   - no production auth
   - no manual SQL
   - no destructive reset
   - no automatic completion unless it is already proven safe and scoped
   - no visual QA required for this block

## Backend/API requirements

- Prefer docs and tests only for this task.
- Do not change behavior unless a clear contract mismatch is discovered.
- Do not introduce production endpoints.

## Frontend/UI requirements

- None required unless the audit discovers that current UI copy materially contradicts the backend contract.
- If a small copy correction is needed, keep it Spanish-first and secondary to the audit result.

## Expected files to modify

- `docs/dev/persisted-gameplay-flow-checklist.md` if created here
- existing Construction or Research docs only if the audit result needs to be recorded there
- backend tests only if a tiny audit-supporting contract correction is required

## Safety constraints

- Do not make any endpoint less restrictive
- Do not bypass validation
- Do not mutate resources from read endpoints
- Do not introduce production auth or deployment behavior
- Do not make completion paths broader than they already are

## Acceptance criteria

- The safe persisted QA flow is documented clearly.
- Later tasks know the exact mutation and read endpoints involved in the real Construction and Research loops.
- Known unsafe or out-of-scope completion behavior is called out explicitly.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Frontend build is required only if frontend files, shared docs tooling, or copy are touched.

If integration checks are reviewed and no configured integration suite exists, record:

`No integration tests configured.`

## Notes / residual risks

- The correct result may be a clarified documentation-first scope if the current behavior is already safe but under-documented.
- If the audit discovers a global completion path that is technically functional but too broad for repeatable QA, later tasks should document or fence it rather than silently relying on it.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm only intended docs or test files changed.
4. Commit with a clear message.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code or docs in a single implementation task.
- If the audit reveals a broader contract gap, create a follow-up task instead of expanding this one into general hardening work.
