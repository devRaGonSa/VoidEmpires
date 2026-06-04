# TASK-20Q-pending-to-classify-label-cleanup

---
id: TASK-20Q-pending-to-classify-label-cleanup
title: Pending to classify label cleanup
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: medium
---

## Goal
Reduce `pendiente de clasificar` fallback labels for known seeded gameplay items.

## Purpose
Make the accepted demo feel intentional by ensuring known seeded entities get proper player-facing names instead of generic fallback placeholders.

## Current Problem
Several pages have shown fallback labels such as `Activo orbital pendiente de clasificar`, `Estructura terrestre pendiente de clasificar`, and `Preparacion terrestre pendiente de clasificar`. Those fallbacks are acceptable for truly unknown values, but they should be rare under `cockpit-validation`.

## Context
- The seed profile and accepted cockpits now expose a richer set of items.
- Presentation helpers may still be missing mappings for seeded known enums or categories.
- This is a frontend-taxonomy cleanup task, not a domain renaming task.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/groundArmyPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/defensesPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- relevant frontend DTO and type files
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`

## Implementation Requirements
1. Identify fallback labels that are visible under `cockpit-validation`.
2. Map known seeded asset, defense, ground, or related types to specific player-facing labels.
3. Keep fallback behavior for truly unknown values.
4. Avoid raw enum leakage in the replacement path.
5. Add helper coverage if there is an existing lightweight pattern; otherwise rely on frontend build plus visual QA.
6. Do not change backend enum or persisted values.

## UI/UX Requirements
- Known seeded cards should show clear player-facing names.
- Unknown fallback labels may remain, but they should become rare in the main demo flow.

## Backend/API Requirements
- None unless backend display metadata is already the established source and must be aligned.

## Safety Constraints
- No gameplay-rule changes.
- No domain enum renames.

## Expected Files to Modify
- targeted presentation helpers under `src/VoidEmpires.Frontend/src/utils/`
- optionally small related type or page files if wiring needs adjustment
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs` only if a seed inconsistency truly blocks proper display metadata and tests are added

## Acceptance Criteria
- Major known fallback labels are replaced with specific names.
- Truly unknown values still have safe fallback behavior.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` only if backend or seed files are changed

## Notes / Residual Risks
- Some unknown domain values may still exist until broader taxonomy work happens later.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on seeded label coverage.
