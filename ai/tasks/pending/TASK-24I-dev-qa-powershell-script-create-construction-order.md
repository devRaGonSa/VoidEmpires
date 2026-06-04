# TASK-24I

---
id: TASK-24I
title: Phase 24I - Dev QA PowerShell script create Construction order
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

Add a repeatable PowerShell QA command or script that creates one real Construction order against the running Development backend.

## Purpose

The user needs a concrete backend-only way to test real persisted Construction data without relying on visual QA or manual SQL.

## Current problem

The repo has read-state and cockpit docs, but it still needs a simple, approved command path for creating an actual Construction order and printing the before or after state difference safely.

## Context

The Development backend is expected to run locally and expose dev endpoints. The script should use the real endpoint contract, default to the documented seeded ids, and remain explicit that it creates persistent rows in the dev database.

## Files to read first

- `scripts/`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- Construction endpoint contracts
- existing docs with PowerShell snippets

## Component discovery

Inspect current repo script style, Construction mutation route shape, baseline snapshot commands, and any existing docs that already demonstrate dev endpoint invocation.

## Dependency analysis

Expected QA flow:

- optional baseline seed apply
- fetch Construction UI state
- choose an available safe option
- call Construction enqueue endpoint
- fetch Construction UI state again
- print queue and resource deltas

## Implementation requirements

1. Add a script if appropriate, for example:
   - `scripts/dev-qa-create-construction-order.ps1`
2. If a script is too risky or inconsistent with repo patterns, document exact PowerShell commands instead.
3. The script or documented command set should:
   - require the backend to be running
   - accept `civilizationId` and `planetId`, defaulting to the `cockpit-validation` ids
   - optionally apply `cockpit-validation` if a switch is provided
   - fetch Construction UI state
   - pick the first available safe Construction option, or require an explicit option parameter if auto-pick is too brittle
   - call the enqueue or create endpoint
   - fetch Construction UI state again
   - print before or after queue count and resources if available
4. The script must not run migrations.
5. The script must not delete data.
6. The script must not perform destructive reset behavior.
7. The script should fail clearly if the backend is not reachable.

## Backend/API requirements

- No backend changes are expected.
- Use existing dev endpoint contracts only.

## Frontend/UI requirements

- None required.

## Expected files to modify

- `scripts/dev-qa-create-construction-order.ps1` if created
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/construction-cockpit-checklist.md` only if a narrow backend-only QA note is needed

## Safety constraints

- The script must be explicit that it creates real dev database rows
- The script must not run by default in tests
- No destructive cleanup behavior
- No manual SQL

## Acceptance criteria

- There is a repeatable command path for creating a real Construction order.
- The usage is documented clearly.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Frontend build is required only if frontend docs are touched.

## Notes / residual risks

- Auto-picking the first available option is convenient but may be brittle if availability becomes more dynamic. If needed, prefer an explicit option parameter over hidden heuristics.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to the intended script or docs scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a small explicit script over a larger QA automation framework.
- If route discovery becomes too complex, document exact commands instead of widening scope.
