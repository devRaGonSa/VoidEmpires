# TASK-13T

---
id: TASK-13T
title: Phase 13T - Planet overview resources and colony status panels
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Implement the first visible Planet cockpit overview as a readable 2D management surface.

## Context
After the route and read model exist, the first Planet screen should stop looking like a debug dump and start feeling like a playable development cockpit. The first slice should focus on identity, status, resources, economy summary, colony context, and quick links.

## Implementation steps

1. Review the current shell primitives and Planet route foundation.
2. Build the first Planet header, status, resources, production summary, colony or control state, and quick-link panels.
3. Use Spanish labels and keep technical ids collapsed or visibly secondary.
4. Add useful empty states without inventing art assets or 3D rendering.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/` Planet route files from Phase 13S
- `src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/UiCard.tsx`, if present
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/README.md`

## Expected files to modify

- Planet route page and supporting Planet cockpit components
- `src/VoidEmpires.Frontend/src/styles.css`
- shared UI primitives only if a small extension is needed

## Acceptance criteria

- The Planet cockpit has a readable header and overview layout.
- Resources, colony status, control state, and quick links are visible in Spanish.
- Technical ids are secondary or collapsed.
- Empty states stay useful and readable.
- The page feels like a playable development surface rather than a raw API dump.

## Constraints

- Do not add 3D rendering.
- Do not invent final art assets.
- Keep the styling aligned with the existing dark galactic cockpit direction.
- Avoid unrelated backend changes in this task.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms the Planet screen looks like a management surface and not a debug response dump

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
