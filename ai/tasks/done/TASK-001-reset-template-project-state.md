# TASK-001

---
id: TASK-001
title: Reset inherited AI Platform template state for VoidEmpires
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 0 - Repository and AI Platform setup"
priority: high
---

## Goal
Clean the AI Platform project state so the repository no longer looks like the original ai-dev-platform-template project.

## Context
The AI Platform template was initialized in the VoidEmpires repository. The repository currently contains inherited task history under `ai/tasks/done` that does not belong to VoidEmpires and can confuse future project tracking.

## Implementation steps

1. Review the contents of `ai/tasks/done`.
2. Move inherited template task files that do not belong to VoidEmpires from `ai/tasks/done` to `ai/tasks/obsolete`.
3. Preserve filenames when moving files.
4. Keep all `.gitkeep` files in place.
5. Add a short note under `ai/tasks/obsolete` explaining that the archived files came from the AI Platform template initialization and are not VoidEmpires delivery tasks.
6. Verify that `ai/tasks/done` only contains `.gitkeep` unless real VoidEmpires tasks already exist there.
7. Do not delete historical files unless moving them is not possible.
8. Do not touch `ai/tasks/pending` except for the task lifecycle work required by the implementation.
9. Do not modify application source code.

## Files to read first

- `ai/task-template.md`
- `ai/tasks/done/.gitkeep`
- `ai/tasks/obsolete/.gitkeep`
- `ai/tasks/pending/.gitkeep`

## Expected files to modify

- `ai/tasks/done/*`
- `ai/tasks/obsolete/*`
- `ai/tasks/obsolete/README.md` or equivalent note file

## Acceptance criteria

- Inherited template task files are no longer present in `ai/tasks/done`.
- The moved files are preserved under `ai/tasks/obsolete` with the same filenames.
- `.gitkeep` files remain in the task directories.
- `ai/tasks/obsolete` contains a clear note explaining the archive purpose.
- No application code is changed.

## Validation

Before completing the task ensure:

- `git status` runs successfully.
- `ai/tasks/pending` exists.
- `ai/tasks/in-progress` exists.
- `ai/tasks/done` exists and only contains `.gitkeep` unless real VoidEmpires tasks already exist.
- `ai/tasks/obsolete` contains the inherited template task files.
- No `dotnet build` is required because the application solution does not exist yet.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore(ai): reset inherited template task state`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
