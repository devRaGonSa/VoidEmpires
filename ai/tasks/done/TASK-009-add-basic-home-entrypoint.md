# TASK-009

---
id: TASK-009
title: Add basic home entrypoint
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1 - Technical foundation"
priority: medium
---

## Goal
Add a minimal home entrypoint for the initial web application.

## Context
VoidEmpires will eventually have a full game dashboard and UI. For now, the application only needs a basic root endpoint or page that confirms the product identity.

## Implementation steps

1. Modify only `VoidEmpires.Web` and tests as needed.
2. Ensure `GET /` returns HTTP 200.
3. Return a minimal response that identifies the application as VoidEmpires.
4. Keep the implementation simple and compatible with a future UI replacement.
5. Add a test that verifies `GET /` returns success and includes the text `VoidEmpires`.
6. Do not add frontend framework complexity, authentication, database dependencies, or gameplay features.

## Files to read first

- `src/VoidEmpires.Web/Program.cs` or equivalent web entrypoint
- `tests/VoidEmpires.Tests/*`
- `ai/reports/solution-bootstrap-plan.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- `GET /` returns HTTP 200.
- The root response includes `VoidEmpires`.
- The implementation remains minimal and easy to replace later.
- The change stays isolated to the web project and tests.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build` succeeds.
- `dotnet test` succeeds.
- The root endpoint test passes.
- No unrelated files are modified.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat: add basic VoidEmpires home entrypoint`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
