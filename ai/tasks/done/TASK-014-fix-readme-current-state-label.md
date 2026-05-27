# TASK-014

---
id: TASK-014
title: Fix README current-state label
status: done
type: docs
team: docs
supporting_teams: []
roadmap_item: "Phase 1 - Technical foundation"
priority: low
---

## Goal
Update the README key document label for `ai/current-state.md` so it no longer describes the repository as Phase 0.

## Context
The repository has moved into Phase 1 technical foundation work, but the README still says `ai/current-state.md` contains Phase 0 status.

## Implementation steps

1. Update the README key documents entry for `ai/current-state.md`.
2. Keep the change limited to the stale phrase.
3. Do not modify application behavior or project files.

## Files to read first

- `README.md`
- `ai/current-state.md`
- `ai/task-template.md`

## Expected files to modify

- `README.md`

## Acceptance criteria

- The README no longer describes `ai/current-state.md` as Phase 0 status.
- The replacement wording accurately describes the current state document.
- No unrelated files are modified.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build` succeeds.
- `dotnet test` succeeds.
- No unrelated files are modified.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs: fix current-state README label`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
