# TASK-002

---
id: TASK-002
title: Write VoidEmpires repository context
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 0 - Repository and AI Platform setup"
priority: high
---

## Goal
Replace the generic template repository context with a VoidEmpires-specific context document.

## Context
VoidEmpires needs a persistent repository context for future AI Platform tasks and Codex CLI executions. This context must explain the product, the intended architecture direction, team workflow, and important constraints.

## Implementation steps

1. Update `ai/repo-context.md` so it is specific to VoidEmpires.
2. Update `ai/current-state.md` if needed so it reflects the repository's initial state.
3. Keep the content deterministic, concise, and useful for future task execution.
4. Do not create application code.
5. Do not create a .NET solution yet.
6. Do not modify task workflow scripts.

## Files to read first

- `ai/task-template.md`
- `ai/repo-context.md`
- `ai/current-state.md`
- `ai/roadmap.md`

## Expected files to modify

- `ai/repo-context.md`
- `ai/current-state.md`

## Acceptance criteria

- `ai/repo-context.md` is clearly VoidEmpires-specific.
- The document includes the product, gameplay pillars, technical direction, workflow, and constraints described in the task brief.
- `ai/current-state.md` states that the project is in Phase 0, that the AI Platform template is installed, and that no production application code exists yet unless already detected.
- No application code is changed.

## Validation

Before completing the task ensure:

- `ai/repo-context.md` no longer reads like a generic AI Platform template document.
- `ai/current-state.md` reflects the current initial state.
- No `dotnet build` is required because the application solution does not exist yet.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs(ai): write VoidEmpires repository context`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
