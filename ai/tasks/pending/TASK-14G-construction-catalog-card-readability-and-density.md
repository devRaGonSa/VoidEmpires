# TASK-14G

---
id: TASK-14G
title: Phase 14G - Construction catalog card readability and density
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Make the construction catalog less raw, less long, and more readable.

## Context
The current construction catalog feels like a stretched DTO table with sparse numeric rows. It should be redesigned into a compact, scannable gameplay catalog that highlights names, categories, costs, durations, and availability.

## Implementation steps

1. Review the current construction catalog card layout in Planet.
2. Redesign the card structure without changing the global app shell.
3. Prioritize the gameplay fields that matter most for quick scanning.
4. Group cards by category and prevent horizontal overflow.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- related Planet catalog presentation helpers, if needed

## Acceptance criteria

- Construction cards emphasize name, category, level, cost, duration, availability, and actions.
- The catalog is easier to scan.
- Available cards stand out without making blocked cards look ready.
- No common desktop-width horizontal overflow appears.

## Constraints

- Keep dark galactic cockpit styling.
- Do not redesign the entire app shell.
- Do not expose raw DTO rows as the main visual structure.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms the catalog is scannable and no longer looks like raw DTO rows

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
