# TASK-14C

---
id: TASK-14C
title: Phase 14C - Planet construction building label source of truth
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Create or consolidate a source of truth for player-facing building labels and construction categories.

## Context
The current Planet cockpit renders building and category values as raw numbers in the primary UI. This task should identify where that happens and add a safe label source so the screen can show gameplay-readable names instead of raw enum values.

## Implementation steps

1. Inspect `BuildingType`, building catalog data, construction services, and current frontend presentation helpers.
2. Identify why the Planet cockpit currently renders category and building values as numbers.
3. Add or extend a frontend-safe label mapper or backend display metadata projection.
4. Keep raw enum values only in collapsed diagnostics.

## Files to read first

- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Application/Buildings/`
- `tests/VoidEmpires.Tests/BuildingCatalogTests.cs`
- `tests/VoidEmpires.Tests/BuildingCategoryTests.cs`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- Planet cockpit view or backend display metadata files, if needed

## Acceptance criteria

- Building labels are Spanish and gameplay-readable.
- Category labels are Spanish and meaningful.
- Core building IDs and enum persistence stay unchanged.
- Raw enum values remain diagnostics-only.

## Constraints

- Do not change persisted building identifiers.
- Do not introduce fake gameplay data.
- Prefer a frontend-safe label mapper unless backend display metadata is clearly needed.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- if backend display metadata changes are added: `dotnet build --no-restore` and `dotnet test --no-build`

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
