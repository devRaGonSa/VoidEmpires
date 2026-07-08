# TASK-45J

---
id: TASK-45J
title: Queue sections compact
status: done
type: frontend
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Make queue sections compact and consistent.

## Context
Empty queue states should be visually small, consistent, and not duplicated.

## Implementation steps

1. Set Construction queue heading to Obras en cola and empty state to Sin obras en cola.
2. Set Research queue heading to Investigacion activa and empty state to Sin investigacion activa.
3. Set Shipyard queue heading to Produccion orbital and empty state to Sin produccion orbital.
4. Set Defenses queue heading to Produccion defensiva and empty state to Sin produccion defensiva.
5. Avoid duplicate empty sentences.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src

## Expected files to modify

- src/VoidEmpires.Frontend/src

## Acceptance criteria

- Queue sections are compact and consistently named.
- Empty states match the required copy exactly in normal UI.
- The same empty sentence is not duplicated.

## Constraints

- Do not remove authenticated sidebar
- Do not remove top resource bar
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
