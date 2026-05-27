# TASK-040

---
id: TASK-040
title: Register galaxy generation services
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2C.2 - Persisted galaxy generation foundation"
priority: high
---

## Goal
Register galaxy generation services through Infrastructure dependency injection.

## Context
The repository already has `IGalaxyGenerator` in Application and `GalaxyGenerator` in Infrastructure. The generator is tested directly but is not yet registered in DI.

## Implementation steps

1. Modify `src/VoidEmpires.Infrastructure`.
2. Modify `src/VoidEmpires.Web` only if needed to call the new registration extension.
3. Modify tests as needed.
4. Add an Infrastructure service registration method such as `AddVoidEmpiresGalaxyGeneration()`.
5. Register `IGalaxyGenerator` to `GalaxyGenerator`.
6. Keep the registration independent from database configuration.
7. Ensure `Program.cs` or the existing composition root calls the registration method.
8. Avoid namespace collisions with `VoidEmpires.Domain.Galaxy.Galaxy`.
9. Keep the namespace as `VoidEmpires.Infrastructure.GalaxyGeneration` or equivalent.
10. Do not persist generated galaxies in this task.
11. Do not add endpoints in this task.
12. Do not connect to real PostgreSQL.
13. Do not call Brevo.

## Files to read first

- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/Program.cs`
- `tests/VoidEmpires.Tests/*`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/*` if needed
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- `IGalaxyGenerator` is registered in DI.
- The registration does not depend on database configuration.
- The application composition root calls the new registration method.
- Tests can resolve `IGalaxyGenerator` from the application service provider.
- Tests do not require PostgreSQL.
- Tests do not call Brevo.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- All tests pass.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(infrastructure): register galaxy generation services`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
