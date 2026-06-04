# TASK-20V-sidebar-and-module-list-polish

---
id: TASK-20V-sidebar-and-module-list-polish
title: Sidebar and module list polish
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: medium
---

## Goal
Polish the sidebar module list and active states now that all main cockpits are implemented.

## Purpose
Reflect the current module reality more clearly so implemented cabinas feel stable and future modules feel intentionally deferred instead of broken.

## Current Problem
The sidebar began as placeholder navigation. Now most major modules are real cockpits, but the active, disabled, and future states may still read like early scaffolding instead of a deliberate navigation model.

## Context
- Accepted modules now include:
   - Planeta
   - Construccion
   - Investigacion
   - Ejercito Tierra
   - Astillero
   - Defensas
   - Flotas
   - Galaxia
- Future placeholder modules still include:
   - Espionaje
   - Alianza
   - Mercado
   - Ranking

## Files to Inspect First
- app shell or sidebar component
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Ensure active state works correctly for:
   - `/planet`
   - `/construction`
   - `/research`
   - `/ground-army`
   - `/shipyard`
   - `/defenses`
   - `/fleets`
   - `/galaxy`
2. Distinguish implemented versus future modules subtly but clearly.
3. Ensure future modules do not look broken.
4. Keep labels Spanish.
5. Do not hide future modules unless the current design strongly requires it.

## UI/UX Requirements
- The sidebar should show progression clearly.
- Active item must be unmistakable.
- Disabled or future items should be less prominent than implemented modules.

## Backend/API Requirements
- None.

## Safety Constraints
- No route removal for accepted modules.
- No new module behavior.

## Expected Files to Modify
- sidebar or app shell component files
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx` if shared module-list behavior lives there
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Sidebar reflects the current module reality better.
- Active, implemented, and future states are clearer.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Future module behavior will continue to evolve later.
- Keep this task focused on clarity, not a full navigation redesign.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on sidebar polish.
