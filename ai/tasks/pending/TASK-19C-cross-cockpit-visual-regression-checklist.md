# TASK-19C-cross-cockpit-visual-regression-checklist

---
id: TASK-19C-cross-cockpit-visual-regression-checklist
title: Cross cockpit visual regression checklist
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - qa
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: medium
---

## Goal
Add a final cross-cockpit visual acceptance checklist covering Galaxy and the already accepted neighboring cockpits.

## Purpose
Ensure the Galaxy fix does not regress Planet, Construction, Research, Shipyard, or Fleets.

## Current Problem
Galaxy is the current blocker, but the final QA pass must verify the full cockpit suite that shares route helpers, seed state, and shell patterns.

## Context
- The repository already has per-cockpit checklists.
- `cockpit-validation` is intended as the richer shared QA baseline.
- The final checklist should be a compact one-stop path for user validation.

## Files to Inspect First
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/strategic-map-cockpit-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`

## Implementation Requirements
1. Add a final visual QA list that starts by applying `cockpit-validation` twice.
2. Include direct checks for:
   - Galaxy
   - Planet
   - Construction
   - Research
   - Shipyard
   - Fleets
3. For each screen, list the minimal expected non-empty result.
4. Keep URLs copy-pasteable.
5. Include reminder constraints:
   - no manual SQL
   - no 3D
   - no combat
   - Galaxy read-only
   - mutations only in dedicated cabins with confirmation

## UI/UX Requirements
- The checklist should be compact but complete.
- It should help visual QA without forcing users to hunt across many docs first.

## Backend/API Requirements
- None.

## Safety Constraints
- Do not overclaim unsupported cabins or gameplay.
- Ground Army and Defenses remain placeholders unless another task changes that.

## Expected Files to Modify
- `docs/dev/frontend-foundation-smoke-checklist.md`
- related cockpit checklist docs if small cross-links are needed

## Acceptance Criteria
- One doc path can drive the final cross-cockpit visual pass.
- Galaxy and neighboring cockpit expectations are explicit and comparable.
- Validation remains green.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Keep the checklist synchronized with the canonical Galaxy route once the routing task lands.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Favor practical QA steps over narrative explanation.
