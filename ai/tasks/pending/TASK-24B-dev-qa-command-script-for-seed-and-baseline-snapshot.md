# TASK-24B

---
id: TASK-24B
title: Phase 24B - Dev QA command script for seed and baseline snapshot
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Add a repeatable Development QA script or documented PowerShell command set that applies the seed profile and captures baseline state before any persisted mutations are created.

## Purpose

Real persisted gameplay QA should start from a deterministic, low-friction baseline. The repository needs one approved way to seed the dev database and inspect the pre-mutation Construction and Research state without relying on memory or manual SQL.

## Current problem

Manual QA currently depends on remembering a set of API calls and read-state checks. That is workable for ad hoc inspection, but it is too fragile for a hardening block focused on proving persisted gameplay loops.

## Context

`cockpit-validation` is already the preferred rich Development profile, is documented as idempotent, and should preserve extra rows created by manual QA. This task should make that baseline preparation easy to repeat before later order-creation tasks.

## Files to read first

- `scripts/`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/`

## Component discovery

Inspect existing PowerShell scripts, command docs, dev endpoint route patterns, and any current baseline inspection examples. Prefer matching existing repository scripting style if one already exists.

## Dependency analysis

Expected baseline flow:

- PowerShell script or documented commands -> `GET /api/dev/seeds/profiles`
- seed apply call -> `POST /api/dev/seeds/apply` with `cockpit-validation`
- repeat apply -> verify idempotent seed use
- Construction and Research read-state calls -> baseline resource and queue snapshot

## Implementation requirements

1. Add a lightweight PowerShell script if the repo’s `scripts/` patterns make that appropriate, for example:
   - `scripts/dev-qa-baseline.ps1`
2. If a new script is not the right pattern, add the exact command sequence to docs instead.
3. The script or documented command set should:
   - call `GET /api/dev/seeds/profiles`
   - apply `cockpit-validation`
   - apply `cockpit-validation` again
   - fetch Construction UI state
   - fetch Research UI state
   - optionally fetch Planet UI state
   - print key counts, queue summaries, and resources in a readable way
4. Do not require tools beyond PowerShell and a running backend.
5. Do not include secrets, connection strings, or auth assumptions.
6. Do not start the backend automatically unless that is already an established script pattern in the repository.

## Backend/API requirements

- No backend changes are expected.
- Reuse current dev endpoint routes and response shapes.

## Frontend/UI requirements

- None required.
- If docs reference cockpit pages, keep that secondary to the backend-only QA goal.

## Expected files to modify

- `scripts/dev-qa-baseline.ps1` if created
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/development-seed-profiles.md` only if it needs a small usage note

## Safety constraints

- The baseline script or commands must not delete data
- must not run migrations
- must not create orders
- must not reset or destroy reused dev state

## Acceptance criteria

- There is a repeatable way to apply the seed baseline and inspect pre-mutation state.
- Documentation explains how to run it against a local Development backend.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Frontend build is required only if frontend files or shared frontend docs are touched.

## Notes / residual risks

- If the repository already has a strong PowerShell scripting pattern, a script is preferable. If not, explicit copy-pasteable commands may be the safer result.
- The script should remain intentionally read-only so users can trust it as the starting point before creating real rows.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm changed files are limited to the intended script and docs scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a lightweight script or command guide over a broad automation harness.
- If more robust tooling is needed later, create a follow-up task instead of expanding this one.
