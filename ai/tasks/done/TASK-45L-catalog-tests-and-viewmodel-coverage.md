# TASK-45L

---
id: TASK-45L
title: Catalog tests and viewmodel coverage
status: done
type: test
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Add or update tests ensuring catalogs are populated and exposed.

## Context
Backend tests should protect authoritative catalog sizes and safe blocked/available metadata without requiring SQL Server.

## Implementation steps

1. Add or update backend tests to verify at least four entries for buildings, research, ships, and defenses.
2. Verify blocked and available metadata remains safe.
3. Verify normal tests do not require SQL Server.
4. Add frontend or static checks if useful.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Application
- tests/VoidEmpires.Tests

## Expected files to modify

- tests/VoidEmpires.Tests
- src/VoidEmpires.Application

## Acceptance criteria

- Catalog population tests cover buildings, research, ships and defenses.
- Tests pass without SQL Server.
- Safe metadata remains covered.

## Constraints

- Do not require SQL Server for automated tests
- Keep the change minimal

## Validation

Before completing the task ensure:

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend

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
