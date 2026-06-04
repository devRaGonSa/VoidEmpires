# TASK-24R

---
id: TASK-24R
title: Phase 24R - Dev QA baseline script resource shape fix
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 24Q-24Z - Persisted QA scripts runtime contract hardening"
priority: high
---

## Goal

Fix `dev-qa-baseline.ps1` so it prints resources safely using the actual DTO shape returned by current Development endpoints.

## Purpose

The baseline helper should remain useful even if resource data is returned as row objects, named properties, or nested stockpile structures. It must not crash just because one specific property path changed.

## Current problem

The script directly accesses `.amount` on each resource row and fails when the real object does not expose that property.

## Files to read first

- `scripts/dev-qa-baseline.ps1`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- audited DTO notes from Task 24Q
- relevant UI-state endpoint tests if they already assert shape

## Implementation requirements

1. Replace direct `.amount` access with a robust formatter.
2. The formatter should support common shapes such as:
   - `resourceType + amount`
   - `name + amount`
   - `resource + amount`
   - `label + value`
   - flat `metal/crystal/gas/credits` objects
   - nested stockpile objects
3. If no recognized resource shape exists, print a readable warning instead of throwing.
4. Keep strict mode if it is already used, but guard property access safely.
5. Ensure output includes:
   - resources or reserves
   - queue counts
   - available counts
   - profile apply result
6. Do not hide real HTTP failures or seed failures.

## Backend/API requirements

- No backend change is expected.

## Frontend/UI requirements

- None.

## Safety constraints

- Do not swallow real endpoint failures
- Do not mutate state
- Do not add destructive behavior

## Acceptance criteria

- `dev-qa-baseline.ps1` no longer crashes when the resource shape does not expose `.amount`.
- The script remains readable and explicit about warnings versus failures.
- Validation passes.

## Validation

Use a parser-only or safe local validation path. If the script supports `WhatIf`, use it; otherwise use parsing only.

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If endpoint payloads evolve again, the formatter should still degrade into a warning instead of a hard runtime stop for non-critical display fields.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on the baseline script and its docs.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer defensive formatting helpers over broad script rewrites.
