# TASK-41AU

---
id: TASK-41AU
title: Final product surface closure
status: pending
type: workflow
team: platform
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Close the block subset if all product-surface tasks are complete.

## Context
This task should move completed TASK-41A through TASK-41AU to done only after their validations have passed.

## Implementation steps

1. Verify TASK-41A through TASK-41AT are complete and in `ai/tasks/done`.
2. Move this task through the normal in-progress to done workflow.
3. Keep pending only for remaining closure/follow-up tasks if any.
4. Run pending directory and backend validation commands.
5. Do not close incomplete or blocked tasks.

## Files to read first

- ai/tasks/pending/
- ai/tasks/in-progress/
- ai/tasks/done/
- ai/current-state.md

## Expected files to modify

- ai/tasks/pending/
- ai/tasks/in-progress/
- ai/tasks/done/
- ai/current-state.md

## Acceptance criteria

- Completed TASK-41A through TASK-41AU are in `ai/tasks/done`.
- Pending contains only remaining closure/follow-up tasks if any.
- Backend validation passes.

## Constraints

- Do not move incomplete tasks to done.
- Do not claim manual/browser QA.
- Do not apply migrations.

## Validation

Before completing the task ensure:

- `dir ai/tasks/pending`
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
- Split the work into additional tasks if limits are exceeded.
