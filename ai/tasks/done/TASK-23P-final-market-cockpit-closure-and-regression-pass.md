# TASK-23P

---
id: TASK-23P
title: Phase 23P - Final Market cockpit closure and regression pass
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
  - docs
  - qa
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Close the Market cockpit block cleanly after the implementation tasks are processed, validated, and moved through the task lifecycle.

## Purpose

The block should finish with a functional read-only Market cockpit, updated docs, validated builds, and a clean task queue. This closure task ensures the queue does not stop halfway through the Market pass.

## Current problem

Without an explicit closure task, it is easy to finish the visible cockpit work but leave tasks unmoved, docs partially updated, or validation incomplete. Market also touches shared navigation, seed expectations, and economy read-state, so closure should verify that the accepted suite still holds together.

## Context

This block is limited to a read-only economy cockpit foundation. It must not quietly expand into transaction gameplay, player-to-player trade, resource mutation, or logistics execution.

## Files to read first

- `ai/tasks/pending/`
- `ai/tasks/in-progress/`
- `ai/tasks/done/`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/market-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `ai/current-state.md`

## Component discovery

Inspect the full queue state, final Market checklist, and shared smoke docs before declaring the block closed. Prefer a narrow regression sweep over broad cleanup work.

## Dependency analysis

Expected closure flow:

- process `TASK-23A` through `TASK-23P`
- move completed tasks to `ai/tasks/done`
- rerun final validation
- confirm docs and current-state alignment
- verify queue cleanliness and regression expectations

## Implementation requirements

1. Process `TASK-23A` through `TASK-23P` in order and move them to `ai/tasks/done` when complete.
2. Ensure `ai/tasks/pending` contains only `.gitkeep` at the end of the block.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
4. Update docs with a final regression checklist that includes:
   - apply `cockpit-validation` twice
   - open `/galaxy`
   - open `/planet`
   - open `/construction`
   - open `/research`
   - open `/shipyard`
   - open `/fleets`
   - open `/defenses`
   - open `/ground-army`
   - open `/espionage`
   - open `/market`
5. Confirm the final expected state:
   - Market loads and is not a placeholder
   - Market does not execute transactions
   - other accepted cockpits remain usable
   - `Galaxy` remains read-only
6. Run `git diff --stat` before closure and verify the block stayed within intentional scope.
7. If real blockers remain, create at most 3 specific follow-up tasks instead of opening a broad new queue.

## UI/UX requirements

- Closure guidance should remain screenshot-QA friendly
- Market should read as an economy cockpit, not a transaction console
- Shared accepted cockpit polish baseline should remain intact

## Backend/API requirements

- None expected unless an earlier task required a tiny metadata or read-model alignment
- Any backend change must stay covered by tests and preserve Development-only boundaries

## Expected files to modify

- task files under `ai/tasks/`
- `docs/dev/market-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `ai/current-state.md`
- any narrowly scoped Market frontend or backend files needed by earlier tasks

## Safety constraints

- No transactions
- No resource mutation
- No WebSockets
- No production auth
- No new gameplay systems beyond the read-only Market foundation

## Acceptance criteria

- The Market cockpit block can be considered closed after user visual QA.
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
- If a regression appears in another accepted cockpit because of shared navigation, helper, or seed changes, fix the narrow regression and keep the block focused.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the working tree matches the intended closure scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer fewer than 3 commits for the closure task itself.
- If more than 5 files or 200 lines would be exceeded by one task during closure, split the remaining work into up to 3 specific follow-ups.
