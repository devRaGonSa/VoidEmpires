# TASK-007

---
id: TASK-007
title: Bootstrap the VoidEmpires .NET solution
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1 - Technical foundation"
priority: high
---

## Goal
Create the initial .NET solution and project structure for VoidEmpires.

## Context
VoidEmpires needs a clean modular monolith foundation. The solution should separate the web entrypoint, application logic, domain model, infrastructure, and tests without introducing premature microservices.

## Implementation steps

1. Create `VoidEmpires.sln`.
2. Create `src/VoidEmpires.Web` as an ASP.NET Core web project.
3. Create `src/VoidEmpires.Application` as a class library.
4. Create `src/VoidEmpires.Domain` as a class library.
5. Create `src/VoidEmpires.Infrastructure` as a class library.
6. Create `tests/VoidEmpires.Tests` as an xUnit test project.
7. Wire project references as specified in the task brief.
8. Add assembly marker classes where useful for the application, domain, and infrastructure projects.
9. Keep the web app minimal and buildable.
10. Do not add database code, authentication, or gameplay features yet.

## Files to read first

- `ai/repo-context.md`
- `ai/roadmap.md`
- `ai/architecture-index.md`
- `ai/reports/solution-bootstrap-plan.md`
- `ai/task-template.md`

## Expected files to modify

- `VoidEmpires.sln`
- `src/VoidEmpires.Web/*`
- `src/VoidEmpires.Application/*`
- `src/VoidEmpires.Domain/*`
- `src/VoidEmpires.Infrastructure/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- The solution and all five projects exist.
- The project types match the task brief.
- The project references match the specified dependency graph.
- Assembly marker classes are present where useful.
- The solution remains minimal and buildable.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build` succeeds.
- `dotnet test` succeeds.
- The build is free of avoidable warnings where reasonably possible.
- No unrelated files are modified.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore: bootstrap VoidEmpires dotnet solution`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files per project area when possible.
- Prefer changes under 200 lines of code per file where practical.
- Split the work into additional tasks if limits are exceeded.
