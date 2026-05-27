# TASK-012

---
id: TASK-012
title: Ignore .NET build artifacts
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1 - Technical foundation"
priority: high
---

## Goal
Update repository ignore rules so .NET build outputs are not left as untracked files after local validation.

## Context
The initial .NET solution creates `bin/` and `obj/` folders under `src/` and `tests/` during restore, build, and test. These generated files should not be committed or appear as pending work.

## Implementation steps

1. Update `.gitignore` with standard .NET build output patterns.
2. Keep the existing `ai/worker.lock` ignore rule.
3. Do not modify application behavior or project files.

## Files to read first

- `.gitignore`
- `README.md`
- `ai/task-template.md`

## Expected files to modify

- `.gitignore`

## Acceptance criteria

- `bin/` and `obj/` outputs are ignored throughout the repository.
- Existing ignore behavior for `ai/worker.lock` is preserved.
- No application behavior is changed.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build` succeeds.
- `dotnet test` succeeds.
- `git status` does not show generated `bin/` or `obj/` folders as untracked.
- No unrelated files are modified.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore: ignore dotnet build artifacts`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
