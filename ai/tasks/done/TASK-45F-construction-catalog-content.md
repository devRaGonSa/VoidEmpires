# TASK-45F

---
id: TASK-45F
title: Construction catalog content
status: done
type: full-stack
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Ensure Construction shows at least four building entries from authoritative catalog or backend state.

## Context
Construction must show a building catalog immediately using existing catalog data where possible. If authoritative data is too small, expand backend/catalog definitions instead of adding frontend-only mocks.

## Implementation steps

1. Confirm Construction displays a building catalog immediately.
2. Ensure at least four visible building cards.
3. Ensure cards show name, level, cost, duration, requirement or block reason, and build action when allowed.
4. Remove unrelated info panels from the Construction module.

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

- Construction shows at least four authoritative building entries.
- Cards expose required player-facing metadata and actions.
- No unrelated info panels remain.

## Constraints

- Do not add frontend-only fake data
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
