# TASK-24U

---
id: TASK-24U
title: Phase 24U - Add PowerShell script parser checks
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 24Q-24Z - Persisted QA scripts runtime contract hardening"
priority: high
---

## Goal

Add lightweight validation that catches PowerShell syntax regressions in the persisted QA helpers.

## Purpose

The repo now relies on several PowerShell QA scripts. It should have at least a cheap parser check so future edits do not introduce syntax errors that are only discovered during a live runtime session.

## Current problem

The current workflow has no dedicated syntax-only check for these scripts.

## Files to read first

- `scripts/dev-qa-baseline.ps1`
- `scripts/dev-qa-create-construction-order.ps1`
- `scripts/dev-qa-create-research-order.ps1`
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Implementation requirements

1. Add or document a parser check using:
   - `[System.Management.Automation.Language.Parser]::ParseFile(...)`
2. Prefer a small helper script such as:
   - `scripts/check-dev-qa-scripts.ps1`
3. Ensure it parses:
   - `scripts/dev-qa-baseline.ps1`
   - `scripts/dev-qa-create-construction-order.ps1`
   - `scripts/dev-qa-create-research-order.ps1`
4. It should fail if parse errors exist.
5. It must not require the backend to be running.
6. Include the command in docs.

## Backend/API requirements

- None.

## Frontend/UI requirements

- None.

## Safety constraints

- Do not add heavy dependencies
- Do not turn this into a full test framework introduction

## Acceptance criteria

- A lightweight syntax-check path exists for the persisted QA scripts.
- The check fails on parse errors and can run without the backend.
- Validation passes.

## Validation

Run the parser check locally if possible and record the command in docs.

## Notes / residual risks

- Parser success does not guarantee runtime correctness, so later tasks should still validate shape handling where feasible.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to the parser helper and related docs.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a tiny local parser helper over adding external tooling.
