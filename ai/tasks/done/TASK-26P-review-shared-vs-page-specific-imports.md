# TASK-26P Review Shared Vs Page Specific Imports

---
id: TASK-26P
title: Review shared imports versus page-specific imports after route splitting
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: medium
---

## Purpose
Reduce accidental shared-bundle bloat by ensuring page-specific helpers and presentation modules stay local to the pages that use them.

## Current problem
Route-level lazy loading alone may not sufficiently reduce the main bundle if page-only helpers are imported from the global shell, route layer, or shared UI components.

## Context from current implementation
The cockpit suite includes multiple presentation and view-model modules that should ideally remain page-local. Shared helpers like route URLs and cockpit status labels should remain global, but page-only presentation code should not leak into the synchronous bundle.

## Goal
Inspect shared shell files and global utilities to ensure page-specific modules are not imported unnecessarily outside their owning pages.

## Implementation steps
1. Inspect imports in `App.tsx`, `AppShell`, sidebar, hero, and other globally rendered shell components.
2. Trace whether page-only helpers are pulled into shared code.
3. Move imports to page-local usage where practical without duplicating logic.
4. Preserve TypeScript correctness and avoid circular dependencies.
5. Rebuild the frontend to verify the import cleanup does not break runtime behavior.

## Files to inspect first
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/pages/

## Expected files to modify
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/

## Implementation requirements
- Inspect imports from `App.tsx`, `AppShell`, `SidebarNav`, `CockpitHero`, and any globally loaded utility files.
- Keep page-specific view models or presentation modules page-local where possible, especially:
- `marketViewModel`
- `espionageViewModel`
- `groundArmyPresentation`
- `defenseViewModel`
- `shipyardPresentation`
- `researchPresentation`
- `planetPresentation` if it is page-specific
- Keep shared route helpers and global status labels available globally.
- Do not create circular imports.
- Do not duplicate large logic just to move an import.

## Frontend requirements
- Preserve TypeScript correctness.
- Preserve existing user-visible behavior.

## Backend/API requirements
- None.

## Safety constraints
- No copy or gameplay logic changes except those required by safer import placement.
- No backend changes.

## Acceptance criteria
- No obvious page-only module remains unnecessarily imported by the global shell.
- The frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Some shared-looking components may legitimately depend on page-level metadata. If an import must stay global, document why instead of forcing an awkward split.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
