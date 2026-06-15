# TASK-34N

---
id: TASK-34N
title: Runtime checklist for deferred queue QA
status: pending
type: docs
team: frontend
supporting_teams: [gameplay, platform]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: medium
---

## Goal
Prepare a deferred visual/manual QA checklist for queue progression.

## Context
Browser/visual QA remains deferred. The docs should provide exact manual steps and commands for a later pass.

## Implementation steps

1. Update `docs/dev/frontend-foundation-smoke-checklist.md`.
2. Add a deferred QA checklist:
   - create playable session;
   - enqueue construction;
   - enqueue research;
   - enqueue shipyard production;
   - run materialization helper with elapsed time;
   - refresh Planet;
   - verify building, research, and stock state;
   - verify no combat, movement, or missions appeared.
3. Explicitly say visual/browser QA was not performed in this block.
4. Include exact commands for:
   - `dev-qa-prepare-playable-session-state.ps1`;
   - `dev-qa-materialize-due-queues.ps1`.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- scripts/dev-qa-prepare-playable-session-state.ps1
- scripts/dev-qa-materialize-due-queues.ps1

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md
- Optional: docs/dev/persisted-gameplay-flow-checklist.md

## Acceptance criteria

- Later QA pass has clear steps.
- Exact helper commands are documented.
- Docs explicitly avoid visual QA overclaim.
- Validation passes.

## Constraints

- Do not perform browser/visual QA.
- Do not claim screenshots, manual browser testing, or visual validation.
- Do not add runtime behavior.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34N message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
