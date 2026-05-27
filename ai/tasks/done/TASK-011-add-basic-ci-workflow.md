# TASK-011

---
id: TASK-011
title: Add basic .NET CI workflow
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1 - Technical foundation"
priority: medium
---

## Goal
Add a GitHub Actions workflow that validates the .NET solution on push and pull request.

## Context
VoidEmpires needs a basic CI gate to protect the main branch as development begins.

## Implementation steps

1. Add or update a GitHub Actions workflow under `.github/workflows`.
2. Trigger the workflow on `push` and `pull_request`.
3. Use a stable .NET SDK version compatible with the solution target framework.
4. Run `dotnet restore`.
5. Run `dotnet build --no-restore`.
6. Run `dotnet test --no-build`.
7. Keep the workflow simple.
8. Do not include secrets, deployment, or Docker.

## Files to read first

- `.github/workflows/*`
- `VoidEmpires.sln`
- `README.md`
- `ai/task-template.md`

## Expected files to modify

- `.github/workflows/*`

## Acceptance criteria

- The workflow runs on both push and pull request events.
- The workflow validates restore, build, and test for the solution.
- The workflow is simple and does not deploy anything.
- No application behavior is changed.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build` succeeds.
- `dotnet test` succeeds.
- The workflow YAML is valid.
- No unrelated files are modified.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `ci: add dotnet validation workflow`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
