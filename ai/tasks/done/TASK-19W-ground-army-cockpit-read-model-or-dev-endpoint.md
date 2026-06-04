# TASK-19W-ground-army-cockpit-read-model-or-dev-endpoint

---
id: TASK-19W-ground-army-cockpit-read-model-or-dev-endpoint
title: Ground Army cockpit read model or dev endpoint
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
  - qa
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Provide a stable Ground Army UI state read model using existing services or a new Development-only endpoint if needed.

## Purpose
Give the frontend one coherent, safe source of terrestrial readiness state instead of forcing the page to stitch together fragile data from unrelated building or queue endpoints.

## Current Problem
A useful Ground Army cockpit needs coherent state for selected planet context, garrison or readiness, structures, available or blocked actions, queue or training state, local resources, and limitations. Stitching raw Construction or building endpoints in the frontend would be brittle and make later UI work much harder to reason about.

## Context
- Planet, Research, Shipyard, Defenses, and Strategic Map already rely on cockpit-style read models.
- Ground Army should follow the same approach when no equivalent contract exists.
- The route must remain Development-safe and must not require production auth.

## Files to Inspect First
- `src/VoidEmpires.Application/`
- `src/VoidEmpires.Infrastructure/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/`
- existing dev UI state services for Planet, Research, Shipyard, Defenses, and Fleet
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`

## Implementation Requirements
1. Reuse existing read endpoints if they already provide enough Ground Army state.
2. If no adequate endpoint exists, add a Development-only Ground Army UI state endpoint, for example:
   - `GET /api/dev/ground-army/ui-state?civilizationId={id}&planetId={id}`
3. The read model should include, where supported:
   - civilization id
   - planet id, name, and system
   - ownership or control state
   - population or manpower
   - local resources relevant for terrestrial training or support
   - current ground structures or buildings
   - garrison or readiness summary
   - troop roster or placeholder readiness
   - available and blocked Ground Army options
   - queue items if a training or recruitment queue exists
   - complete-due capability or limitation
   - diagnostics and limitations
4. Follow existing development endpoint conventions for validation and error handling.
5. Add focused tests that cover:
   - Development-only gating
   - invalid civilization id
   - invalid planet id
   - not controlled planet if relevant
   - success with `cockpit-validation`
   - no mutation on read
   - available and blocked distribution if supported

## UI/UX Requirements
- The DTO should support Spanish presentation, but the primary copy can live in frontend helpers.
- The contract must support both rich seeded states and honest read-only placeholder states.

## Backend/API Requirements
- Do not add a production endpoint.
- Do not require auth.
- Avoid migrations.
- Prefer extending an existing dev read-state service instead of creating a parallel stack when a close fit already exists.

## Safety Constraints
- The read endpoint must not mutate state.
- No combat.
- No invasion.
- No troop movement.
- No Galaxy mutations.

## Expected Files to Modify
- Ground Army read-state service and DTO files under `src/VoidEmpires.Application/` or `src/VoidEmpires.Infrastructure/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- focused tests under `tests/VoidEmpires.Tests/`

## Acceptance Criteria
- Ground Army cockpit can fetch stable state from a single safe read model.
- Tests cover endpoint or service behavior if added.
- Backend build and tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend types are touched

## Notes / Residual Risks
- If backend support is still thin, expose honest readiness and limitation state rather than pretending full troop systems already exist.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the contract narrow and cockpit-specific.
