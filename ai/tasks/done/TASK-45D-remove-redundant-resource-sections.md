# TASK-45D

---
id: TASK-45D
title: Remove redundant resource sections
status: done
type: frontend
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Remove redundant in-page resources available sections from core modules.

## Context
Resources are shown by the global top resource bar. Core module cards may still show costs and must retain backend resource data for validation and actions.

## Implementation steps

1. Remove large Recursos disponibles blocks inside Construction, Research, Shipyard and Defenses.
2. Preserve costs on cards.
3. Preserve backend resource data used by validations and actions.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src

## Expected files to modify

- src/VoidEmpires.Frontend/src

## Acceptance criteria

- Core modules do not show separate large Recursos disponibles blocks.
- Card costs and backend validations still work.

## Constraints

- Do not remove authenticated sidebar
- Do not remove top resource bar
- Do not modify unrelated modules
- Keep the change minimal

## Validation

Before completing the task ensure:

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
