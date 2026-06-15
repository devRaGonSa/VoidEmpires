# TASK-33L

---
id: TASK-33L
title: Playable session QA helper docs
status: pending
type: docs
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: medium
---

## Goal
Align QA helper documentation with the playable loop and explicitly avoid visual QA overclaims.

## Context
The repository has a helper script for preparing playable session state. Documentation should explain what it does and how later manual checks should use it.

## Implementation steps

1. Read current dev QA and gameplay flow docs.
2. Document the exact helper command:
   `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-playable-session-state.ps1 -ElapsedSeconds 3600`
3. Document that the helper creates a Development-safe playable start, can materialize resource accrual, and prints ids.
4. Document that the helper does not perform browser QA.
5. Document the recommended later manual flow:
   - run helper;
   - open `/onboarding` or use the returned Planet URL;
   - navigate hub links;
   - verify resources/cockpits visually later.
6. Make no behavior changes.

## Files to read first

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- scripts/dev-qa-prepare-playable-session-state.ps1
- scripts/check-dev-qa-scripts.ps1

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Docs clearly explain the helper and exact command.
- Docs state that visual/browser QA is not performed by the helper.
- No behavior changes are made.
- No visual QA is claimed.

## Constraints

- Do not edit the helper script unless documentation reveals an actual bug requiring a separate task.
- Do not claim browser/manual visual validation.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33L message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
