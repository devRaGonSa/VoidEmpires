# TASK-020

---
id: TASK-020
title: Update current state after persistence foundation
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1B - Persistence foundation with PostgreSQL"
priority: medium
---

## Goal
Refresh project documentation after the PostgreSQL persistence foundation tasks are implemented.

## Context
After the persistence foundation exists, `ai/current-state.md` and `README.md` should accurately describe the new state.

## Implementation steps

1. Update `ai/current-state.md`.
2. Update `README.md` if needed.
3. Do not modify application behavior.
4. Do not add new packages or entities.
5. Do not add migrations.
6. Keep the documentation evidence-based and conservative.

## Files to read first

- `ai/current-state.md`
- `README.md`
- `ai/repo-context.md`
- `ai/task-template.md`

## Expected files to modify

- `ai/current-state.md`
- `README.md` if needed

## Acceptance criteria

- `ai/current-state.md` mentions that Phase 1B persistence foundation has started or completed.
- `ai/current-state.md` mentions PostgreSQL 16 as the selected primary database engine.
- `ai/current-state.md` mentions EF Core + Npgsql as the intended persistence stack.
- `ai/current-state.md` mentions whether a `DbContext` skeleton exists if TASK-018 has completed.
- `ai/current-state.md` mentions that no gameplay entities or migrations exist yet unless later tasks add them.
- `ai/current-state.md` mentions that tests do not require a real PostgreSQL instance.
- `README.md` mentions safe configuration and that tests run without a database if not already documented.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- Documentation matches the actual repository state.
- No unrelated files are modified.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs: update state after persistence foundation`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
