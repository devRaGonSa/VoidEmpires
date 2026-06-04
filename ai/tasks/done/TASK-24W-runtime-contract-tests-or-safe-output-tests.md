# TASK-24W

---
id: TASK-24W
title: Phase 24W - Runtime contract tests or safe output tests
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 24Q-24Z - Persisted QA scripts runtime contract hardening"
priority: medium
---

## Goal

Add lightweight automated coverage or script-level checks for the PowerShell formatting helpers where feasible.

## Purpose

The script hardening should not rely only on manual inspection. Even without a full PowerShell test framework, the repo should have some repeatable validation for the most important formatting fallbacks.

## Current problem

There is currently no automated check around the helper logic that formats resources from multiple possible object shapes.

## Files to read first

- `scripts/dev-qa-baseline.ps1`
- `scripts/check-dev-qa-scripts.ps1` if created
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Implementation requirements

1. If the scripts contain local formatting helpers, add a simple PowerShell check path where feasible.
2. If no PowerShell test framework exists, add safe sample-shape checks inside `check-dev-qa-scripts.ps1` or document manual checks.
3. Validate resource formatting for at least:
   - a row with `amount`
   - a flat stockpile object with `metal/crystal/gas/credits`
   - an unknown shape fallback
4. Do not add heavy dependencies.

## Backend/API requirements

- None.

## Frontend/UI requirements

- None.

## Safety constraints

- Keep the check lightweight
- Do not require a running backend
- Do not add external test frameworks just for scripts

## Acceptance criteria

- There is at least one repeatable validation path for the script formatting helpers.
- The check is lightweight and repo-native.
- Validation passes.

## Validation

Run `scripts/check-dev-qa-scripts.ps1` if created.

## Notes / residual risks

- Lightweight helper validation is better than none, but it still does not replace a full live backend run when the user wants end-to-end confirmation.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to the lightweight check surface and related docs.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer minimal built-in checks over new test infrastructure.
