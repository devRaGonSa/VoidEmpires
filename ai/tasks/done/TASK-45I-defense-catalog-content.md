# TASK-45I

---
id: TASK-45I
title: Defense catalog content
status: done
type: full-stack
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Rework Defensas into only defensive queue and defensive catalog.

## Context
Defenses must show a defense catalog immediately using existing catalog data where possible. If authoritative data is too small, expand backend/catalog definitions instead of adding frontend-only mocks.

## Implementation steps

1. Remove Entrada de vista, Cargar contexto defensivo, Dashboard defensivo, Postura actual y siguiente paso, Revisar cobertura protectora, and raw id inputs.
2. Ensure at least four visible defense cards.
3. Ensure cards show name, quantity or level if available, cost, duration, requirements or block reason, and production action if supported.
4. If defensive production mutation is unsupported, show disabled or prepared action without fake behavior.

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

- Defenses shows at least four authoritative defense entries.
- Defenses shows only defensive queue and defense catalog.
- Grid targets four cards per row on desktop.

## Constraints

- Do not add frontend-only fake data
- Do not add combat
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
