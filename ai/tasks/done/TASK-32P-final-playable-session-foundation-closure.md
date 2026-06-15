# TASK-32P

---
id: TASK-32P
title: Close playable session foundation block and publish final state
status: pending
type: feature
team: gameplay
supporting_teams: [frontend, backend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Close the full 32A-32P block by moving completed tasks, publishing the final repository state, and reporting the exact validation and QA-helper usage details.

## Context
This is the block-closure task after all audits, modal work, onboarding work, resource-economy work, and QA-helper work are complete. It exists to verify the task lifecycle, ensure `ai/tasks/pending` is clean again, and capture a precise final report without claiming visual QA.

## Implementation steps

1. Verify TASK-32A through TASK-32P are complete and move them from `ai/tasks/pending` to `ai/tasks/done`.
2. Confirm `ai/tasks/pending` contains only `.gitkeep`.
3. Run the full final validation set if TASK-32O did not already capture the current final result.
4. Commit and push the closure state.
5. Produce the final output with commit list, validation results, test count, exact QA helper command, and an explicit note that visual QA was not performed.

## Files to read first

- `ai/current-state.md`
- `ai/tasks/pending`
- `ai/tasks/done`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `scripts/dev-qa-prepare-playable-session-state.ps1`

## Expected files to modify

- `ai/tasks/pending/*`
- `ai/tasks/done/*`
- `ai/current-state.md` only if a final accuracy correction is still required

## Acceptance criteria

- TASK-32A through TASK-32P are moved to `ai/tasks/done`.
- `ai/tasks/pending` contains only `.gitkeep`.
- Final validation passes and the report includes the exact requested output items.
- The final note explicitly states that visual QA was not performed.

## Constraints

- Follow the repository task workflow strictly.
- Keep the final output factual and specific.
- Do not claim combat, fleet movement, missions, or production auth behavior that was not implemented.

## Validation

Before completing the task ensure:

- `git diff --stat` has been reviewed.
- `git diff --name-only` matches the task's expected file scope.
- The required build, test, frontend, and script validations pass.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore(ai): close playable session foundation block`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- If closure work reveals unrelated issues, create focused follow-up tasks instead of expanding this task.
