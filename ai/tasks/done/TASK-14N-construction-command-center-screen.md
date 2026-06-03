# TASK-14N

---
id: TASK-14N
title: Phase 14N - Construction command center screen
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Build the first playable Construction command center screen.

## Context
The `/construction` route should feel like the dedicated place to manage buildings instead of repeating every Planet section. The screen should focus on the parts of construction that matter most: planet, resources, catalog, actions, queue, and safety notes.

## Implementation steps

1. Review the route foundation created in the previous task.
2. Build the first command center layout around selected planet, local resources, catalog, actions, queue, and diagnostics.
3. Keep technical details collapsed and use Spanish-first labels.
4. Reuse readable building names and categories from the Planet work.

## Files to read first

- `/construction` route page from the previous task
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify

- `/construction` route page and supporting components
- `src/VoidEmpires.Frontend/src/styles.css`
- shared Planet construction helpers, if needed

## Acceptance criteria

- `/construction` focuses on the right gameplay areas.
- The screen feels like a dedicated building management view.
- Spanish-first labels are used throughout.
- Confirmation-before-mutate behavior remains intact.
- Complete-due stays disabled or placeholder unless safe.

## Constraints

- Do not duplicate every Planet section unnecessarily.
- Do not add 3D.
- Keep diagnostics collapsed.
- Keep the construction screen useful as a standalone cockpit.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms `/construction` is useful as a standalone construction screen

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
