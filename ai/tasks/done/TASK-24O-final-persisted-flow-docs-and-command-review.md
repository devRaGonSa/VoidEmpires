# TASK-24O

---
id: TASK-24O
title: Phase 24O - Final persisted flow docs and command review
status: done
type: platform
team: platform
supporting_teams:
  - docs
  - backend
  - qa
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Review the final docs and scripts for usability, safety, and copy-paste reliability.

## Purpose

This block is meant to let the user run backend-only persisted QA confidently. Before closure, the final scripts and command docs should be practical, safe, and easy to execute without guesswork.

## Current problem

Even if the underlying persisted flows are technically correct, weak scripts or unclear docs would still leave the user uncertain about which commands to run, what success should look like, or how to avoid unsafe environments.

## Context

By this point the block should already have baseline, Construction-order, and Research-order command paths plus a central persisted-flow guide. This review task should check those final artifacts rather than creating broad new tooling.

## Files to read first

- `scripts/dev-qa-baseline.ps1` if created
- `scripts/dev-qa-create-construction-order.ps1` if created
- `scripts/dev-qa-create-research-order.ps1` if created
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/development-seed-profiles.md`

## Component discovery

Inspect the final script parameter style, error output, default ids, and doc command flow. Prefer aligning language and examples across the scripts and docs instead of adding more tooling.

## Dependency analysis

Expected review flow:

- start backend command
- apply seed command
- baseline snapshot command
- create Construction order command
- create Research order command
- success or failure interpretation
- seed reapply preservation check

## Implementation requirements

1. Review the final scripts or commands for:
   - clear parameters
   - default `cockpit-validation` ids where appropriate
   - useful printed output
   - clear failure when the backend is not running
2. Review docs to ensure they include:
   - backend start command
   - seed apply command
   - order creation commands
   - expected success output
   - warning that rows persist
   - no production warning
   - cleanup or reseed expectations
3. Tighten wording only where needed for usability or safety.
4. Do not add destructive cleanup commands.
5. Do not expand the block into a broader automation suite.

## Backend/API requirements

- None expected.
- If docs reveal a current route mismatch, fix only the narrow mismatch needed for correctness.

## Frontend/UI requirements

- None required.

## Expected files to modify

- final persisted-flow scripts if they exist
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/development-seed-profiles.md`

## Safety constraints

- No delete or reset script
- No manual SQL
- No secrets
- No production-targeted command examples

## Acceptance criteria

- A user can execute backend-only persisted QA without guessing.
- Final scripts and docs are practical and safe.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If script output remains verbose because of raw JSON, the doc should call out the few fields the user actually needs to inspect so the flow stays usable.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to intended docs or script review fixes.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer polish and clarity fixes over feature expansion.
- If larger tooling improvements are still desirable, create focused follow-up tasks rather than broadening this review.
