# TASK-45E

---
id: TASK-45E
title: Core module catalog layout
status: done
type: frontend
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Create or refine a shared compact catalog layout for core modules.

## Context
Construction, Research, Shipyard and Defenses should share a compact structure: page heading, queue section, and catalog grid.

## Implementation steps

1. Refine or create shared layout styles/components for core module catalogs.
2. Target four cards per row on desktop with responsive tablet and mobile fallback.
3. Keep cards compact and avoid long explanatory panels above catalogs.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src

## Expected files to modify

- src/VoidEmpires.Frontend/src

## Acceptance criteria

- Core modules use a compact heading, queue, catalog grid structure.
- Desktop catalog grids target four cards per row.
- Long explanatory panels do not appear above catalogs.

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
