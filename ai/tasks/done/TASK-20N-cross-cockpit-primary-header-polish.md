# TASK-20N-cross-cockpit-primary-header-polish

---
id: TASK-20N-cross-cockpit-primary-header-polish
title: Cross-cockpit primary header polish
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Reduce repeated generic development header dominance and make each cockpit's primary identity clearer.

## Purpose
Let each accepted cockpit feel recognizable at first glance while still preserving the fact that these are development-safe routes.

## Current Problem
Many pages currently start with very similar generic development panels such as `Superficie de mando solo para desarrollo` or `Superficie de endpoints de desarrollo`. These warnings are useful, but they dominate the hierarchy and make different cockpits feel too similar.

## Context
- The app is still a development cockpit.
- The user wants the accepted cockpit set to feel like a coherent first playable demo.
- The technical warning should remain visible, but the module identity should be more prominent than the repetitive backend or route warning.

## Files to Inspect First
- app shell or shared header components
- all accepted cockpit pages under `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Review the shared page-header pattern used across accepted cockpits.
2. Make the cockpit-specific title and description more prominent where appropriate.
3. Reduce repetitive development or endpoint copy in the primary hierarchy.
4. Move repetitive backend profile text to a smaller or clearly secondary panel if safe.
5. Keep the fact that the routes are Development-only visible somewhere on each relevant page.
6. Do not remove backend URL or profile context entirely if it is currently useful for QA.
7. Ensure pages still communicate:
   - module name
   - purpose
   - key safety limitation

## UI/UX Requirements
- Each cockpit should be recognizable at first glance.
- Development warning should not overshadow the main gameplay-oriented content.
- Spanish-first.

## Backend/API Requirements
- None.

## Safety Constraints
- No route or auth changes.
- No gameplay changes.

## Expected Files to Modify
- shared header-related frontend files if they exist
- targeted cockpit pages
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Headers feel less repetitive.
- Cockpit identity is clearer across accepted modules.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Do not over-redesign the app shell.
- Keep scope bounded to hierarchy and copy placement.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on header polish.
