# TASK-20W-cross-cockpit-smoke-docs-update

---
id: TASK-20W-cross-cockpit-smoke-docs-update
title: Cross-cockpit smoke docs update
status: done
type: platform
team: platform
supporting_teams:
  - docs
  - frontend
  - qa
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: medium
---

## Goal
Update documentation so QA validates the polished cross-cockpit demo flow.

## Purpose
Give QA one concise, repeatable pass for the accepted demo after the UX and language consolidation work lands.

## Current Problem
Docs already exist per cockpit, but there is not yet one concise flow focused on validating the polished multi-cockpit demo after this block.

## Context
- `frontend-foundation-smoke-checklist` already contains a cross-cockpit pass.
- This block should refresh that flow after the polish work is implemented.

## Files to Inspect First
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/*cockpit-checklist.md`

## Implementation Requirements
1. Update the cross-cockpit smoke checklist to cover:
   - apply `cockpit-validation` twice
   - open Galaxy
   - open Planet
   - open Construction
   - open Research
   - open Shipyard
   - open Fleets
   - open Defenses
   - open Ground Army
2. For each cockpit, list:
   - expected primary visible state
   - no-go behaviors
   - one screenshot target
3. Mention that primary UI should avoid raw technical copy.
4. Mention that diagnostics should remain collapsed by default.
5. Keep URLs copy-pasteable and practical.

## UI/UX Requirements
- Docs should be brief, practical, and screenshot-friendly.

## Backend/API Requirements
- None.

## Safety Constraints
- Do not document unsupported systems as implemented.

## Expected Files to Modify
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md` if the smoke flow needs scenario references
- optionally cockpit checklist docs if cross-links need adjustment

## Acceptance Criteria
- QA docs match the polished UI expectations.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Keep the docs concise.
- Do not let the smoke checklist grow into a giant test plan.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on QA documentation.
