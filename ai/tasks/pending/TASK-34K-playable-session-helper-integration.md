# TASK-34K

---
id: TASK-34K
title: Playable session helper integration
status: pending
type: tooling
team: platform
supporting_teams: [gameplay, frontend]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: medium
---

## Goal
Align playable session QA helper documentation and optional script output with queue materialization.

## Context
Manual QA should have a clear flow from playable session creation through enqueueing and explicit queue materialization without changing existing helper behavior unexpectedly.

## Implementation steps

1. Read existing playable session and cockpit QA helper scripts.
2. Update docs and optionally scripts so the manual QA flow can:
   - create a playable session;
   - enqueue construction, research, and shipyard orders;
   - materialize resources and queues;
   - refresh Planet and cockpits.
3. If safe, add optional parameters to `dev-qa-prepare-playable-session-state.ps1` to print the queue materialization command.
4. Do not make the helper auto-complete queues unless explicitly passed and clearly documented.
5. Preserve existing helper behavior by default.

## Files to read first

- scripts/dev-qa-prepare-playable-session-state.ps1
- scripts/dev-qa-materialize-due-queues.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- Optional: scripts/dev-qa-prepare-playable-session-state.ps1
- Optional: scripts/check-dev-qa-scripts.ps1

## Acceptance criteria

- QA flow is clearer.
- Existing helper behavior is preserved by default.
- Scripts still parse.
- Backend tests pass if scripts or endpoints are touched.

## Constraints

- Do not auto-complete queues by default.
- Do not perform or claim browser/visual QA.
- Do not replace existing helper scripts.

## Validation

Before completing the task run:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34K message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
