# TASK-18W-galaxy-readonly-action-guard-regression

---
id: TASK-18W-galaxy-readonly-action-guard-regression
title: Galaxy readonly action guard regression
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
Ensure Galaxy remains strictly read-only after the cockpit render is restored.

## Purpose
Prevent the regression fix from accidentally turning Galaxy into a mutation surface.

## Current Problem
Restoring the cockpit can tempt shortcut fixes such as wiring direct action buttons to mutating cabins or API calls. That would violate the accepted Galaxy boundary.

## Context
- Earlier accepted Galaxy behavior was strategic inspection with handoffs to dedicated cabins.
- Other cockpits already own mutations through explicit confirmations.
- The strategic map read model already exposes command metadata; those hints must not become direct execution authority.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- route handoff helpers or components
- `docs/dev/strategic-map-cockpit-checklist.md`

## Implementation Requirements
1. Audit Galaxy buttons, links, and command metadata displays.
2. Allowed interactions:
   - inspect system
   - inspect planet
   - open Planet
   - open Fleets
   - open Construction, Research, or Shipyard with context
   - review transfer and readiness summaries
3. Disallowed direct actions:
   - create transfer
   - create fleet
   - split or merge fleet
   - enqueue construction
   - enqueue research
   - enqueue shipyard production
   - complete due actions
4. Ensure no mutating API client is called from `StrategicMapPage`.
5. If capability metadata is shown, keep it clearly secondary and read-only.

## UI/UX Requirements
- Labels must not imply command execution from Galaxy.
- Spanish-first copy should reinforce the read-only boundary.

## Backend/API Requirements
- No backend change.

## Safety Constraints
- Galaxy read-only is mandatory.
- No hidden mutation affordances through keyboard shortcuts or helper hooks.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- Galaxy cockpit docs if the read-only boundary needs clearer wording

## Acceptance Criteria
- Restored Galaxy cockpit exposes no direct mutating action path.
- Cross-cabin handoffs still work.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Read-only command metadata is acceptable as long as execution remains in the owning cabin.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Treat this as a guardrail task, not a feature task.
