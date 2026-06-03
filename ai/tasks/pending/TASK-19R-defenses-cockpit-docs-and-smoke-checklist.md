# TASK-19R-defenses-cockpit-docs-and-smoke-checklist

---
id: TASK-19R-defenses-cockpit-docs-and-smoke-checklist
title: Defenses cockpit docs and smoke checklist
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - qa
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: medium
---

## Goal
Document Defenses cockpit behavior, seeded QA flow, smoke checklist expectations, and explicit exclusions.

## Purpose
Give future implementation and QA runs a repeatable checklist that protects module boundaries and keeps the cockpit from silently expanding into combat or other unsupported systems.

## Current Problem
Once Defenses becomes a real cockpit, it needs the same durable manual QA and scope documentation that already exists for Galaxy, Construction, Research, Shipyard, and Planet.

## Context
- Existing cockpit-specific docs are the acceptance memory for the repo.
- `frontend-foundation-smoke-checklist.md` and `development-seed-profiles.md` already anchor the current manual QA story.
- This task should record the seeded URL and exactly what the user should confirm visually.

## Files to Inspect First
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/planet-module-boundaries.md`
- `ai/current-state.md`

## Implementation Requirements
1. Create `docs/dev/defenses-cockpit-checklist.md`.
2. Include the seeded QA route:
   - `/defenses?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
3. Document manual checks including:
   - page loads
   - context shows `Aurelia`
   - defensive readiness is visible
   - structures and options are visible
   - available and blocked states are clear if supported
   - queue or complete-due behavior is truthful
   - handoff to Construction, Shipyard, and Fleets is visible
   - diagnostics are collapsed
   - no combat behavior exists
   - no 3D or WebGL exists
   - no fleet movement is launched from Defenses
4. Update `docs/dev/frontend-foundation-smoke-checklist.md` with the new cockpit route.
5. Update `docs/dev/development-seed-profiles.md` with Defenses expectations.
6. Update `docs/dev/planet-module-boundaries.md` if the placeholder wording must now become cockpit-foundation wording.

## UI/UX Requirements
- Docs should be practical, concise, and screenshot-QA friendly.
- Copy should reinforce that Defenses is a readiness cockpit, not a combat surface.

## Backend/API Requirements
- No backend change expected.

## Safety Constraints
- Do not document unsupported combat behavior as if it exists.
- Keep exclusions explicit.

## Expected Files to Modify
- `docs/dev/defenses-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/planet-module-boundaries.md` if needed

## Acceptance Criteria
- A teammate can QA Defenses from the docs without guessing.
- The route, seed profile, expected states, and exclusions are clearly documented.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Keep the docs aligned with the implemented safe scope; if mutation remains disabled, say so plainly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the work documentation-focused and operational.
