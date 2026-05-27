# TASK-016

---
id: TASK-016
title: Add secure database configuration contract
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1B - Persistence foundation with PostgreSQL"
priority: high
---

## Goal
Prepare configuration conventions for the future PostgreSQL connection without storing secrets.

## Context
VoidEmpires needs a safe and deterministic way to configure database connectivity. The repository must include placeholder configuration keys and documentation, but real connection strings must come from environment variables, user-secrets, deployment secrets, or Portainer stack environment configuration.

## Implementation steps

1. Update `appsettings.json` and `appsettings.Development.json` only with safe placeholders.
2. Update `README.md` with safe local configuration instructions.
3. Add documentation under `ai/project-memory` or `ai/reports` if useful.
4. Add a `ConnectionStrings` section if it does not exist.
5. Use an empty or clearly placeholder `DefaultConnection` value.
6. Document the `ConnectionStrings__DefaultConnection` environment variable override.
7. Document optional user-secrets usage for local development.
8. Do not include the real password, NAS hostname, or VPN IP.
9. Mention that the real PostgreSQL 16 database is reachable only through private infrastructure or VPN.
10. Do not add EF Core `DbContext` yet.
11. Do not add migrations yet.
12. Do not connect to the real NAS database.

## Files to read first

- `appsettings.json`
- `appsettings.Development.json`
- `README.md`
- `ai/project-memory/*` if present
- `ai/task-template.md`

## Expected files to modify

- `appsettings.json`
- `appsettings.Development.json`
- `README.md`
- `ai/project-memory/*` or `ai/reports/*` if needed

## Acceptance criteria

- The configuration files contain only safe placeholder values for the default connection.
- The repository documents the `ConnectionStrings__DefaultConnection` override pattern.
- The repository documents the optional user-secrets workflow.
- The repository does not store real secrets or private infrastructure details.
- No database connectivity is attempted.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- `appsettings` files contain no real secrets.
- `README.md` documents the safe override pattern.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore: add secure database configuration contract`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
