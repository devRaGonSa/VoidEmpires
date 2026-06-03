# TASK-13U

---
id: TASK-13U
title: Phase 13U - Planet buildings grid and development summary
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Present planet buildings as a readable development grid with player-facing labels and categories.

## Context
The Planet cockpit should expose existing building state as something a player can scan quickly, not as a list of raw enum or persistence rows. If building catalog or category data already exists, the frontend should translate it into meaningful sections and readable status cues.

## Implementation steps

1. Review existing building data from the Planet cockpit read model and any building catalog helpers already available.
2. Render building cards or rows with name, level, category, and short role or effect copy when available.
3. Group buildings into meaningful gameplay categories when the data supports it.
4. Keep technical diagnostics collapsed and make sparse or dense lists both readable.

## Files to read first

- Planet cockpit route and components from Phases 13Q-13T
- `src/VoidEmpires.Frontend/src/api/` Planet and building type files
- `src/VoidEmpires.Application/Buildings/`
- `tests/VoidEmpires.Tests/BuildingCatalogTests.cs`
- `tests/VoidEmpires.Tests/BuildingCategoryTests.cs`

## Expected files to modify

- Planet cockpit components related to buildings
- Planet frontend view-model helpers
- `src/VoidEmpires.Frontend/src/styles.css`, if needed

## Acceptance criteria

- Existing buildings render with readable labels, levels, and categories.
- Catalog-backed not-yet-built entries can appear as available or locked candidates when the data supports it.
- Primary UI avoids raw enum names.
- Categories such as economy, infrastructure, military, research, orbital or shipyard, and other are readable in Spanish when supported.
- Diagnostics remain secondary or collapsed.

## Constraints

- Prefer reusing existing catalog data.
- Do not add unrelated backend mutations.
- Keep the layout compact and readable for both small and large building sets.
- Keep primary labels Spanish-first.

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
