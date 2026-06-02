# TASK-9Y

---
id: TASK-9Y
title: Phase 9Y - Development seed contracts and conventions
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Phase 9Y - Development seed contracts and conventions"
priority: high
---

## Goal

Define a safe, repeatable convention for local development seed data without applying it automatically to real databases.

## Context

The repository now has working development validation paths for the strategic map, fleet UI state, and planet visual state, but the development database can still be technically valid and empty.

This task should establish how seed data is intentionally triggered for development validation without changing production startup behavior or mutating databases unless the user explicitly invokes it.

## Implementation steps

1. Inspect existing seed/dev data patterns, migrations, dev endpoints, and README/docs.
2. Define how development seed data should be triggered.
3. Prefer an explicit development-only command, service, endpoint, script, or documented EF-compatible mechanism that fits the current repository conventions.
4. Keep the seed path opt-in and development-only.
5. Ensure it targets validation scenarios for:
   - strategic map / Galaxia
   - fleet UI state / Flotas
   - `PlanetVisualState`
6. Add documentation explaining:
   - target DB: `VoidEmpireDB_Dev` or any explicitly configured dev DB
   - required environment: `Development` or explicit `DevEndpoints` enabled
   - how to run seed safely
   - how to reset and re-run seed idempotently if implemented
7. Do not add the full seed dataset in this task unless it is needed as scaffolding.
8. Do not add migrations unless absolutely required.
9. Do not touch frontend code.

## Files to read first

- `README.md`
- `ai/architecture-index.md`
- `src/VoidEmpires.Web/Program.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `tests/VoidEmpires.Tests/TestWebApplicationFactoryExtensions.cs`

## Expected files to modify

- `README.md`
- `src/VoidEmpires.Web/Program.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `tests/VoidEmpires.Tests/TestWebApplicationFactoryExtensions.cs`

## Acceptance criteria

- A clear development seed convention exists and is documented.
- The seed path is explicit, opt-in, and dev-only.
- Production startup does not auto-run seed logic.
- No frontend changes are introduced.
- Validation commands pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
