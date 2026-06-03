# TASK-18Y-galaxy-dev-diagnostics-and-state-summary

---
id: TASK-18Y-galaxy-dev-diagnostics-and-state-summary
title: Galaxy dev diagnostics and state summary
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: medium
---

## Goal
Add or restore useful collapsed diagnostics for Galaxy without polluting the primary cockpit.

## Purpose
Give future QA and debugging passes enough state visibility to diagnose regressions quickly while keeping the main viewport gameplay-first.

## Current Problem
When Galaxy became empty, the UI did not surface enough contextual detail to distinguish route, context, data, and selection failures.

## Context
- `StrategicMapPage.tsx` already includes a collapsed technical disclosure area.
- Other cockpits also use collapsed diagnostics rather than exposing raw DTOs in the main flow.
- The regression showed the value of a compact state summary even when the full cockpit fails.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- diagnostics patterns in Research, Shipyard, and Planet pages

## Implementation Requirements
1. Add or restore a collapsed section such as `Diagnostico secundario`.
2. Include concise state fields:
   - `civilizationId`
   - systems count
   - planets count
   - fleet markers count
   - transfers count
   - selected or focused system id and name
   - API load status if available
3. Keep raw JSON or full payloads behind an additional nested disclosure if needed.
4. Do not surface raw GUIDs, DTO names, or capability keys as primary cards.
5. Keep diagnostics collapsed by default and visually secondary.

## UI/UX Requirements
- Spanish label and support text.
- Secondary visual priority only.
- The summary should be readable at a glance and not become a giant payload dump.

## Backend/API Requirements
- No backend change expected.

## Safety Constraints
- No sensitive data exposure.
- No mutation controls.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Diagnostics help explain future empty-state regressions.
- The primary cockpit remains map-first.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Avoid turning the diagnostics area into the only place where critical failures are visible; user-facing states still belong in the main cockpit region.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep diagnostics concise and collapsed.
