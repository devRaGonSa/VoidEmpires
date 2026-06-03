# TASK-17S-shipyard-cockpit-docs-and-smoke-checklist

---
id: TASK-17S-shipyard-cockpit-docs-and-smoke-checklist
title: Shipyard cockpit docs and smoke checklist
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - frontend
  - backend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: medium
---

## Goal
Document Shipyard cockpit behavior, seeded URLs, manual QA steps, and intentional exclusions so future work does not blur Shipyard and Fleet responsibilities.

## Purpose
Create the same kind of operational continuity that already exists for Planet, Construction, Research, and Fleet work.

## Current Problem
Without dedicated docs, later tasks may accidentally treat Shipyard as a Fleet mutation surface or may overclaim support for combat, movement, split, or merge. Manual QA also needs one stable checklist for the seeded cockpit.

## Context
- The repo already contains development smoke checklists and module-boundary docs.
- Shipyard will become a development-safe cockpit foundation rather than a final production UI.
- The seeded context should remain stable and explicitly documented.

## Files to Inspect First
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-module-boundaries.md`
- `docs/dev/fleet-api-contracts.md`
- `ai/current-state.md`

## Implementation Requirements
1. Create `docs/dev/shipyard-cockpit-checklist.md`.
2. Include the seeded URL:
   - `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
3. Document smoke checks for:
   - page loads;
   - Aurelia context is visible;
   - resources are visible;
   - production catalog is visible;
   - available and blocked asset options are visible if supported;
   - confirmation appears before enqueue if enabled;
   - queue updates after one enqueue if enabled;
   - complete-due is enabled only if safe, otherwise disabled or placeholder;
   - stock-to-Fleet relationship is clear;
   - diagnostics stay collapsed or secondary;
   - no 3D;
   - no combat;
   - no fleet movement from Shipyard.
4. Update `docs/dev/frontend-foundation-smoke-checklist.md` with a Shipyard section or cross-reference.
5. Update `docs/dev/planet-module-boundaries.md` if Shipyard needs explicit boundary wording.
6. Do not document unsupported behavior as implemented.

## UI/UX Requirements
- Docs should be practical, screenshot-friendly, and easy to follow in development.
- Player-facing descriptions should stay Spanish-aware even if the docs are mixed-language.

## Backend/API Requirements
- No backend changes are expected from documentation work alone.

## Safety Constraints
- Do not claim final gameplay support.
- Do not imply that Shipyard can move fleets, split groups, merge groups, or resolve combat.

## Expected Files to Modify
- `docs/dev/shipyard-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-module-boundaries.md` if needed

## Acceptance Criteria
- Shipyard manual QA can be run from docs alone.
- Intentional exclusions are explicit.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Browser-based visual QA remains manual unless later automation is added.
- Keep the checklist aligned with the actual seeded state instead of ideal future behavior.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Keep the docs operational and concrete.
