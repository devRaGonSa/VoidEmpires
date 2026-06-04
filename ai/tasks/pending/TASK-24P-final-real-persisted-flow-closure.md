# TASK-24P

---
id: TASK-24P
title: Phase 24P - Final real persisted flow closure
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
  - docs
  - qa
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Close the persisted Construction and Research QA block cleanly after the implementation tasks are processed, validated, and moved through the task lifecycle.

## Purpose

The block should finish with validated persisted enqueue coverage, safe backend-only QA commands and docs, an empty pending queue, and a clean working tree. This closure task ensures the repo does not stop halfway through the hardening pass.

## Current problem

Without an explicit closure task, it is easy to land the core tests but leave docs half-updated, scripts inconsistently reviewed, or task files stranded in `pending` or `in-progress`.

## Context

This block is specifically about real persisted Development-only QA for Construction and Research. It must not quietly expand into broader gameplay systems, production behavior, market transactions, combat, or visual acceptance work.

## Files to read first

- `ai/tasks/pending/`
- `ai/tasks/in-progress/`
- `ai/tasks/done/`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `ai/current-state.md`

## Component discovery

Inspect the full queue state, final persisted-flow docs, and any scripts created by this block before declaring it closed. Prefer a narrow regression and cleanup sweep over broad new work.

## Dependency analysis

Expected closure flow:

- process `TASK-24A` through `TASK-24P`
- move completed tasks to `ai/tasks/done`
- rerun final validation
- confirm docs, scripts, and current-state alignment
- verify queue cleanliness and no new gameplay scope creep

## Implementation requirements

1. Move `TASK-24A` through `TASK-24P` to `ai/tasks/done` when processed.
2. Ensure `ai/tasks/pending` contains only `.gitkeep` at the end of the block.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
4. Confirm the final expected state:
   - Construction persisted enqueue path is covered
   - Research persisted enqueue path is covered
   - resource behavior is documented or tested
   - `cockpit-validation` preserves manual QA-created orders
   - backend-only PowerShell or command docs exist
   - no visual QA is required to close this block
   - no new gameplay systems were introduced
5. Run `git diff --stat` before closure and verify the block stayed within intentional scope.
6. Do not create broad follow-up tasks unless real blockers remain.
7. If follow-ups are needed, create at most 3 specific tasks.

## Backend/API requirements

- None expected beyond the narrow persisted QA hardening already introduced by earlier tasks.
- Any backend change that remained from earlier tasks must be covered by tests.

## Frontend/UI requirements

- None beyond any narrow refresh or feedback fixes already proven necessary.
- Do not turn closure into a visual redesign or acceptance task.

## Expected files to modify

- task files under `ai/tasks/`
- final persisted-flow docs and scripts
- `ai/current-state.md`
- any narrowly scoped backend or frontend files required by earlier tasks

## Safety constraints

- No production auth
- No production data
- No destructive reset
- No market transactions
- No combat
- No 3D
- No broad gameplay expansion

## Acceptance criteria

- The block can be closed technically.
- `ai/tasks/pending` is empty except for `.gitkeep`.
- Validation passes.
- The working tree is clean after the final commit.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- This task should not become a broad cleanup bucket.
- If a blocker remains outside the persisted QA scope, record it as a focused follow-up task rather than silently expanding the block.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the working tree matches the intended closure scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer fewer than 3 commits for the closure task itself.
- If more than 5 files or 200 lines would be exceeded by one task during closure, split the remaining work into up to 3 specific follow-ups.
