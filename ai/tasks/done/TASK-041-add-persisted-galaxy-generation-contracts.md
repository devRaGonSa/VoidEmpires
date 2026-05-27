# TASK-041

---
id: TASK-041
title: Add persisted galaxy generation contracts
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2C.2 - Persisted galaxy generation foundation"
priority: high
---

## Goal
Add application-level contracts for generating and persisting a galaxy.

## Context
VoidEmpires can generate a galaxy in memory. The next step is to define an application contract for generating and saving a galaxy through persistence, without exposing EF Core details to callers.

## Implementation steps

1. Modify `src/VoidEmpires.Application`.
2. Modify tests as needed.
3. Do not implement persistence in this task.
4. Do not modify Infrastructure unless required for compilation.
5. Do not add endpoints.
6. Do not add player or civilisation ownership.
7. Add contracts and models such as `IGalaxyGenerationService`, `GenerateAndPersistGalaxyRequest`, and `GenerateAndPersistGalaxyResult`.
8. Include request fields for name, seed, solar system count, minimum planets per system, maximum planets per system, and an optional overwrite flag if needed.
9. Include result fields for success, galaxy ID, galaxy name, solar system count, planet count, and errors.
10. Keep contracts provider-agnostic.
11. Do not expose EF Core types.
12. Keep result models deterministic and testable.

## Files to read first

- `src/VoidEmpires.Application/*`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Application/*`
- `tests/VoidEmpires.Tests/*` if needed

## Acceptance criteria

- Application contains contracts for persisted galaxy generation.
- Contracts do not expose EF Core types.
- Result models are deterministic and testable.
- Success and failure factory methods exist if consistent with existing Application patterns.
- No persistence is implemented yet.

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
3. Commit with message: `feat(application): add persisted galaxy generation contracts`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
