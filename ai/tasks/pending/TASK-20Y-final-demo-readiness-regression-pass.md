# TASK-20Y-final-demo-readiness-regression-pass

---
id: TASK-20Y-final-demo-readiness-regression-pass
title: Final demo readiness regression pass
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
  - docs
  - qa
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Run a final narrow regression pass over all accepted cockpits after polish.

## Purpose
Confirm that shared UX and copy work did not destabilize the existing accepted cockpit flows before the block is declared demo-ready.

## Current Problem
Cross-cockpit polish will touch shared files and repeated patterns. That kind of work can easily create regressions even when gameplay behavior itself does not change.

## Context
- The accepted cockpit set currently includes:
   - Galaxy
   - Planet
   - Construction
   - Research
   - Shipyard
   - Fleets
   - Defenses
   - Ground Army
- This regression pass should be narrow, practical, and aligned with the current manual QA style.

## Files to Inspect First
- `docs/dev/frontend-foundation-smoke-checklist.md`
- accepted cockpit pages
- related read-model tests only if a regression is suspected

## Implementation Requirements
1. Validate that:
   - `/galaxy` still renders a read-only strategic cockpit
   - `/planet` dashboard still loads
   - `/construction` still loads actions
   - `/research` still exposes catalog and queue
   - `/shipyard` still exposes stock and queue
   - `/fleets` still exposes squads and transfers
   - `/defenses` still exposes readiness
   - `/ground-army` still exposes garrison and readiness
2. Prefer documented manual QA backed by existing validation rather than heavy new browser automation.
3. Add or update automated tests only if a real regression is found in read models or shared helpers.
4. Ensure frontend build still passes.
5. Verify that no obvious primary UI raw technical language remains in the accepted demo flow.

## UI/UX Requirements
- No accepted cockpit should feel blank, broken, or dominated by raw technical copy in the primary view.

## Backend/API Requirements
- No backend change is expected unless a regression exposes a contract issue.

## Safety Constraints
- No new gameplay systems.
- No feature expansion disguised as polish.

## Expected Files to Modify
- final smoke docs if the regression pass is documented there
- focused tests only if an actual regression requires coverage
- targeted frontend files only if a discovered regression must be fixed

## Acceptance Criteria
- Demo-readiness regression pass is documented.
- Build and tests pass.
- Frontend build passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Manual visual QA remains user-driven even after this regression pass.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on regression confirmation.
