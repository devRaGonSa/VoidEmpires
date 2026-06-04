# TASK-26L

---
id: TASK-26L
title: Phase 26L - Final Market visual polish closure
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: high
---

## Goal

Close the Market visual QA and read-only polish block with validated docs, a clean pending queue, and an accurate current-state summary.

## Current problem

Without a closure task, the block could finish with incomplete cleanup, stale current-state notes, or unclear confirmation about whether Market visual QA was performed or still remains user-driven.

## Context from current implementation

- The repository already uses explicit closure tasks for each multi-task block.
- This block is visual and read-only, so closure should confirm polish completion without claiming new market gameplay.
- The final output must report commits, validation results, and whether visual QA happened or remains pending.

## Files to read first

- ai/current-state.md
- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- ai/tasks/pending/TASK-26C-market-visual-readiness-audit.md
- scripts/check-dev-qa-scripts.ps1

## Expected files to modify

- ai/tasks/in-progress/*
- ai/tasks/done/*
- ai/tasks/pending/.gitkeep
- ai/current-state.md
- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements

1. Move `TASK-26C` through `TASK-26L` to `ai/tasks/done` after they are processed.
2. Ensure `ai/tasks/pending` contains only `.gitkeep`.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
4. Update `ai/current-state.md` to reflect:
   - Market visual and read-only polish completed
   - Market remains read-only
   - visual QA is still user-driven until screenshots are provided
   - no transactions were introduced
5. Do not create broad follow-up tasks unless a concrete blocker remains.
6. Final task output should report:
   - commit list
   - validation results
   - whether visual QA was performed or remains pending

## UI/UX requirements

- Closure should only claim the read-only polished baseline that was actually completed.
- Do not represent user-driven visual QA as fully automated acceptance.

## Backend/API requirements

- No new backend gameplay behavior should be introduced during closure.
- Preserve the current read-only Market boundary.

## Safety constraints

- No transactions.
- No resource mutation.
- No route execution.
- No feature expansion beyond read-only polish.

## Acceptance criteria

- Pending is empty except for `.gitkeep`.
- Current-state and docs reflect the final Market read-only polish outcome.
- Validation passes.
- Remaining visual QA status is reported honestly.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```

## Notes / residual risks

- Because this block is visual and user-driven at the acceptance layer, closure must distinguish implemented polish from final screenshot-backed approval.

