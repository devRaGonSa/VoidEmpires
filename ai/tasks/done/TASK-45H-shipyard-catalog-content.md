# TASK-45H

---
id: TASK-45H
title: Shipyard catalog content
status: done
type: full-stack
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Ensure Shipyard shows only ship production queue and ship catalog.

## Context
Shipyard must show a ship catalog immediately using existing catalog data where possible. If authoritative data is too small, expand backend/catalog definitions instead of adding frontend-only mocks.

## Implementation steps

1. Remove non-shipyard information panels.
2. Ensure at least four visible ship cards.
3. Ensure cards show name, class or role, stock, cost, duration, requirements or block reason, and produce action when allowed.
4. Do not add fleet movement actions.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src
- src/VoidEmpires.Application
- tests/VoidEmpires.Tests

## Expected files to modify

- src/VoidEmpires.Frontend/src
- src/VoidEmpires.Application
- tests/VoidEmpires.Tests

## Acceptance criteria

- Shipyard shows at least four authoritative ship entries.
- Shipyard shows only production queue and ship catalog.
- Grid targets four cards per row on desktop.

## Constraints

- Do not add frontend-only fake data
- Do not add fleet movement actions
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
