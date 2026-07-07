# TASK-43B-visual-checklist-registration-copy-refresh

---
id: TASK-43B
title: Visual checklist registration copy refresh
status: done
type: docs
team: frontend
supporting_teams: []
roadmap_item: "Post-Block 42 documentation cleanup"
priority: medium
---

## Goal
Refresh the deferred visual QA checklist line that still frames onboarding as a local game start.

## Context
Block 42 made `/register` the product account entry and `/onboarding` a compatibility alias. The deferred visual checklist should ask future browser QA to verify registration/login navigation, not a local-game onboarding start.

## Implementation steps

1. Review the deferred visual QA checklist.
2. Update the stale onboarding/local-game wording to the Block 42 registration boundary.
3. Keep browser/manual QA explicitly deferred.

## Files to read first

- docs/dev/deferred-visual-qa-master-checklist.md
- docs/dev/product-readiness-report.md
- ai/current-state.md

## Expected files to modify

- docs/dev/deferred-visual-qa-master-checklist.md

## Acceptance criteria

- Checklist references registration/login/account entry rather than local-game onboarding.
- Checklist does not claim browser QA was performed.

## Constraints

- Documentation only.
- Keep the change minimal.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

## Completion notes

- Refreshed deferred visual QA checklist account-entry wording for Block 42.
- `dotnet build --no-restore`: passed with 0 warnings and 0 errors.
- `dotnet test --no-build`: passed with 779 tests, 0 failed, 0 skipped.
- No browser/manual QA, SQL Server connection, migration apply, seed apply, or real credential handling was performed.
