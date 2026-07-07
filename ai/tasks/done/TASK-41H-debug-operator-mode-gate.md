# TASK-41H

---
id: TASK-41H
title: Debug operator mode gate
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Implement a safe internal operator-mode gate for diagnostics/tools.

## Context
Operator tooling may remain available, but only through a local, explicit, off-by-default mechanism.

## Implementation steps

1. Review existing frontend state utilities and routing helpers.
2. Implement or centralize an operator-mode check using a safe local mechanism such as `?operator=1` or an existing localStorage-style internal toggle.
3. Keep the mode off by default and avoid sensitive persisted state.
4. Expose a small reusable helper for components that must hide diagnostics/tools.
5. Keep operator UI clearly separated from normal player flow.

## Files to read first

- docs/dev/product-mode-visibility-contract.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/

## Acceptance criteria

- Operator mode is off by default.
- Operator mode can be explicitly revealed through a safe local mechanism.
- No sensitive state is persisted.
- Normal player flow does not reveal operator tooling.

## Constraints

- Do not add production authentication.
- Do not expose secrets, backend URLs, or raw ids in product mode.
- Do not change gameplay semantics.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`

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
