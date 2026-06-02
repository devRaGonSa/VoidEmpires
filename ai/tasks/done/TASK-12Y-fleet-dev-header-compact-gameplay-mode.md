# TASK-12Y

---
id: TASK-12Y
title: Phase 12Y - Fleet development header compact gameplay mode
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12Y-13D"
priority: medium
---

## Goal
Reduce the visual dominance of the development and header panels on the Fleet page so gameplay content appears sooner.

## Context
The Fleet screen is playable, but the top development surface still consumes too much vertical space. This task keeps the existing safety warnings while making the primary game content feel immediate and readable.

## Implementation steps

1. Inspect the Fleet page and its shared shell/header/sidebar styling.
2. Compact the top development and endpoint sections into smaller, secondary surfaces.
3. Keep warnings and development context visible, but move them out of the way of the gameplay content.
4. Ensure the main gameplay areas appear earlier on the page without changing backend behavior.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- the shell/header/sidebar components used by `/fleets`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- the shell/header/sidebar components used by `/fleets`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- The development header area is visibly more compact.
- Gameplay content appears earlier on the Fleet screen.
- Safety warnings remain present but secondary.
- No new commands are added.
- Split and merge remain disabled or prototype-only.
- Backend contracts are unchanged.

## Constraints

- Do not change backend contracts.
- Do not add new commands.
- Do not enable split or merge execution.
- Do not apply EF migrations.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
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
