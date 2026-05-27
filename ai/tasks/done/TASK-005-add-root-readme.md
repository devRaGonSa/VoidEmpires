# TASK-005

---
id: TASK-005
title: Add root repository README for VoidEmpires
status: done
type: docs
team: docs
supporting_teams:
  - platform
roadmap_item: "Phase 0 - Repository and AI Platform setup"
priority: high
---

## Goal
Create the missing root `README.md` so the repository has a human-facing entry point that matches the current VoidEmpires planning state.

## Context
The repository currently has no root `README.md`. This leaves the project without a discoverable overview even though multiple AI workflow documents assume a root-level entry document exists.

## Implementation steps

1. Create `README.md`.
2. Summarize the current purpose of the repository, the Phase 0 status, and the role of the AI Platform workflow.
3. Reference the key planning documents under `ai/`.
4. Keep the document accurate to the current repository state and avoid describing application code that does not exist yet.
5. Do not modify scripts or create the `.NET` solution in this task.

## Files to read first

- `ai/task-template.md`
- `ai/repo-context.md`
- `ai/current-state.md`
- `ai/roadmap.md`
- `ai/reports/solution-bootstrap-plan.md`

## Expected files to modify

- `README.md`

## Acceptance criteria

- `README.md` exists at the repository root.
- The README describes VoidEmpires as it exists today, not as a finished application.
- The README points readers to the core AI planning and workflow documents.
- No application code or scripts are changed.

## Validation

Before completing the task ensure:

- `README.md` exists.
- The document does not claim that the `.NET` solution already exists.
- No `dotnet build` is required because the application solution does not exist yet.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs: add root repository readme`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
