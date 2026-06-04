# TASK-20H-ground-army-cockpit-docs-and-smoke-checklist

---
id: TASK-20H-ground-army-cockpit-docs-and-smoke-checklist
title: Ground Army cockpit docs and smoke checklist
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - frontend
  - qa
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: medium
---

## Goal
Document Ground Army cockpit behavior, seed URL, manual QA, and exclusions.

## Purpose
Create repeatable QA guidance so future work does not accidentally add combat, invasion, or boundary confusion between Ground Army and neighboring modules.

## Current Problem
The new cockpit needs repeatable QA documentation so future blocks do not accidentally add combat, invasion, or mix Ground Army into Construction, Defenses, or Fleets.

## Context
- Existing docs already cover other cockpits.
- Ground Army needs equivalent documentation and explicit exclusion notes.

## Files to Inspect First
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/planet-module-boundaries.md`
- `docs/dev/defenses-cockpit-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Create `docs/dev/ground-army-cockpit-checklist.md`.
2. Include the seeded URL:
   - `/ground-army?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
3. Include checks for:
   - page loads
   - context shows Aurelia
   - garrison or readiness is visible
   - structures and options are visible
   - available and blocked states are clear if supported
   - queue and complete-due behavior are truthful
   - handoff to Construction, Defenses, and Fleets works
   - diagnostics stay collapsed
   - no combat
   - no invasion
   - no 3D
   - no fleet movement
4. Update `docs/dev/frontend-foundation-smoke-checklist.md`.
5. Update `docs/dev/development-seed-profiles.md` with Ground Army expectations.
6. Update `docs/dev/planet-module-boundaries.md` if Ground Army boundaries need clarification.

## UI/UX Requirements
- Docs should be practical, screenshot-friendly, and easy to follow during manual QA.

## Backend/API Requirements
- No backend change is expected.

## Safety Constraints
- Do not document unsupported invasion or combat behavior as implemented.

## Expected Files to Modify
- `docs/dev/ground-army-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/planet-module-boundaries.md` if needed

## Acceptance Criteria
- A user can QA Ground Army from the docs alone.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- The docs must clearly say that combat and invasion are excluded from this block.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on QA documentation.
