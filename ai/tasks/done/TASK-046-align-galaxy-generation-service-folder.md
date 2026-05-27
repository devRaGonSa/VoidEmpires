# TASK-046

---
id: TASK-046
title: Align galaxy generation service folder with namespace
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2C.2 - Persisted galaxy generation foundation"
priority: low
---

## Goal
Move the persisted galaxy generation service implementation file into the folder that matches its namespace.

## Context
`GalaxyGenerationService` uses namespace `VoidEmpires.Infrastructure.GalaxyGeneration`, but the file lives under `src/VoidEmpires.Infrastructure/Galaxy/GalaxyGenerationService.cs`. The physical folder should match the namespace, consistent with the generator location.

## Implementation steps

1. Move `GalaxyGenerationService.cs` from `src/VoidEmpires.Infrastructure/Galaxy/GalaxyGenerationService.cs` to `src/VoidEmpires.Infrastructure/GalaxyGeneration/GalaxyGenerationService.cs`.
2. Preserve the namespace `VoidEmpires.Infrastructure.GalaxyGeneration`.
3. Update any project file entries only if required.
4. Update tests only if required.
5. Do not change service behavior.
6. Do not change public contracts.
7. Do not change database migrations.
8. Do not add new endpoints.
9. Do not add gameplay systems.

## Files to read first

- `src/VoidEmpires.Infrastructure/Galaxy/GalaxyGenerationService.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj`
- `tests/VoidEmpires.Tests/GalaxyGenerationServiceTests.cs`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Galaxy/GalaxyGenerationService.cs`
- `src/VoidEmpires.Infrastructure/GalaxyGeneration/GalaxyGenerationService.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj` if needed
- `tests/VoidEmpires.Tests/GalaxyGenerationServiceTests.cs` if needed

## Acceptance criteria

- `GalaxyGenerationService.cs` exists under `src/VoidEmpires.Infrastructure/GalaxyGeneration/`.
- The namespace remains `VoidEmpires.Infrastructure.GalaxyGeneration`.
- The old `src/VoidEmpires.Infrastructure/Galaxy/` folder is removed if it becomes empty.
- Service behavior remains unchanged.
- No gameplay behavior is added.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- All tests pass.
- The old folder is removed if empty.
- The new path exists at `src/VoidEmpires.Infrastructure/GalaxyGeneration/GalaxyGenerationService.cs`.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore(infrastructure): align galaxy generation service folder`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
