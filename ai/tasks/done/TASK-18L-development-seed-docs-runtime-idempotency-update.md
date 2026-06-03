# TASK-18L-development-seed-docs-runtime-idempotency-update

---
id: TASK-18L-development-seed-docs-runtime-idempotency-update
title: Development seed docs runtime idempotency update
status: done
type: platform
team: platform
supporting_teams:
  - docs
  - backend
roadmap_item: "Block 18G-18R - Cockpit validation seed idempotency runtime hardening"
priority: medium
---

## Goal
Update the seed profile docs to explain runtime idempotency over reused local development databases.

## Purpose
Set the right expectations for how seed profiles behave after manual QA has already created queue rows.

## Current Problem
The current seed docs emphasize determinism and idempotency but do not explain clearly enough how reapply should behave over a reused development database with prior research, construction, or Shipyard queue state.

## Context
- The user explicitly does not want manual SQL or destructive reset behavior.
- `cockpit-validation` should be a safe reapply tool over an existing development DB.

## Files to Inspect First
- `docs/dev/development-seed-profiles.md`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- Runtime hardening tests for seed profile apply

## Implementation Requirements
1. Update `docs/dev/development-seed-profiles.md`.
2. Explain that seed profiles are intended to be reapplicable over an existing development database.
3. Explain that the profiles do not destructively reset existing data.
4. Explain expected behavior after manual QA creates queue rows.
5. Include the `cockpit-validation` apply command.
6. Do not tell users to use manual SQL.

## UI/UX Requirements
- The docs should be copy-pasteable and practical for local QA.
- Wording should reduce confusion about when to reapply a profile versus when to use a fresh disposable DB.

## Backend/API Requirements
- No backend change is required beyond any clarifying response details introduced elsewhere in the block.

## Safety Constraints
- No secrets.
- No connection strings.
- No unsupported reset instructions.

## Expected Files to Modify
- `docs/dev/development-seed-profiles.md`

## Acceptance Criteria
- The docs explain runtime idempotency expectations over reused local development databases.
- The docs include a working `cockpit-validation` example.
- Validation remains green.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- Keep the docs aligned with actual hardened behavior rather than idealized behavior.
- Prefer a concise operational explanation over a long theoretical section.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the update practical.
