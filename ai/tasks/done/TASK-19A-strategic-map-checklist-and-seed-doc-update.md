# TASK-19A-strategic-map-checklist-and-seed-doc-update

---
id: TASK-19A-strategic-map-checklist-and-seed-doc-update
title: Strategic map checklist and seed doc update
status: done
type: platform
team: platform
supporting_teams:
  - docs
  - qa
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: medium
---

## Goal
Update the strategic map and seed documentation so Galaxy expectations under `cockpit-validation` are explicit and easy to validate.

## Purpose
Make the docs strong enough that this regression would have been caught during routine cockpit QA.

## Current Problem
Current seed and strategic-map docs do not explicitly assert that Galaxy must render non-empty strategic content rather than only the shared shell.

## Context
- `development-seed-profiles.md` still documents Galaxy primarily on `/`.
- `strategic-map-cockpit-checklist.md` already describes accepted read-only cockpit behavior.
- The docs should remain concise and copy-pasteable for local QA.

## Files to Inspect First
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/strategic-map-cockpit-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Update the strategic map checklist with:
   - exact QA URL or canonical route
   - required seed profile
   - expected system name such as `Helios Gate`
   - expected map, legend, focus, and read-only handoff panels
   - explicit read-only boundary
2. Add a regression-specific check stating that Galaxy must not render only the generic shell.
3. Cross-link the strategic map checklist from `development-seed-profiles.md` where helpful.
4. Keep the doc operational, short, and easy to paste into a QA runbook.

## UI/UX Requirements
- The checklist should support screenshot QA and quick human verification.
- Primary wording should match the Spanish-first cockpit, while the doc itself may remain in the repository's current documentation style.

## Backend/API Requirements
- None.

## Safety Constraints
- Do not document unsupported Galaxy mutations.
- Do not overclaim `/galaxy` versus `/` until the route decision is implemented.

## Expected Files to Modify
- `docs/dev/strategic-map-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md` if cross-links or checklist wording need alignment

## Acceptance Criteria
- The docs explicitly require a non-empty Galaxy cockpit.
- The docs would have caught the current regression.
- Validation remains green.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Keep route references aligned with the implemented canonical Galaxy path once the routing task is complete.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Favor precise QA wording over long prose.
