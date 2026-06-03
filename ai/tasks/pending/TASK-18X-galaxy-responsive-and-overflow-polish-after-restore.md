# TASK-18X-galaxy-responsive-and-overflow-polish-after-restore

---
id: TASK-18X-galaxy-responsive-and-overflow-polish-after-restore
title: Galaxy responsive and overflow polish after restore
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - qa
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: medium
---

## Goal
Verify the restored Galaxy cockpit remains readable at common desktop widths without overflow regressions.

## Purpose
Protect the accepted visual polish while restoring the broken render path.

## Current Problem
Galaxy is a dense cockpit with map, focus panels, legends, route summaries, and diagnostics. A render restore can easily reintroduce clipping, unsafe wrapping, or horizontal overflow.

## Context
- Earlier Galaxy work already had several layout-hardening passes.
- Other cockpits are now visually dense as well, so shared shell changes may affect Galaxy widths indirectly.
- The user will perform screenshot QA later, but the implementation should land with sensible layout safeguards.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/strategic-map-cockpit-checklist.md`

## Implementation Requirements
1. Check Galaxy layout around `1200px` to `1440px` desktop widths.
2. Ensure:
   - the map panel fits the shell width
   - sidebar or shell chrome does not overlap the map
   - focus and intelligence panels wrap safely
   - planet chips and action chips wrap cleanly
   - collapsed diagnostics do not force overflow
   - no global horizontal scrollbar appears
3. Reuse existing CSS conventions and component classes.
4. Do not redesign the full app shell.

## UI/UX Requirements
- Preserve the established dark cockpit style.
- Keep visual hierarchy map-first, intelligence second, diagnostics last.
- Avoid turning the cockpit into stacked debug blocks unless the viewport truly requires it.

## Backend/API Requirements
- None.

## Safety Constraints
- No gameplay change.
- No new dependencies for layout only.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Restored Galaxy renders without obvious overflow or clipped panels at common desktop widths.
- Diagnostics remain collapsed and secondary.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Manual screenshot QA remains required, but this task should eliminate obvious CSS regressions before handoff.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the work layout-focused.
