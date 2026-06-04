# TASK-24Z

---
id: TASK-24Z
title: Phase 24Z - Final persisted QA script hardening closure
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

Close the corrective script-hardening block cleanly after the implementation tasks are processed, validated, and moved through the task lifecycle.

## Purpose

This block should finish with hardened scripts, updated docs, a clean pending queue, and validated parser or repo checks so the persisted QA flow is usable again.

## Current problem

Without an explicit closure task, it would be easy to fix the baseline script but leave the helper docs, parser checks, or task lifecycle cleanup half-done.

## Files to read first

- `ai/tasks/pending/`
- `ai/tasks/in-progress/`
- `ai/tasks/done/`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `scripts/check-dev-qa-scripts.ps1` if created
- `ai/current-state.md`

## Implementation requirements

1. Move all `TASK-24Q` through `TASK-24Z` to `ai/tasks/done`.
2. Ensure `ai/tasks/pending` contains only `.gitkeep`.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
   - `scripts/check-dev-qa-scripts.ps1` if created
4. Ensure the working tree is clean.
5. Push final state.

## Backend/API requirements

- None expected beyond the narrow hardening changes from earlier tasks.

## Frontend/UI requirements

- None.

## Safety constraints

- No production behavior
- No destructive reset
- No gameplay expansion

## Acceptance criteria

- `dev-qa-baseline.ps1` no longer fails on missing `.amount`.
- Construction and Research QA scripts are hardened against DTO shape changes.
- Docs are updated.
- A parser or lightweight check script exists if feasible.
- No visual QA is required to close this block.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Run `scripts/check-dev-qa-scripts.ps1` if created.

## Notes / residual risks

- The final closeout should state clearly whether the scripts were runtime-tested against a live backend or only parser-checked in this session.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the working tree matches the intended closure scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer fewer than 3 commits for the closure task itself.
- If new blockers appear, create at most 3 focused follow-up tasks.
