# TASK-24J

---
id: TASK-24J
title: Phase 24J - Dev QA PowerShell script create Research order
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

Add a repeatable PowerShell QA command or script that creates one real Research order against the running Development backend.

## Purpose

The user needs a concrete backend-only way to test real persisted Research data without relying on visual QA or manual SQL.

## Current problem

The repo has Research read-state and enqueue support, but it still needs a clean, approved command path for creating an actual Research order and printing the before or after state difference safely.

## Context

Research enqueue should use backend-provided command metadata where possible so the script or commands do not reconstruct invalid payloads by guesswork. The result must stay explicit that it creates real persistent rows in the dev database.

## Files to read first

- `scripts/`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- Research endpoint contracts
- existing docs with PowerShell snippets

## Component discovery

Inspect current repo script style, Research enqueue route shape, backend-provided command metadata, and any current docs that already demonstrate dev endpoint usage.

## Dependency analysis

Expected QA flow:

- optional baseline seed apply
- fetch Research UI state
- choose an available safe option using backend metadata
- call Research enqueue endpoint
- fetch Research UI state again
- print queue and resource deltas

## Implementation requirements

1. Add a script if appropriate, for example:
   - `scripts/dev-qa-create-research-order.ps1`
2. If a script is too risky or inconsistent with repo patterns, document exact PowerShell commands instead.
3. The script or documented command set should:
   - require the backend to be running
   - accept `civilizationId` and `planetId`, defaulting to the `cockpit-validation` ids
   - optionally apply `cockpit-validation` if a switch is provided
   - fetch Research UI state
   - pick the first available safe Research option using backend-provided command metadata
   - call the enqueue endpoint
   - fetch Research UI state again
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

- `scripts/dev-qa-create-research-order.ps1` if created
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/research-cockpit-checklist.md` only if a narrow backend-only QA note is needed

## Safety constraints

- The script must be explicit that it creates real dev database rows
- The script must not run by default in tests
- No destructive cleanup behavior
- No manual SQL

## Acceptance criteria

- There is a repeatable command path for creating a real Research order.
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

- If auto-selecting the first available research option becomes brittle, prefer an explicit research id or command target parameter over hidden heuristics.

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
