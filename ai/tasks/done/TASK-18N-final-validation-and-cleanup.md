# TASK-18N-final-validation-and-cleanup

---
id: TASK-18N-final-validation-and-cleanup
title: Final validation and cleanup
status: done
type: platform
team: platform
supporting_teams:
  - docs
  - backend
roadmap_item: "Block 18G-18R - Cockpit validation seed idempotency runtime hardening"
priority: high
---

## Goal
Close the corrective block cleanly after validation passes.

## Purpose
Finish the runtime hardening work with the repo in a stable, reviewable state and without pushing `main` yet.

## Current Problem
This corrective block should only be considered done if the queue is cleaned up, validation passes, and the working tree is left clean for the user to decide on pushing later.

## Context
- `main` is ahead of `origin/main` locally and must not be pushed automatically.
- The user wants validation and runtime confidence first.

## Files to Inspect First
- `ai/tasks/pending/`
- `ai/tasks/done/`
- Validation results from the block
- `ai/current-state.md`

## Implementation Requirements
1. Move all corrective task files to `ai/tasks/done`.
2. Ensure `ai/tasks/pending` contains only `.gitkeep`.
3. Run:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
4. Ensure the working tree is clean at the end.
5. Do not push unless explicitly instructed by the user after validation.

## UI/UX Requirements
- None directly, but the repo state should be easy for the user to inspect and push manually if desired.

## Backend/API Requirements
- No new backend behavior should be introduced here.
- Focus on validation and task lifecycle cleanup.

## Safety Constraints
- No push to `origin/main`.
- No destructive reset.
- No skipped validation.

## Expected Files to Modify
- `ai/tasks/done/` corrected task files
- `ai/tasks/pending/` for task removal only

## Acceptance Criteria
- The corrective task queue is empty except `.gitkeep`.
- Build, tests, and frontend build all pass.
- The working tree is clean.
- No push has occurred.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Manual runtime verification against the local Development backend is still expected after code changes.
- Record clearly whether runtime validation was actually executed.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep this task operational and closing-focused.
