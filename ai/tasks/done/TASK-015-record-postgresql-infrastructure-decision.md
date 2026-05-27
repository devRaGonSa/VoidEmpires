# TASK-015

---
id: TASK-015
title: Record PostgreSQL infrastructure decision
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 1B - Persistence foundation with PostgreSQL"
priority: high
---

## Goal
Document the decision to use PostgreSQL 16 as the primary database engine for VoidEmpires.

## Context
Earlier planning documents may still mention SQL Server as the initial database direction. The current infrastructure direction is PostgreSQL 16, hosted outside the repository and reachable only through private infrastructure such as a VPN for local or private environments. This decision must be recorded without exposing secrets or private connection details.

## Implementation steps

1. Update `ai/repo-context.md` to state that PostgreSQL 16 is the primary relational database.
2. Update `ai/current-state.md` to reflect the PostgreSQL persistence direction.
3. Update `ai/architecture-index.md` if it still mentions SQL Server as the planned database engine.
4. Create or update `ai/project-memory/decisions.md` with a short decision entry if that file is present or appropriate for the repository.
5. Replace or qualify stale SQL Server references in planning documents.
6. Do not modify application code.
7. Do not add EF Core packages yet.
8. Do not add connection strings with real hostnames, usernames, passwords, or VPN details.

## Files to read first

- `ai/repo-context.md`
- `ai/current-state.md`
- `ai/architecture-index.md`
- `ai/project-memory/decisions.md` if it exists
- `ai/task-template.md`

## Expected files to modify

- `ai/repo-context.md`
- `ai/current-state.md`
- `ai/architecture-index.md`
- `ai/project-memory/decisions.md` if needed

## Acceptance criteria

- The repository documents PostgreSQL 16 as the primary database engine.
- The repository documents EF Core with Npgsql as the intended persistence stack unless changed later by a future decision.
- The repository documents that the real database configuration is supplied externally.
- The repository documents that secrets must not be committed.
- The repository documents that CI and tests must not depend on the real NAS PostgreSQL database.
- Stale SQL Server references are removed or clearly qualified.
- No private connection data is committed.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- Documentation search shows no stale unqualified SQL Server references.
- No real database password, VPN address, or private NAS host is committed.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs: record PostgreSQL persistence decision`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
