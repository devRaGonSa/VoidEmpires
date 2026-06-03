# TASK-14L

---
id: TASK-14L
title: Phase 14L - Planet economy production summary polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Make the local economy summary more useful even when production profiles are incomplete.

## Context
The current fallback text is technically correct but not very player-friendly. The Planet economy area should separate reserves from production and explain when detailed production is not configured for this build.

## Implementation steps

1. Review the current economy section and fallback copy.
2. Replace the fallback with a clearer Spanish gameplay or development message.
3. Show production, deltas, and energy balance when the data exists.
4. Keep reserves separate from production when detailed profiles are missing.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Application/Economy/ApplyPlanetProductionResult.cs`
- `src/VoidEmpires.Infrastructure/Economy/PlanetEconomyTickService.cs`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`, if needed

## Acceptance criteria

- The fallback is more player-friendly.
- Production and reserves are clearly separated.
- Production data is shown when available.
- The UI does not look broken when production profiles are incomplete.

## Constraints

- Do not invent production values.
- Do not confuse reserves with per-tick output.
- Keep the copy Spanish-first.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- backend tests only if backend read model changes

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
