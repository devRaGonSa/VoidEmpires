# TASK-010

---
id: TASK-010
title: Document local development commands
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1 - Technical foundation"
priority: medium
---

## Goal
Update the repository documentation with the commands needed to build, test, and run the initial application.

## Context
After the .NET solution exists, developers and AI workers need deterministic commands for local validation and basic runtime checks.

## Implementation steps

1. Update `README.md` with a development section.
2. Update `ai/current-state.md` to reflect the current technical foundation state.
3. Include prerequisites, build commands, test commands, the web run command, and the health endpoint.
4. Include a short AI workflow summary that matches the repository rules.
5. Keep the documentation evidence-based and conservative.
6. Do not modify application behavior or add new projects.

## Files to read first

- `README.md`
- `ai/current-state.md`
- `ai/repo-context.md`
- `ai/task-template.md`

## Expected files to modify

- `README.md`
- `ai/current-state.md`

## Acceptance criteria

- `README.md` contains the new development section and commands.
- `ai/current-state.md` reflects the current state after the initial application bootstrap work.
- The documentation is concise, accurate, and aligned with the repository workflow.
- No application behavior is changed.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build` succeeds.
- `dotnet test` succeeds.
- `README.md` contains the new development commands.
- `ai/current-state.md` reflects the current state.
- No unrelated files are modified.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs: document VoidEmpires development commands`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
