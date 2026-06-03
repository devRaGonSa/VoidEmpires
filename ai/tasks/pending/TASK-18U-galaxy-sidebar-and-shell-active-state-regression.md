# TASK-18U-galaxy-sidebar-and-shell-active-state-regression

---
id: TASK-18U-galaxy-sidebar-and-shell-active-state-regression
title: Galaxy sidebar and shell active state regression
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: medium
---

## Goal
Ensure the shell and sidebar route Galaxy to the real strategic cockpit and highlight it correctly.

## Purpose
Prevent route shadowing or placeholder fallback behavior from making Galaxy look like a generic shell page.

## Current Problem
The reported screen looks like a shared shell and generic development placeholder instead of the accepted Galaxy cockpit. That strongly suggests routing or page-collision regression.

## Context
- `App.tsx` currently wires `Galaxia` to `/`.
- Specialized module routes now share more shell infrastructure and placeholder pages.
- A generic shell header is acceptable, but it must be followed by Galaxy-specific cockpit content.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- sidebar or shell UI components
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`

## Implementation Requirements
1. Verify that Galaxy navigation resolves to `StrategicMapPage`, not `ModuleCabinPage` or a no-match shell.
2. Audit route ordering so `/galaxy` or `/` cannot be swallowed by a generic module path.
3. Ensure the active sidebar state highlights Galaxy consistently for the chosen canonical route.
4. Ensure the page title and primary content are strategic-map specific, not only generic development copy.
5. Keep shared shell framing if it is app-wide, but ensure Galaxy content follows immediately after it.

## UI/UX Requirements
- Active-state highlight for `Galaxia` must be clear.
- Do not duplicate navigation affordances unnecessarily.
- Placeholder copy must not replace real Galaxy content.

## Backend/API Requirements
- None.

## Safety Constraints
- No gameplay changes.
- No mutations.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/App.tsx`
- shell or sidebar components if active-state logic needs alignment
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx` only if route collision needs cleanup

## Acceptance Criteria
- Galaxy routes map to the real cockpit page.
- Sidebar active state matches the actual Galaxy route.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- A no-match route inside a persistent shell can look like a valid page at first glance; verify both route selection and rendered content.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the fix route-focused.
