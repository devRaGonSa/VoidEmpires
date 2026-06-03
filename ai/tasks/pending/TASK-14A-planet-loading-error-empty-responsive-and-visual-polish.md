# TASK-14A

---
id: TASK-14A
title: Phase 14A - Planet loading error empty responsive and visual polish
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Polish the Planet cockpit so it is usable as a development gameplay screen with clear hierarchy, readable states, and responsive behavior.

## Context
After the main Planet panels exist, the route still needs a final polish pass for loading, error, empty, and responsive states. The goal is to make `/planet` feel stable and legible while keeping technical details collapsed and avoiding visual complexity.

## Implementation steps

1. Review the current Planet cockpit layout at common desktop widths and identify hierarchy or overflow issues.
2. Add Spanish loading, error, and empty states across the page and its key panels.
3. Harden responsive behavior to avoid horizontal overflow and clarify section priority.
4. Keep technical diagnostics collapsed by default and preserve the current dark galactic cockpit style.

## Files to read first

- Planet route page and supporting Planet cockpit components
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/components/ui/` shared shell primitives
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `src/VoidEmpires.Frontend/README.md`

## Expected files to modify

- Planet route page and supporting Planet cockpit components
- `src/VoidEmpires.Frontend/src/styles.css`
- shared UI primitives only if a small polish extension is needed

## Acceptance criteria

- `/planet` has clear Spanish loading, error, and empty states.
- Desktop layout avoids horizontal overflow.
- The page hierarchy clearly prioritizes identity or status, resources, buildings, construction queue or actions, and technical diagnostics.
- Technical details stay collapsed by default.
- The screen looks like a readable development cockpit, not a debug panel.

## Constraints

- Avoid excessive visual complexity.
- Keep styling aligned with Fleets and Galaxy.
- Do not add unrelated gameplay systems.
- Do not expand scope into 3D or final art.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms `/planet` is legible, responsive, and not debug-panel-like

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
