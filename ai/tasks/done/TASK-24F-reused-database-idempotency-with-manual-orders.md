# TASK-24F

---
id: TASK-24F
title: Phase 24F - Reused database idempotency with manual orders
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

Prove that `cockpit-validation` remains safe when manual QA has created real Construction and Research orders.

## Purpose

The repository already treats `cockpit-validation` as additive and idempotent. This task must verify that the same guarantee still holds once real persisted manual orders exist in a reused Development database.

## Current problem

Earlier seed hardening focused on deterministic baseline rows and queue-history uniqueness. Now that this block will create real QA orders, the seed must be explicitly proven not to delete them, duplicate baseline rows, or trigger key conflicts on reapply.

## Context

The Development database may be reused across multiple QA sessions. The standard path must remain seed-first and non-destructive, not ad hoc SQL reset.

## Files to read first

- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- existing seed idempotency tests
- Construction and Research order entities
- `docs/dev/development-seed-profiles.md`

## Component discovery

Inspect how `cockpit-validation` currently inserts baseline rows, protects existing data, and handles queue-history identity or sequence concerns. Then inspect how manual Construction and Research orders are persisted.

## Dependency analysis

Expected test flow:

- apply `cockpit-validation`
- create one Construction order through the supported path
- create one Research order through the supported path
- reapply `cockpit-validation`
- assert manual orders remain and seed rows are not duplicated

## Implementation requirements

1. Add a test that:
   - applies `cockpit-validation`
   - creates a Construction order through the normal application or dev endpoint path
   - creates a Research order through the normal application or dev endpoint path
   - reapplies `cockpit-validation`
   - asserts the manual Construction order still exists
   - asserts the manual Research order still exists
   - asserts no duplicate seed rows were created
   - asserts no unique-key or sequence conflict occurred
2. Use deterministic ids and count assertions where possible.
3. Update seed docs if the verification result adds a useful QA guarantee or caveat.
4. Avoid changing seed behavior unless the test actually fails.

## Backend/API requirements

- Backend tests are required.
- Avoid changing seed behavior unless it is genuinely broken.
- No schema changes.

## Frontend/UI requirements

- None required.

## Expected files to modify

- seed-related tests under `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs` only if a narrow fix is required
- `docs/dev/development-seed-profiles.md` if a guarantee or warning needs to be documented

## Safety constraints

- No destructive reseed
- No deletion of manual QA rows
- No sequence collisions
- No manual SQL

## Acceptance criteria

- The reused database scenario is covered automatically.
- `cockpit-validation` remains safe after manual Construction and Research orders exist.
- `dotnet test` passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If the test reveals that some manual rows are currently preserved but certain summaries are re-baselined, document that nuance explicitly rather than overstating idempotency.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on seed safety and its tests.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a narrow seed hardening fix over a broad reseed redesign.
- If multiple seed issues appear, address only the ones required for safe persisted QA and split the rest.
