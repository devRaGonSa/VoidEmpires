# TASK-045

---
id: TASK-045
title: Align galaxy generation folder with namespace
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2C.2 - Persisted galaxy generation foundation"
priority: low
---

## Goal
Move the galaxy generation implementation file into a folder that matches its namespace.

## Context
`GalaxyGenerator` uses namespace `VoidEmpires.Infrastructure.GalaxyGeneration`, but the file appears to live under `src/VoidEmpires.Infrastructure/Galaxy/GalaxyGenerator.cs`. The physical folder should match the namespace to keep the codebase clear and avoid future confusion.

## Implementation steps

1. Move `GalaxyGenerator.cs` from `src/VoidEmpires.Infrastructure/Galaxy/GalaxyGenerator.cs` to `src/VoidEmpires.Infrastructure/GalaxyGeneration/GalaxyGenerator.cs`.
2. Preserve the namespace `VoidEmpires.Infrastructure.GalaxyGeneration`.
3. Update any project file entries only if required.
4. Update tests only if required.
5. Do not change generator behavior.
6. Do not change public contracts.
7. Do not change database migrations.
8. Do not add new endpoints.
9. Do not add gameplay systems.

## Files to read first

- `src/VoidEmpires.Infrastructure/Galaxy/GalaxyGenerator.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj`
- `tests/VoidEmpires.Tests/*` if any path references exist
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Galaxy/GalaxyGenerator.cs`
- `src/VoidEmpires.Infrastructure/GalaxyGeneration/GalaxyGenerator.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj` if needed
- `tests/VoidEmpires.Tests/*` if needed

## Acceptance criteria

- `GalaxyGenerator.cs` exists under `src/VoidEmpires.Infrastructure/GalaxyGeneration/`.
- The namespace remains `VoidEmpires.Infrastructure.GalaxyGeneration`.
- The old folder is removed if it becomes empty.
- Generator behavior remains unchanged.
- No gameplay behavior is added.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- All tests pass.
- The old folder is removed if empty.
- The new path exists at `src/VoidEmpires.Infrastructure/GalaxyGeneration/GalaxyGenerator.cs`.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore(infrastructure): align galaxy generation folder`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
