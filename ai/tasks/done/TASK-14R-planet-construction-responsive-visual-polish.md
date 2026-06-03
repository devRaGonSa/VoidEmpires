# TASK-14R

---
id: TASK-14R
title: Phase 14R - Planet construction responsive visual polish
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Polish responsive layout for Planet and Construction.

## Context
The Planet and Construction screens need one more responsive pass so the new gameplay readability work holds up at common desktop widths without overflow, button dominance, or cramped layout issues.

## Implementation steps

1. Review `/planet` and `/construction` at common desktop widths.
2. Prevent horizontal overflow and improve spacing or density where needed.
3. Keep sidebar and top resource bar stable.
4. Make sure blocked buttons do not dominate blocked cards.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `/construction` route page
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/components/ui/`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `/construction` route page
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- No obvious horizontal overflow at common desktop widths.
- The layout is easier to scan.
- Buttons do not overpower blocked cards.
- Diagnostics stay collapsed.

## Constraints

- Preserve the existing dark galactic cockpit style.
- Do not expand scope into 3D or unrelated screens.
- Keep the shell stable.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms the layout is readable and does not overflow

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
