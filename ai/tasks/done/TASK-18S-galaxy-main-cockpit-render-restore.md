# TASK-18S-galaxy-main-cockpit-render-restore

---
id: TASK-18S-galaxy-main-cockpit-render-restore
title: Galaxy main cockpit render restore
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - qa
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: high
---

## Goal
Restore the full Galaxy strategic cockpit whenever valid strategic data exists.

## Purpose
Recover the previously accepted read-only Galaxy experience instead of leaving only shell framing and generic development cards.

## Current Problem
Galaxy should show the accepted strategic map cockpit, but the main content is currently bypassed or not rendering after the shared shell.

## Context
- `StrategicMapPage.tsx` still contains the accepted cockpit structure, including the map stage, focus panels, summary cards, handoffs, and collapsed diagnostics.
- Earlier accepted Galaxy UI included `Cabina estrategica de Galaxia`, the 2D strategic map, seeded system focus, intelligence summaries, legend, and read-only handoffs.
- The restore task should reconnect existing code paths before considering any rewrite.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/strategic-map-cockpit-checklist.md`
- local git history for strategic map cockpit layout commits, if needed

## Implementation Requirements
1. Fix the render guard, route path, or state branch that currently prevents the cockpit from appearing.
2. Ensure the map stage renders whenever the returned systems list is non-empty.
3. Ensure the selected or focused system defaults sensibly to the requested, owned, visible, or first available system.
4. Ensure the cockpit includes:
   - strategic summary
   - 2D map surface
   - focus system panel
   - planet intelligence list
   - transfer route summary
   - legend or visibility cues
   - collapsed diagnostics
5. Ensure handoff links to Planet, Construction, and Fleets still work from focused planet context.
6. Keep Galaxy strictly read-only.
7. Do not add new mutation buttons or command execution flows.

## UI/UX Requirements
- Preserve the dark galactic cockpit style already accepted.
- Keep map-first hierarchy.
- Avoid panel overflow and diagnostics-first presentation.

## Backend/API Requirements
- No backend change expected unless the API shape has drifted.

## Safety Constraints
- No Galaxy mutations.
- No fleet movement from Galaxy.
- No 3D or WebGL.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- `galaxy` navigation for the seeded civilization shows the full strategic cockpit.
- `Helios Gate` or the current seeded system is visible in the restored map.
- Summary, map, focus, and legend/route panels render together.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If route refactors bypassed this page entirely, reconnect the correct component instead of duplicating Galaxy UI elsewhere.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Restore rather than redesign.
